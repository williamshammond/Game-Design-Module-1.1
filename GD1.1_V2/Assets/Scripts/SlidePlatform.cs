using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : MonoBehaviour
{
    //Indicates direction that the plate is moving
    //0 = left 1 = right
    private int direction;
    private GameObject platform;

    //Serialized fields
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        platform = this.gameObject;
        player = GameObject.Find("Player");
        moveLeft();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 pos = platform.transform.position;

        if (direction == 0)
        {
            platform.transform.position = new Vector3(pos.x - 0.05f, pos.y, pos.z);
        }
        else
        {
            platform.transform.position = new Vector3(pos.x + 0.05f, pos.y, pos.z);
        }
    }

    private void moveLeft()
    {
        direction = 0;
        Invoke("moveRight", 4f);
    }

    private void moveRight()
    {
        direction = 1;
        Invoke("moveLeft", 4f);
    }

    private void OnCollisionStay(Collision collision)
    {
        //Check that the player is above the platform to avoid the head getting stuck bug
        if (collision.gameObject == player & player.transform.position.y > gameObject.transform.position.y)
        {
            player.transform.position = new Vector3(transform.position.x, player.transform.position.y, 0);
        }
    }
}
