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
    // Lobby -> 플레이어가 원하는 게임을 찾거나 새 게임을 만들고 대기할 수 있는 
    // Relay -> 매칭된 플레이어들의 Relay의 Join Code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티플레이 환경을 유지
    // -> 랜덤매칭을 돌리고 있는 유저 두명을 알아서 한 방에 넣어주고 같이 플레이하게 만들어주는것
    private Lobby currentlobby;

    public Button StartMatchButton, JoinMatchButton;
    public InputField input;
    public Text JoinCodeText;
    // 비동기 -> 동시에 일어나지 않는다.
    // web에 어떠한 요청을 보냈을 때 delay가 있는데 이러한 대기가 있는 작업이 비동기 작업이다.
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
            Debug.Log("유효하지 않은 코드입니다.");
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
            Debug.Log("Join code로 게임 접속 성공");
        }
        catch(RelayServiceException ex)
        {
            Debug.Log($"게임 접속 실패: {ex.Message}");
        }
    }
    public async void StartMatchMaking()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인 되지 않았습니다.");
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
            Debug.Log("로비 찾기 실패"+e);
        }
        return null;
    }
    private async Task CreateNewLobby()
    {
        try
        {
            currentlobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭방", 2);
            Debug.Log("새로운 방 생성됨:" + currentlobby.Id);
            await AllocateRelayServerAndJoin(currentlobby);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 생성 실패." + e);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            currentlobby =await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log("방에 접속되었습니다." + currentlobby.Id);
            StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log("로비 참가 실패 : " + e);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            JoinCodeText.text = joinCode;
            Debug.Log("Relay 서버 할당 완료. Join Code : " + joinCode);
        }
        catch (RelayServiceException e) 
        {
            Debug.Log("Relay 서버 할당 실패" + e); 
        } 
    }
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("호스트가 시작되었습니다.");
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트가 시작되었습니다.");
    }
}
