using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;
using System;

public class PlayerRacer : NetworkBehaviour
{
    [SyncVar] public bool isIt=false;
    [SyncVar] public bool isFroze = false;
    [SyncVar] public bool isFinished = false;
    [SyncVar] public float time;
    [SyncVar] public string playerName;
    [SyncVar] public bool winner;

    private bool playerDataUpdated = false;

    private TMP_Text nametag;

    private void Start()
    {
        nametag = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        if (isLocalPlayer)
        {
            name = GameManager.currLocalPlayer.username;
            playerName = name;
            nametag.text = name;
            CmdTellName(name);
        }

        //if (!isServer) return;
        
    }

    private void Update()
    {
        if (nametag.text == "[username]" && playerName != "")
        {
            nametag.text = playerName;
        }

        if (!isLocalPlayer || !isFinished || playerDataUpdated) return;
        Debug.Log("Local player end: " + GameManager.currLocalPlayer.username);
        PlayerData data = GameManager.currLocalPlayer;
        data.gamesplayed++;
        if (time < data.besttime) data.besttime = time;
        if(winner) data.wincount++;
        PlayerDataPost.inst.UpdatePlayer(data, null);
        playerDataUpdated = true;
    }

    [Server]
    public void FinishRace(bool first)
    {
        GameTimer timer = FindObjectOfType<GameTimer>();
        time = timer.time;
        isFinished = true;
        winner = first;

        PlayerRacer[] racers = FindObjectsByType<PlayerRacer>(FindObjectsSortMode.None).OrderBy((p)=>p.time).Reverse().ToArray();
        if (AllRacersFinished(racers))
        {
            //int finalRank = EndGameAndGetRank(racers);
            //RPCEndState(finalRank);
            Debug.Log("game over!");
        }
        //Debug.Log(racers[0].name + ", " + playerName + ", "+racers.Length);
        RPCUpdateState(time, first);
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
        winner = firstFinish;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Renderer>().material.color = (firstFinish)?Color.yellow:Color.green;
        transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text+=": "+finaltime;

        /*
        isFroze = frozen;
        GetComponent<Renderer>().material.color = isFroze ? Color.cyan : Color.grey;
        GetComponent<PlayerController>().enabled = !isFroze;
        */
    }


    [Command]
    void CmdTellName(string name)
    {
        playerName = name;
        transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = playerName;
    }
}
