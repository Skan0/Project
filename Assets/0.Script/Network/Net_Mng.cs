using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
public partial class Net_Mng : MonoBehaviour
{
    // Lobby -> 플레이어가 원하는 게임을 찾거나 새 게임을 만들고 대기할 수 있는 
    // Relay -> 매칭된 플레이어들의 Relay의 Join Code로 연결되어, 호스트-클라이언트 방식으로 실시간 멀티플레이 환경을 유지
    // -> 랜덤매칭을 돌리고 있는 유저 두명을 알아서 한 방에 넣어주고 같이 플레이하게 만들어주는것
    private Lobby currentlobby;

    private const int maxPlayers = 2;
    public string gamePlaySceneName = "GamePlayScene";
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

}
