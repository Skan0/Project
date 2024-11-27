using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
public class Net_Mng : MonoBehaviour
{
    // Lobby -> �÷��̾ ���ϴ� ������ ã�ų� �� ������ ����� ����� �� �ִ� 
    // Relay -> ��Ī�� �÷��̾���� Relay�� Join Code�� ����Ǿ�, ȣ��Ʈ-Ŭ���̾�Ʈ ������� �ǽð� ��Ƽ�÷��� ȯ���� ����
    // -> ������Ī�� ������ �ִ� ���� �θ��� �˾Ƽ� �� �濡 �־��ְ� ���� �÷����ϰ� ������ִ°�
    private Lobby currentlobby;

    public Button StartMatchButton, JoinMatchButton;
    public InputField input;
    public Text JoinCodeText;
    // �񵿱� -> ���ÿ� �Ͼ�� �ʴ´�.
    // web�� ��� ��û�� ������ �� delay�� �ִµ� �̷��� ��Ⱑ �ִ� �۾��� �񵿱� �۾��̴�.
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        StartMatchButton.onClick.AddListener(() => StartMatchMaking());
        JoinMatchButton.onClick.AddListener(() => JoinGameWithCode(input.text));
    }
    public async void JoinGameWithCode(string inputJoinCode)
    {
        if (string.IsNullOrEmpty(inputJoinCode)) {
            Debug.Log("��ȿ���� ���� �ڵ��Դϴ�.");
            return;
        }
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );
            StartClient();
            Debug.Log("Join code�� ���� ���� ����");
        }
        catch(RelayServiceException ex)
        {
            Debug.Log($"���� ���� ����: {ex.Message}");
        }
    }
    public async void StartMatchMaking()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("�α��� ���� �ʾҽ��ϴ�.");
            return;
        }
        currentlobby = await FindAvaileLobby();

        if (currentlobby == null) { 
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(currentlobby.Id);
        }
    }

    private async Task<Lobby> FindAvaileLobby()
    {
        try
        {
            var querryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (querryResponse.Results.Count > 0)
            {
                return querryResponse.Results[0];
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log("�κ� ã�� ����"+e);
        }
        return null;
    }
    private async Task CreateNewLobby()
    {
        try
        {
            currentlobby = await LobbyService.Instance.CreateLobbyAsync("������Ī��", 2);
            Debug.Log("���ο� �� ������:" + currentlobby.Id);
            await AllocateRelayServerAndJoin(currentlobby);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("�κ� ���� ����." + e);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            currentlobby =await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log("�濡 ���ӵǾ����ϴ�." + currentlobby.Id);
            StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log("�κ� ���� ���� : " + e);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            JoinCodeText.text = joinCode;
            Debug.Log("Relay ���� �Ҵ� �Ϸ�. Join Code : " + joinCode);
        }
        catch (RelayServiceException e) 
        {
            Debug.Log("Relay ���� �Ҵ� ����" + e); 
        } 
    }
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("ȣ��Ʈ�� ���۵Ǿ����ϴ�.");
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Ŭ���̾�Ʈ�� ���۵Ǿ����ϴ�.");
    }
}
