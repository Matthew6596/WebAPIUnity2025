using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRacer : NetworkBehaviour
{
    [SyncVar] public bool isIt=false;
    [SyncVar] public bool isFroze = false;
    [SyncVar] public bool isFinished = false;
    [SyncVar] public float time;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return; //only server handles tagging

        PlayerTag otherPlayer = other.GetComponent<PlayerTag>();
        if (otherPlayer != null)
        {
            if(isIt && !otherPlayer.isFroze)
            {
                otherPlayer.FreezePlayer();
            }else if(!isIt && otherPlayer.isFroze && !otherPlayer.isIt)
            {
                otherPlayer.UnfreezePlayer();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return; //only server handles tagging

        PlayerTag otherPlayer = collision.gameObject.GetComponent<PlayerTag>();
        if (otherPlayer != null)
        {
            if (isIt && !otherPlayer.isFroze)
            {
                otherPlayer.FreezePlayer();
            }
            else if (!isIt && otherPlayer.isFroze && !otherPlayer.isIt)
            {
                otherPlayer.UnfreezePlayer();
            }
        }
    }

    [Server]
    public void FreezePlayer()
    {
        isFroze = true;
        //RPCUpdateState(isFroze);
    }
    [Server]
    public void UnfreezePlayer()
    {
        isFroze = false;
        //RPCUpdateState(isFroze);
    }

    [Server]
    public void FinishRace()
    {
        GameTimer timer = FindObjectOfType<GameTimer>();
        time = timer.time;
        isFinished = true;
        RPCUpdateState(time);
    }

    [ClientRpc]
    void RPCUpdateState(float finaltime)
    {
        time = finaltime;
        isFinished = true;
        GetComponent<Renderer>().material.color = Color.green;

        /*
        isFroze = frozen;
        GetComponent<Renderer>().material.color = isFroze ? Color.cyan : Color.grey;
        GetComponent<PlayerController>().enabled = !isFroze;
        */
    }
}
