using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : Platform {

    public override Platform.PlatformType getPlatformType()
    {
        return Platform.PlatformType.Column;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCollisionEnter(Collision collision)
    {
        //Collision normal needs to be up/down or it wont refresh jump
        if (collision.contacts[0].normal == Vector3.up || collision.contacts[0].normal == Vector3.down)
        {
            GameManager.Instance.player.scorePlatform(colColor);
            GameManager.Instance.player.ResetJumps();
            burnColor();
        }           
    }

    
}
