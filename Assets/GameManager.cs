using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static PlayerData currLocalPlayer;

    [SyncVar]
    public GameObject itPlayer;

    private void Start()
    {
        if (!isServer) return;
        itPlayer = NetworkServer.connections[0].identity.gameObject;
        //Set player to it
        //itPlayer.GetComponent<PlayerTag>().isIt = true;
        //SetItPlayerRPC();
    }

    [ClientRpc]
    void SetItPlayerRPC()
    {
        itPlayer = NetworkServer.connections[0].identity.gameObject;
        //Set player to it
        itPlayer.GetComponent<PlayerTag>().isIt = true;
    }

    public override void OnStartClient()
    {
        if (!isServer) return;
        Debug.Log(NetworkServer.connections.Count);
        Debug.Log(NetworkServer.connections[0]);
        if (NetworkServer.connections.Count == 1)
        {
            itPlayer = NetworkServer.connections[0].identity.gameObject;
            //Set player to it
            //itPlayer.GetComponent<PlayerTag>().isIt = true;
        }
    }
}
