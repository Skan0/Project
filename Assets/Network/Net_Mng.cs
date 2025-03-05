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

    private async void Start()// 요청을 보내고 받는데 시간이 걸리는 작업을 비동기 작업이라고 한다.
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
            Debug.Log("로그인 되지 않았습니다.");
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

            Debug.Log("로비찾기 실패" + ex);
        }
        return null;
    }
    private async Task CreateNewLobby()
    {
        try
        {
            CurLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭방", 2);
            Debug.Log("새로운 방 생성됨");
            await AllocateRelayServerAndJoin(CurLobby);
            StartHost();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log("로비찾기 실패" + ex);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay 서버 할당 완료. Join Code : "+ joinCode);
        }
        catch(RelayServiceException ex) 
        {
            Debug.Log("Relay 서버 할당 실패 "+ ex);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            CurLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyId);
            Debug.Log("방에 접속되었습니다." + CurLobby.Id);
            StartClient();
        }
        catch(LobbyServiceException ex) 
        {
            Debug.Log("로비 참가 실패 : " + ex);
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
        Debug.Log("클라이언트가 연결되었습니다.");
    }
}
