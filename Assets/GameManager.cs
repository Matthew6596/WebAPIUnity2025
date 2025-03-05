using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public GameObject itPlayer;

    public override void OnStartServer()
    {
        return;
        Debug.Log(NetworkServer.connections.Count);
        Debug.Log(NetworkServer.connections[0]);
        Debug.Log(NetworkServer.connections.Keys);
        if (NetworkServer.connections.Count == 1)
        {
            itPlayer = NetworkServer.connections[0].identity.gameObject;
            //Set player to it
            itPlayer.GetComponent<PlayerTag>().isIt = true;
        }
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
            itPlayer.GetComponent<PlayerTag>().isIt = true;
        }
    }
}
