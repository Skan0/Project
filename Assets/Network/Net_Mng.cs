using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using Unity.Services.Core;
using System.Net.Security;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using UnityEngine.UI;

public class Net_Mng : MonoBehaviour
{
    private Lobby CurLobby;

    public Button StartMatchButton;

    private async void Start()// ��û�� ������ �޴µ� �ð��� �ɸ��� �۾��� �񵿱� �۾��̶�� �Ѵ�.
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        StartMatchButton.onClick.AddListener(() => startMatchMaking());
    }
    public async void startMatchMaking()
    {
        if (!!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("�α��� ���� �ʾҽ��ϴ�.");
            return;
        }
        
        CurLobby = await FindAvailableLobby();

        if (CurLobby == null)
        {
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(CurLobby.Id);
        }
    }
    private async Task<Lobby> FindAvailableLobby()
    {
        try
        {
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch (LobbyServiceException ex)
        {

            Debug.Log("�κ�ã�� ����" + ex);
        }
        return null;
    }
    private async Task CreateNewLobby()
    {
        try
        {
            CurLobby = await LobbyService.Instance.CreateLobbyAsync("������Ī��", 2);
            Debug.Log("���ο� �� ������");
            await AllocateRelayServerAndJoin(CurLobby);
            StartHost();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log("�κ�ã�� ����" + ex);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay ���� �Ҵ� �Ϸ�. Join Code : "+ joinCode);
        }
        catch(RelayServiceException ex) 
        {
            Debug.Log("Relay ���� �Ҵ� ���� "+ ex);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            CurLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyId);
            Debug.Log("�濡 ���ӵǾ����ϴ�." + CurLobby.Id);
            StartClient();
        }
        catch(LobbyServiceException ex) 
        {
            Debug.Log("�κ� ���� ���� : " + ex);
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
        Debug.Log("Ŭ���̾�Ʈ�� ����Ǿ����ϴ�.");
    }
}
