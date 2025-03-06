using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RoomPlayer : NetworkRoomPlayer
{
    public TMP_Text playerStatusTxt;

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateReadyStatus();
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        UpdateReadyStatus();
    }

    private void UpdateReadyStatus()
    {
        playerStatusTxt.text = readyToBegin ? "Is Ready" : "Is Not Ready";
    }
}
