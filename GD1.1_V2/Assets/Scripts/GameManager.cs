using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject camera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject checkPoint;
    //UI Elements
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject replayButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject powerUpText; 
    [SerializeField] GameObject winText;
    [SerializeField] GameObject checkPointText;
    
    

    [SerializeField] Material red;

    public int currentCheckPoint;
    public Vector3[] checkPoints;
    private Rigidbody playerBody;
    

    // Start is called before the first frame update
    void Start()
    {
        playerBody = player.GetComponent<Rigidbody>();
        resetGame();

        //Set up list of checkpoints for respawn method
        checkPoints = new Vector3[4];
        checkPoints[0] = new Vector3(47.5f, 3, 0);
        checkPoints[1] = new Vector3(147.5f, 4.5f, 0);
        checkPoints[2] = new Vector3(260f, 0.5f, 0);
        checkPoints[3] = new Vector3(353, 4.5f, 0);
        //checkPoints[4] = new Vector3(376, 20f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //The camera stops following the player at the very end of the game
        if (player.transform.position.x < 384.11f)
        {
            camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+2, camera.transform.position.z);
        }
        

        
        
    }

    private void FixedUpdate()
    {
        if (player.transform.position.y < -10)
        {
            spawnAgain();
        }

        //Check for winning x position
        if (player.transform.position.x > 386)
        {
            endGame();
        }


        

    }


    //Reset game resets as if starting from fresh and displays the start button
    public void resetGame()
    {
        //Set Most UI elements to false and some to true
        startButton.SetActive(true);
        replayButton.SetActive(false);
        winText.SetActive(false);
        powerUpText.SetActive(false);
        checkPointText.SetActive(false);

        //Set back to default checkpoint and freeze player
        currentCheckPoint = -1; 
        playerBody.constraints = RigidbodyConstraints.FreezeAll;

        //Original Spawn
        player.transform.position = new Vector3(-3, 3, 0);
        //Set camera two units above player
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, camera.transform.position.z);
        player.GetComponent<Player>().powered = false;

        //Get each of the necessary child objects of the player using
        //the transform and the GetChild method
        GameObject head = player.transform.GetChild(0).gameObject;
        GameObject hand1 = player.transform.GetChild(1).gameObject;
        GameObject hand2 = player.transform.GetChild(2).gameObject;
        GameObject ear1 = player.transform.GetChild(3).gameObject;
        GameObject ear2 = player.transform.GetChild(4).gameObject;

        //Set the material of each new object to the material red
        head.GetComponent<MeshRenderer>().material = red;
        hand1.GetComponent<MeshRenderer>().material = red;
        hand2.GetComponent<MeshRenderer>().material = red;
        ear1.GetComponent<MeshRenderer>().material = red;
        ear2.GetComponent<MeshRenderer>().material = red;

    }


    //startGame begins the game and starts playing
    public void startGame()
    {
        replayButton.SetActive(false);
        startButton.SetActive(false);
        //Original Spawn
        player.transform.position = new Vector3(-3, 3, 0);

        //Generate checkpoints
        for(int i = 0; i < checkPoints.Length; i++)
        {
            Instantiate(checkPoint, checkPoints[i], Quaternion.identity);
        }
        

        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, camera.transform.position.z);
        playerBody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionX & ~RigidbodyConstraints.FreezePositionY;
    }

    public void spawnAgain()
    {
        //currentCheckPoint == -1 means original spawn
        if (currentCheckPoint == -1)
        {
            player.transform.position = new Vector3(-3, 3, 0);
        }
        else
        {
            //Otherwise, use currentCheckPoint to index checkPoints
            player.transform.position = checkPoints[currentCheckPoint];
        }
     
        
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, camera.transform.position.z);
        playerBody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionX & ~RigidbodyConstraints.FreezePositionY;
    }





    private void endGame()
    {
        playerBody.constraints = RigidbodyConstraints.FreezeAll;
        winText.SetActive(true);
        replayButton.SetActive(true);
        //playerBody.velocity = Vector3.zero;
        //playerBody.useGravity = false;
    }
    
}
