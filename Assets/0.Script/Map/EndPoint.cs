using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    GameManager gameManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            gameManager.UpdateGameState(GameState.Stop);
        }
    }
}
