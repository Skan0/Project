using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public partial class Net_Mng : MonoBehaviour
{
    // Lobby -> �÷��̾ ���ϴ� ������ ã�ų�, �� ������ ����� ����� �� �ִ�
    // Relay -> ��Ī�� �÷��̾���� Relay�� Join Code�� ����Ǿ�, ȣ��Ʈ-Ŭ���̾�Ʈ ������� �ǽð� ��Ƽ�÷��� ȯ���� ����
    private Lobby currentLobby;

    private const int maxPlayers = 2;
    private string gameplaySceneName = "GamePlayScene";
    public Button StartMatchButton;
    public GameObject Matching_Object;
    public Button CancelButton;
    private async void Start() // �񵿱� -> ���ÿ� �Ͼ�� �ʴ´�.
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        StartMatchButton.onClick.AddListener(() => StartMatchmaking());
        //JoinMatchButton.onClick.AddListener(() => JoinGameWithCode(fieldText.text));
    }
}
