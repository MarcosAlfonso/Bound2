using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour {

    public GameObject columnPrefab;
    public GameObject railPrefab;

    public bool forceRegen;

    public List<Platform> platformList = new List<Platform>();

    private int colorIncrementer = 0;

	// Use this for initialization
	void Awake () {
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

        GenerateLevel();
	}
	
	// Update is called once per frame
	void Update () {
		if (forceRegen)
        {
            GenerateLevel();
            forceRegen = false;
        }
	}

    public class ColumnDef
    {
        public int num;
        public float spacing;
        public float heightOffset;
        public float heightVariance;
        public float columnChance;

        public ColumnDef(int num, float spacing, float heightOffset, float heightVariance, float columnChance)
        {
            this.num = num;
            this.spacing = spacing;
            this.heightOffset = heightOffset;
            this.heightVariance = heightVariance;
            this.columnChance = columnChance;
        }
    }

    public class RailDef
    {
        public int num;
        public float spacing;
        public float spacingVariance;
        public float heightOffset;
        public float heightVariance;

        public RailDef(int num, float spacing, float spacingVariance, float heightOffset, float heightVariance)
        {
            this.num = num;
            this.spacing = spacing;
            this.spacingVariance = spacingVariance;
            this.heightOffset = heightOffset;
            this.heightVariance = heightVariance;
        }
    }

    public void GenerateRow(ColumnDef rowDef, float zCalc)
    {
        bool columnExists = false;
        for (int index = 0; index < rowDef.num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = rowDef.spacing * (index - rowDef.num / 2.0f) + rowDef.spacing / 2.0f;
            yCalc = -rowDef.heightOffset + Random.value * rowDef.heightVariance - rowDef.heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

            var platGO = Instantiate(columnPrefab, platPos, Quaternion.identity);
            platGO.transform.SetParent(this.transform);

            if (Random.value < rowDef.columnChance & !columnExists)
            {
                platGO.transform.localScale = new Vector3(platGO.transform.localScale.x, 1000, platGO.transform.localScale.z);
                columnExists = true;
            }

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer%4));

            platformList.Add(plat);
            colorIncrementer++;
        }
    }

    public void GenerateRow(RailDef rowDef, float zCalc)
    {
        for (int index = 0; index < rowDef.num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = rowDef.spacing * (index - rowDef.num / 2.0f) + rowDef.spacing / 2.0f + (Random.value * rowDef.spacingVariance - rowDef.spacingVariance / 2.0f);
            yCalc = -rowDef.heightOffset + Random.value * rowDef.heightVariance - rowDef.heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

            var platGO = Instantiate(railPrefab, platPos, Quaternion.identity);
            platGO.transform.SetParent(this.transform);

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));


            platformList.Add(plat);
            colorIncrementer++;
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

        colorIncrementer = 0;

        //Column Placement
        var numRows = 100;
        var rowSpacing = 20;        

        bool railToggle = false;
        for (int rowPlacement = 0; rowPlacement < numRows; rowPlacement++)
        {
            if (rowPlacement != 0 && rowPlacement % 5 == 0)
            {
                railToggle = !railToggle;
            }

            if (railToggle)
            {
                GenerateRow(new RailDef(6, 10, 5, 0, 10), rowSpacing * rowPlacement);
            }
            else
            {
                var colHeightOffset = columnPrefab.transform.localScale.y / 2.0f;
                GenerateRow(new ColumnDef(7, 10, colHeightOffset, 10, .1f), rowSpacing * rowPlacement);
            }

        }
    }
}
