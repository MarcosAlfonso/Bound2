using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Rigidbody myRigidBody;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody>();

	}

    void FixedUpdate()
    {
        myRigidBody.AddForce(Physics.gravity * myRigidBody.mass);
    }
}
