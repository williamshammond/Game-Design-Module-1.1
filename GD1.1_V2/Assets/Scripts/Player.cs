using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Instance variables
    private float horizontalMovement;
    private bool jumped;
    public bool powered;
    private bool canDash;
    private bool dashingRight;
    private bool dashingLeft;

    private Rigidbody playerBody;
    

    //Serialized Instance Variables
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] LayerMask notPlayer;
    [SerializeField] LayerMask platforms;
    [SerializeField] GameObject gameManager;
    [SerializeField] Material purple; 
    [SerializeField] ParticleSystem powerUpParticles;
    [SerializeField] GameObject powerUpText;
    [SerializeField] GameObject checkPointText;


    // Start is called before the first frame update
    void Start()
    {
        powered = false;
        playerBody = this.gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");

        //Don't let the player's input effect the velocity
        if (!dashingLeft & !dashingRight)
        {
            //Don't let the player move side to side if on a platform
            if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f, platforms).Length == 0)
            {
                playerBody.velocity = new Vector3(horizontalMovement * 5, playerBody.velocity.y, 0);
            }

            else
            {
                playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
          
            }
        }

        //Allow the player to jump if colliding with at least one collider at its feet outside of its own mask
        if (Input.GetKeyDown(KeyCode.Space) && Physics.OverlapSphere(groundCheckTransform.position, 0.1f, notPlayer).Length != 0)
        {
            jumped = true;
        }

        //Check for horizontalMovement directon and change where its facing
        if(horizontalMovement < 0 & Physics.OverlapSphere(groundCheckTransform.position, 0.1f, platforms).Length == 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (horizontalMovement > 0 & Physics.OverlapSphere(groundCheckTransform.position, 0.1f, platforms).Length == 0) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        //Set canDash to true if touching ground for shift left or shift right in the air to do air dash
        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f, notPlayer).Length != 0)
        {
            canDash = true;
        }

        //Check for shift left or shift right in the air to do air dash
        //This means checking that the player is powered up (powered) and has reset their dash by touching the ground (canDash)
        //and that they are not currently touching the ground (overlapsphere) and that they are pressing left or right shift
        if (powered & canDash & Physics.OverlapSphere(groundCheckTransform.position, 0.1f, notPlayer).Length == 0 & (Input.GetKeyDown(KeyCode.LeftShift) | Input.GetKeyDown(KeyCode.RightShift)))
        {
            /*Use Input.GetKey instead of Input.GetKeyDown because GetKeyDown only returns
             * true on the first frame that a key is held down and the player should be
             * able to hold down the right or left arrow key before pressing shift*/
            if (Input.GetKey(KeyCode.RightArrow)){
                dashingRight = true;
                canDash = false;
                //Set the starting velocity of the dash in the right direction
                playerBody.velocity = new Vector3(10, playerBody.velocity.y, 0);
                //Invoke method in 1 second to stop dash
                //Create a particle effect behind the player
                Instantiate(powerUpParticles, transform.position, new Quaternion(0, -0.5f, 0, 1));
                Invoke("stopDash", 0.5f); 
            }
            else if (Input.GetKey(KeyCode.LeftArrow)){
                dashingLeft = true;
                canDash = false;
                //Set the starting velocity of the dash in the left direction
                playerBody.velocity = new Vector3(-10, playerBody.velocity.y, 0);
                //Create a particle effect behind the player
                Instantiate(powerUpParticles, transform.position, new Quaternion(0, 0.5f, 0, 1));
                //The stopdash method will stop the dash from happening in half a second
                Invoke("stopDash", 0.5f);
            }
        }
    }



    private void FixedUpdate()
    {
        //Check for jumped variable to jump and then set jumped to false until the player can touch the
        //ground and jump again
        if (jumped)
        {
            playerBody.AddForce(6 * Vector3.up, ForceMode.VelocityChange);
            jumped = false;
        }

        //Check for dashingRight or dashingLeft booleans set in the update() method
        if (dashingRight)
        { 
            //Gradually slow down the player from the dash
            playerBody.velocity = new Vector3(playerBody.velocity.x - 0.02f, playerBody.velocity.y, 0);
        }

        else if (dashingLeft)
        {
            //Gradually slow down the player from the dash
            playerBody.velocity = new Vector3(playerBody.velocity.x + 0.02f, playerBody.velocity.y, 0);
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            gameManager.GetComponent<GameManager>().spawnAgain();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PowerUp"){
            powerUp();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "CheckPoint")
        {
            Destroy(other.gameObject);
            //Increase the checkpoint variable from the gameManager script
            gameManager.GetComponent<GameManager>().currentCheckPoint += 1;
            checkPointText.SetActive(true);
            Invoke("hideCheckPointText", 2f);
        }
    }


    private void powerUp()
    {
        //Get each of the necessary child objects of the player using
        //the transform and the GetChild method
        GameObject head = gameObject.transform.GetChild(0).gameObject;
        GameObject hand1 = gameObject.transform.GetChild(1).gameObject;
        GameObject hand2 = gameObject.transform.GetChild(2).gameObject;
        GameObject ear1 = gameObject.transform.GetChild(3).gameObject;
        GameObject ear2 = gameObject.transform.GetChild(4).gameObject;

        //Set the material of each new object to the material purple
        head.GetComponent<MeshRenderer>().material = purple;
        hand1.GetComponent<MeshRenderer>().material = purple;
        hand2.GetComponent<MeshRenderer>().material = purple;
        ear1.GetComponent<MeshRenderer>().material = purple;
        ear2.GetComponent<MeshRenderer>().material = purple;

        Instantiate(powerUpParticles,new Vector3(133.75f,2,0),new Quaternion(-0.4f,0,0,1));
        powered = true;

        powerUpText.SetActive(true);

        //yield return new WaitForSeconds(5);
        Invoke("hidePowerUpText", 4f);
        //powerUpText.SetActive(false);
    }


    private void stopDash()
    {
        //Stop either dashing variable
        dashingLeft = false;
        dashingRight = false;
    }

    //Simple method to set checkPoint text inactive
    private void hideCheckPointText()
    {
        checkPointText.SetActive(false);
    }

    //Simple method to set powerUp text inactive
    private void hidePowerUpText()
    {
        powerUpText.SetActive(false);
    }
}
