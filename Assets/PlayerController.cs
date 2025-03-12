using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float spd=1;
    public Vector3 velocity;
    public float friction = 0.95f;

    Transform playerModel;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        playerModel = transform.GetChild(1);
        if (!isLocalPlayer)
        {
            //Destroy camera, leaving only the local player's camera
            Destroy(transform.GetChild(2).gameObject);
        }
    }

    private Vector3 prevPos;
    // Update is called once per frame
    void Update()
    {
        if (transform.position != prevPos)
        {
            Vector3 dir = transform.position - prevPos;
            dir.Normalize();
            playerModel.forward = dir;
        }
        prevPos = transform.position;

        if (!isLocalPlayer) return;

        Vector3 forwardMove = transform.forward*Input.GetAxis("Vertical");
        Vector3 horizontalMove = transform.right * Input.GetAxis("Horizontal");

        velocity += (forwardMove + horizontalMove) * spd;
        velocity *= friction;

        transform.position += velocity * Time.deltaTime;
    }
}
