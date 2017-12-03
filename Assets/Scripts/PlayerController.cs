using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour{

    public Rigidbody playerBody;
    public Camera playerCamera;

    public GameObject projectilePrefab;

    //Look Control Vars
    private float mouseSensitivity = 100.0f;
    private float clampAngle = 90.0f;
    public float horzRot = 0.0f; // rotation around the up/y axis
    public float vertRot = 0.0f; // rotation around the right/x axis

    private float baseMovementSpeed = 12f;
    private float projectileSpeed = 25f;
    private const int jumpCount = 2;
    private int jumpPower = 340;

    private bool isGoing;

    private int railDirection = 0;
    private Platform attachedRail;

    private int jumpsRemaining = -1;

    private Vector3 startPosition;

    private GameObject projectile;

    private int score;
    private int combo;
    private Platform.PlatformColor lastColor = Platform.PlatformColor.Touched;

	// Use this for initialization
	void Awake() {
        startPosition = transform.position;
        jumpsRemaining = jumpCount;
	}
	
	// Update is called once per frame
	void Update () {
        HandleLookControl();

        if (Input.GetKeyDown(KeyCode.S))
        {
            jumpsRemaining = jumpCount;
            isGoing = true;
        }

        if (isGoing)
        {

            if (railDirection == 0)
            {
                var xVel = Mathf.Sin(horzRot * Mathf.Deg2Rad);
                var zVel = Mathf.Cos(horzRot * Mathf.Deg2Rad);
                playerBody.velocity = new Vector3(xVel * baseMovementSpeed, playerBody.velocity.y, zVel * baseMovementSpeed);
            }
            else
            {
                playerBody.velocity = new Vector3(0, 0, railDirection * baseMovementSpeed);
            }


            if (Input.GetMouseButtonDown(0) && jumpsRemaining > 0)
            {
                //This clears a potential constraint by a rail
                railDirection = 0;

                //Kill player velocity first
                playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);

                //Apply jump force
                playerBody.AddForce(new Vector3(0, jumpPower, 0));

                jumpsRemaining--;
            }

            if (attachedRail != null)
            {
                if (transform.position.z > attachedRail.GetComponent<BoxCollider>().bounds.max.z ||
                    transform.position.z < attachedRail.GetComponent<BoxCollider>().bounds.min.z)
                {
                    attachedRail = null;
                    railDirection = 0;
                }
            }


           

        }
        //Check for fall death
        if (transform.position.y < -10f || Input.GetKeyDown(KeyCode.R))
            Kill();

        //Right click projectile
        if (Input.GetMouseButtonDown(1))
        {
            //Projectile exists, teleport to it
            if (projectile != null)
            {
                transform.DOMove(projectile.transform.position + new Vector3(0, .5f, 0), .1f);
                Destroy(projectile);
                projectile = null;
            }
            //spawn projectile
            else
            {
                var vertMod = -vertRot;
                if (vertMod > 0)
                {
                    vertMod *= 2;
                }

                var xVel = Mathf.Sin(horzRot * Mathf.Deg2Rad);
                var yVel = Mathf.Sin(vertMod * Mathf.Deg2Rad);
                var zVel = Mathf.Cos(horzRot * Mathf.Deg2Rad);


                var projectileVel = new Vector3(xVel, yVel, zVel);

                projectileVel.Normalize();

                projectile = Instantiate(projectilePrefab, transform.position + projectileVel * 1.5f, Quaternion.identity);                 

                projectile.GetComponent<Rigidbody>().velocity = projectileVel * projectileSpeed;                    
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);
	}

    private void HandleLookControl()
    {
        //Escape unlocks cursor
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        //Get mouse deltas
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        
        //apply as scaled rotations
        horzRot += mouseX * (mouseSensitivity * Time.deltaTime);

        if (horzRot > 180)
            horzRot = -180 - (180 - horzRot);
        else if (horzRot < -180)
            horzRot = 180 + (180 + horzRot);
        
        vertRot += mouseY * mouseSensitivity * Time.deltaTime;

        //Clamp x rotation
        vertRot = Mathf.Clamp(vertRot, -clampAngle, clampAngle);
        Quaternion localRotation = Quaternion.Euler(vertRot, horzRot, 0.0f);
        playerCamera.transform.localRotation = localRotation;
    }

    void OnCollisionEnter(Collision collision)
    {

        var platform = collision.collider.GetComponent<Platform>();

        if (platform != null)
        {

            //Column collision
            if (platform.colType == Platform.PlatformType.Column)
            {
                //Collision normal needs to be up/down or it wont refresh jump
                if (collision.contacts[0].normal == Vector3.up || collision.contacts[0].normal == Vector3.down)
                {
                    updateScore(platform.colColor);
                    platform.burnColor();
                    jumpsRemaining = 2;
                }                
            }
            //Rail Collision
            if (platform.colType == Platform.PlatformType.Rail)
            {

                attachedRail = platform;

                //Check which way we're facing to determine rail direction
                if (horzRot > -90 && horzRot < 90)
                {
                    railDirection = 1;
                }
                else
                {
                    railDirection = -1;
                }

                jumpsRemaining = 2;
                updateScore(platform.colColor);
                platform.burnColor();
                
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("End collision!");

        var platform = collision.collider.GetComponent<Platform>();
    }

    void Kill()
    {
        GameManager.Instance.levelGenerator.GenerateLevel();
        transform.position = startPosition;
        playerBody.velocity = Vector3.zero;
        isGoing = false;

    }

    void updateScore(Platform.PlatformColor color)
    {
        if (color == lastColor)
        {
            combo++;
        }
        else
        {
            combo = 1;
        }

        lastColor = color;

        score += combo;
        GameManager.Instance.UpdateScoreText(score, combo);
    }
}
