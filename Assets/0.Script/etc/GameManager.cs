using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameState
{
    Stop,
    Play,
    End
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;
    public GameObject[] Player;
    public Transform[] PlayerSpawnPoint;
    public int PlayerCount;
    void Awake()
    {
        // GameManager 인스턴스가 하나만 존재하도록 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 오브젝트가 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);  // 이미 존재하는 경우 새로 생성된 오브젝트는 파괴
        }
    }

    private void Start()
    {
        for (int i = 0; i < PlayerCount; i++) {
            Instantiate(Player[i], PlayerSpawnPoint[i]);
        }
        StartCoroutine(startGame(5f));
    }
    IEnumerator startGame(float delay)
    {
        yield return new WaitForSeconds( delay);
        UpdateGameState(GameState.Play);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;  // 현재 상태를 업데이트

        switch (newState)
        {
            case GameState.Stop:
                HandleStopState();  // Stop 상태일 때 실행되는 로직
                break;

            case GameState.Play:
                HandlePlayState();  // Play 상태일 때 실행되는 로직
                break;
        }
    }

    private void HandleStopState()
    {
        // 게임이 멈췄을 때 실행되는 로직
        Debug.Log("게임이 멈췄습니다.");
        Time.timeScale = 0f; // 게임 일시정지
    }

    private void HandlePlayState()
    {
        // 게임이 진행 중일 때 실행되는 로직
        Debug.Log("게임이 진행 중입니다.");
        Time.timeScale = 1f; // 게임 재개
    }

}
