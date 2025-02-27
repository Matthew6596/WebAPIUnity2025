using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float spd=5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 inp = new Vector3(Input.GetAxis("Horizontal"), 0,Input.GetAxis("Vertical")) * spd;
        transform.position += inp * Time.deltaTime;
    }
}
