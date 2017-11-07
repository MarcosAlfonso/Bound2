using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    public Rigidbody playerBody;
    public Camera playerCamera;

    //Look Control Vars
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    private float horzRot = 0.0f; // rotation around the up/y axis
    private float vertRot = 0.0f; // rotation around the right/x axis

	// Use this for initialization
	void Awake() {
		
	}
	
	// Update is called once per frame
	void Update () {
        HandleLookControl();

        var xVel = Mathf.Sin(horzRot * Mathf.Deg2Rad);
        var zVel = Mathf.Cos(horzRot * Mathf.Deg2Rad);
        playerBody.velocity = new Vector3(xVel *10f, playerBody.velocity.y, zVel * 10f);

        playerBody.maxAngularVelocity = 0;

        var jumpPower = 310;

        if (Input.GetMouseButtonDown(0))
        {
                playerBody.constraints = RigidbodyConstraints.None;


            playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);
            playerBody.AddForce(new Vector3(0, jumpPower, 0));
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
        horzRot += mouseX * mouseSensitivity * Time.deltaTime;
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
            if (platform.colType == Platform.ColumnType.Rail)
            {
                playerBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("End collision!");

        var platform = collision.collider.GetComponent<Platform>();

        if (platform != null)
        {
            if (platform.colType == Platform.ColumnType.Rail)
            {
                playerBody.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
