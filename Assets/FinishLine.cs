using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        PlayerRacer racer = other.gameObject.GetComponent<PlayerRacer>();
        if (racer == null) return;

        if (!racer.isFinished)
        {
            racer.FinishRace();
        }
    }
}
