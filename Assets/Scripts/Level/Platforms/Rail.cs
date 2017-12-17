using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : Platform {

    public BoxCollider attachmentZone;

    public override Platform.PlatformType getPlatformType()
    {
        return Platform.PlatformType.Rail;
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.player.attachedRail = this;

        var railYRotation = transform.parent.localRotation.eulerAngles.y;

        //Check which way we're facing to determine rail direction
        var forwardDiff = Mathf.Abs(Mathf.Abs(railYRotation) - Mathf.Abs(GameManager.Instance.player.horzRot));
        var backwardDiff = Mathf.Abs(Mathf.Abs(railYRotation - 180) - Mathf.Abs(GameManager.Instance.player.horzRot));
        
        if (forwardDiff < backwardDiff)
        {
            var xVel = Mathf.Sin(railYRotation * Mathf.Deg2Rad);
            var zVel = Mathf.Cos(railYRotation * Mathf.Deg2Rad);
            GameManager.Instance.player.railDirection = new Vector3(xVel, 0, zVel);
        }
        else
        {
            var xVel = Mathf.Sin((railYRotation + 180) * Mathf.Deg2Rad);
            var zVel = Mathf.Cos((railYRotation + 180) * Mathf.Deg2Rad);
            GameManager.Instance.player.railDirection = new Vector3(xVel, 0, zVel);
        }


        GameManager.Instance.player.scorePlatform(colColor);
        GameManager.Instance.player.ResetJumps();
        burnColor();
    }

    
}
