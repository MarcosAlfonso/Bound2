using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public enum PlatformType
    {
        Null,
        Column,
        Rail
    }

    public enum PlatformColor
    {
        Green,
        Red,
        Blue,
        Yellow,
        Touched
    }
    [HideInInspector]
    public PlatformColor colColor;

    private List<Color> ColumnColors = new List<Color>() 
    {
        //red
        new Color(.85f, .05f, .05f),
        //teal
        new Color(.1f, .6f, .9f),
        //green
        new Color(.05f, .8f, .05f),
        //yellow
        new Color(1, .6f, 0),
        //touched
        new Color(0f, 0f, 0f)
    };

	// Use this for initialization
	void Awake () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual PlatformType getPlatformType() { return PlatformType.Null; }

    public void initializeColor(PlatformColor platColor)
    {
        colColor = platColor;

        var tempMaterial = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
        tempMaterial.color = ColumnColors[(int)colColor];
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
    }

    public void burnColor()
    {
        colColor = PlatformColor.Touched;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = ColumnColors[(int)colColor];
    }
   


}
