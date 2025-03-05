using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameTimer : NetworkBehaviour
{
    [SyncVar] public float timeRemaining = 60;

    private void Update()
    {
        if (!isServer) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            EndGame();
        }
    }

    [Server]
    void EndGame()
    {
        int unFrozeCount=0;
        PlayerTag[] players = FindObjectsByType<PlayerTag>(FindObjectsSortMode.None);
        bool itPlayerWon = true;
        foreach(PlayerTag player in players)
        {
            if (!player.isFroze)
            {
                unFrozeCount++;
                if (unFrozeCount > 1)
                {
                    itPlayerWon = false;
                    break;
                }
            }
        }

        RPCShowWin(itPlayerWon);
    }

    [ClientRpc]
    void RPCShowWin(bool itPlayerWon)
    {
        GetComponent<TMP_Text>().text = itPlayerWon ? "It Player won!" : "Survivors Win!";
        if (itPlayerWon)
        {
            //it player wins
            Debug.Log("It Player won!");
        }
        else
        {
            //survivors win
            Debug.Log("Survivors Win!");
        }
    }
}
