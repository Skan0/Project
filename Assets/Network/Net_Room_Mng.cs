using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using UnityEngine;
using Unity.VisualScripting;

public partial class Net_Mng : MonoBehaviour
{
    public async void JoinGameWithCode(string inputJoinCode)
    {
        if (string.IsNullOrEmpty(inputJoinCode))
        {
            Debug.Log("��ȿ���� ���� Join Code�Դϴ�.");
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
            Debug.Log("Join Code�� ���ӿ� ���� ����!");
        }
        catch (RelayServiceException e)
        {
            Debug.Log("���� ���� ���� : " + e);
        }
    }

    public async void StartMatchmaking()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("�α��ε��� �ʾҽ��ϴ�.");
            return;
        }

        Matching_Object.SetActive(true);
        currentLobby = await FindAvailableLooby();

        if (currentLobby == null)
        {
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(currentLobby.Id);
        }
    }


    private async Task<Lobby> FindAvailableLooby()
    {
        // ���� ó��
        try
        {
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("�κ� ã�� ����" + e);
        }
        return null;
    }

    private async void DestroyLobby(string lobbyId)
    {
        try
        {
            if (!string.IsNullOrEmpty(lobbyId))
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                Debug.Log("Lobby destroyed : " + lobbyId);
            }
            if(NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                Matching_Object.SetActive(false);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Failed to destroy lobby : " + e.Message);
        }
    }

    private async Task CreateNewLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("������Ī��", maxPlayers);
            Debug.Log("���ο� �� ������:" + currentLobby.Id);
            await AllocateRelayServerAndJoin(currentLobby);
            CancelButton.onClick.AddListener(() => DestroyLobby(currentLobby.Id));
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
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log("�濡 ���ӵǾ����ϴ�." + currentLobby.Id);
            StartClient();
        }
        catch (LobbyServiceException e)
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
            //JoinCodeText.text = joinCode;
            Debug.Log("Relay ���� �Ҵ� �Ϸ�. Join Code : " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log("Relay ���� �Ҵ� ����:" + e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("ȣ��Ʈ�� ���۵Ǿ����ϴ�.");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        OnPlayerJoined();
    }

    private void OnHostDisconnected(ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnHostDisconnected;
        }
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Ŭ���̾�Ʈ�� ����Ǿ����ϴ�.");
    }
}
