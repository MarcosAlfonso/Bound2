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

    public Vector3? railDirection = null;
    public Rail attachedRail;

    private int jumpsRemaining = -1;

    private Vector3 startPosition;

    private GameObject projectile;

    private int score;
    private int combo;
    private int currency;
    private Platform.PlatformColor lastColor = Platform.PlatformColor.Touched;

	// Use this for initialization
	void Awake() {
        startPosition = transform.position;
        ResetJumps();
	}
	
	// Update is called once per frame
	void Update () {
        HandleLookControl();

        if (Input.GetKeyDown(KeyCode.S))
        {
            ResetJumps();
            isGoing = true;
        }

        if (isGoing)
        {

            if (railDirection == null)
            {
                var xVel = Mathf.Sin(horzRot * Mathf.Deg2Rad);
                var zVel = Mathf.Cos(horzRot * Mathf.Deg2Rad);
                playerBody.velocity = new Vector3(xVel * baseMovementSpeed, playerBody.velocity.y, zVel * baseMovementSpeed);
            }
            else
            {
                playerBody.velocity = railDirection.Value * baseMovementSpeed;

                if (!attachedRail.attachmentZone.bounds.Contains(transform.position))
                {
                    attachedRail = null;
                    railDirection = null;
                }
            }


            if (Input.GetMouseButtonDown(0) && jumpsRemaining > 0)
            {
                //This clears a potential constraint by a rail
                railDirection = null;
                attachedRail = null;

                //Kill player velocity first
                playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);

                //Apply jump force
                playerBody.AddForce(new Vector3(0, jumpPower, 0));

                jumpsRemaining--;
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
            else if (attemptPowerup(1))
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

    void Kill()
    {
        GameManager.Instance.levelGenerator.GenerateLevel();
        transform.position = startPosition;
        playerBody.velocity = Vector3.zero;
        isGoing = false;

        score = 0;
        combo = 0;
        currency = 0;

        GameManager.Instance.UpdateScoreText(score, combo);
        GameManager.Instance.UpdateCurrencyText(currency);

    }

    public void scorePlatform(Platform.PlatformColor color)
    {
        if (color == lastColor)
        {
            combo++;

            if (combo % 5 == 0)
                currency++;
        }
        else
        {
            combo = 1;
        }

        lastColor = color;

        score += combo;
        GameManager.Instance.UpdateScoreText(score, combo);
        GameManager.Instance.UpdateCurrencyText(currency);
    }

    public void ResetJumps()
    {
        jumpsRemaining = jumpCount;
    }

    public bool attemptPowerup(int cost)
    {
        if (currency - cost >= 0)
        {
            currency -= cost;
            return true;
        }

        return false;
    }
}
