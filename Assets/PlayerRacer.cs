using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;

public class PlayerRacer : NetworkBehaviour
{
    [SyncVar] public bool isIt=false;
    [SyncVar] public bool isFroze = false;
    [SyncVar] public bool isFinished = false;
    [SyncVar] public float time;

    private void Awake()
    {
        if (isLocalPlayer)
        {
            name = GameManager.currLocalPlayer.username;
        }
    }

    private void Start()
    {
        if (!isServer) return;
        SendNames();
    }

    [Server]
    void SendNames()
    {
        RPCInformName(name);
    }

    [Server]
    public void FinishRace()
    {
        GameTimer timer = FindObjectOfType<GameTimer>();
        time = timer.time;
        isFinished = true;

        PlayerRacer[] racers = FindObjectsByType<PlayerRacer>(FindObjectsSortMode.None).OrderBy((p)=>p.time).Reverse().ToArray();
        if (AllRacersFinished(racers))
        {
            //int finalRank = EndGameAndGetRank(racers);
            //RPCEndState(finalRank);
            Debug.Log("game over!");
        }
        //Debug.Log(racers[0].name + ", " + playerName + ", "+racers.Length);
        RPCUpdateState(time, name == racers[0].name);
        //Debug.Log("Player finished race: " + playerName + ", " + time);
    }

    [Server]
    private bool AllRacersFinished(PlayerRacer[] players)
    {
        foreach (PlayerRacer player in players)
        {
            if (!player.isFinished) return false;
        }
        return true;
    }

    [Server]
    private int EndGameAndGetRank(PlayerRacer[] racers)
    {
        for(int i=0; i<racers.Length; i++)
        {
            racers[i].RPCEndState(i);
        }
        return 0;
    }

    [ClientRpc]
    void RPCEndState(int finalRank)
    {
        GetComponent<PlayerController>().enabled = false;

        if (finalRank == 0)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    [ClientRpc]
    void RPCUpdateState(float finaltime,bool firstFinish)
    {
        time = finaltime;
        isFinished = true;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Renderer>().material.color = (firstFinish)?Color.yellow:Color.green;
        transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text+=": "+finaltime;

        /*
        isFroze = frozen;
        GetComponent<Renderer>().material.color = isFroze ? Color.cyan : Color.grey;
        GetComponent<PlayerController>().enabled = !isFroze;
        */
    }

    [ClientRpc]
    void RPCInformName(string name)
    {
        transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = name;
    }
}
