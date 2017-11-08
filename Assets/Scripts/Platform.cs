using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public enum PlatformType
    {
        Column,
        Rail
    }
    public PlatformType colType;

    public enum PlatformColor
    {
        Green,
        Red,
        Blue,
        Yellow,
        Grey
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
        //orange
        new Color(1, .6f, 0),
        //grey (touched)
        new Color(.4f, .4f, .4f)
    };

	// Use this for initialization
	void Awake () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setToRandomColor()
    {
        colColor = (PlatformColor)Random.Range(0, 4);

        var tempMaterial = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
        tempMaterial.color = ColumnColors[(int)colColor];
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
    }
}
