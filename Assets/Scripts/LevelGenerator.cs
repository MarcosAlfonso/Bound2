using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour {

    public GameObject columnPrefab;
    public GameObject railPrefab;

    public bool forceRegen;

    public List<Platform> platformList = new List<Platform>();

	// Use this for initialization
	void Awake () {
        GenerateLevel();

        foreach(var transform in GetComponentsInChildren<Transform>())
        {
            if (transform.gameObject != this.gameObject)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(transform.gameObject);
                else
                    Destroy(transform.gameObject);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (forceRegen)
        {
            GenerateLevel();
            forceRegen = false;
        }
	}

    public class RowDefinition
    {
        public Platform.PlatformType platftormType;
        public int num;
        public float spacing;
        public float spacingVariance;
        public float heightOffset;
        public float heightVariance;

        public RowDefinition(Platform.PlatformType platformType, int num, float spacing, float spacingVariance, float heightOffset, float heightVariance)
	    {
            this.platftormType = platformType;
            this.num = num;
            this.spacing = spacing;
            this.spacing = spacingVariance;
            this.heightOffset = heightOffset;
            this.heightVariance = heightVariance;
	    }
    }

    public void GenerateRow(RowDefinition rowDef, float zCalc)
    {
        for (int index = 0; index < rowDef.num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = rowDef.spacing * (index - rowDef.num / 2.0f) + rowDef.spacing / 2.0f + (Random.value * rowDef.spacingVariance - rowDef.spacingVariance / 2.0f);
            yCalc = -rowDef.heightOffset + Random.value * rowDef.heightVariance - rowDef.heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

            var prefab = rowDef.platftormType == Platform.PlatformType.Rail ? railPrefab : columnPrefab;
            var platGO = Instantiate(prefab, platPos, Quaternion.identity);
            platGO.transform.SetParent(this.transform);

            var plat = platGO.GetComponent<Platform>();
            plat.setToRandomColor();

            platformList.Add(plat);
        }
    }

    public void GenerateLevel()
    {
        //Clear out old columns
        foreach(var column in platformList)
        {

            if (column != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(column.gameObject);
                else
                    Destroy(column.gameObject);

            }
        }
        platformList.Clear();

        //Column Placement
        var numRows = 50;
        var rowSpacing = 20;        

        bool railToggle = false;
        for (int rowPlacement = 0; rowPlacement < numRows; rowPlacement++)
        {
            if (rowPlacement % 10 == 0)
                railToggle = !railToggle;

            if (railToggle)
            {
                GenerateRow(new RowDefinition(Platform.PlatformType.Rail, 6, 15, 5, 0, 10), rowSpacing * rowPlacement);
            }
            else
            {
                var colHeightOffset = columnPrefab.transform.localScale.y / 2.0f;
                GenerateRow(new RowDefinition(Platform.PlatformType.Column, 7, 10, 5, colHeightOffset, 10), rowSpacing * rowPlacement);
            }
        }
    }
}
