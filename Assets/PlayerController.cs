using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float spd=1;
    public Vector3 velocity;
    public float friction = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            //Destroy camera, leaving only the local player's camera
            Destroy(transform.GetChild(1).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 forwardMove = transform.forward*Input.GetAxis("Vertical");
        Vector3 horizontalMove = transform.right * Input.GetAxis("Horizontal");

        velocity += (forwardMove + horizontalMove) * spd;
        velocity *= friction;

        transform.position += velocity * Time.deltaTime;
    }
}
