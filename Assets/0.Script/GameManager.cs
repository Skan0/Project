using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Stop,
    Play,
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameState State;

    void Awake()
    {
        // GameManager �ν��Ͻ��� �ϳ��� �����ϵ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ���� �ٲ� ������Ʈ�� �ı����� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �����ϴ� ��� ���� ������ ������Ʈ�� �ı�
        }
    }

    private void Start()
    {
        StartCoroutine(startGame(5f));
    }
    IEnumerator startGame(float delay)
    {
        yield return new WaitForSeconds( delay);
        UpdateGameState(GameState.Play);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;  // ���� ���¸� ������Ʈ

        switch (newState)
        {
            case GameState.Stop:
                HandleStopState();  // Stop ������ �� ����Ǵ� ����
                break;

            case GameState.Play:
                HandlePlayState();  // Play ������ �� ����Ǵ� ����
                break;
        }
    }

    private void HandleStopState()
    {
        // ������ ������ �� ����Ǵ� ����
        Debug.Log("������ ������ϴ�.");
        Time.timeScale = 0f; // ���� �Ͻ�����
    }

    private void HandlePlayState()
    {
        // ������ ���� ���� �� ����Ǵ� ����
        Debug.Log("������ ���� ���Դϴ�.");
        Time.timeScale = 1f; // ���� �簳
    }

}
