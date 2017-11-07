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
        var colPerRow = 8;
        var colSpacing = 10;

        var colHeightOffset = columnPrefab.transform.localScale.y / 2.0f;
        var heightVariance = 5; 

        var numRows = 50;
        var rowSpacing = 20;

        bool railToggle = false;
        for (int rowPlacement = 0; rowPlacement < numRows; rowPlacement++)
        {
            if (rowPlacement % 10 == 0)
                railToggle = !railToggle;

            for (int widthPlacement = 0; widthPlacement < colPerRow; widthPlacement++)
            {
                float xCalc;
                float yCalc;
                float zCalc;

                //Time to handle per row/type generation more elegantly
                if (true)
                {
                    xCalc = colSpacing * (widthPlacement - colPerRow / 2.0f) + colSpacing / 2.0f;
                    yCalc = -colHeightOffset + Random.value * heightVariance - heightVariance / 2.0f;
                    zCalc = rowSpacing * rowPlacement;
                }
                
                var colPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

                var prefab = railToggle ? railPrefab : columnPrefab;
                var colGO = Instantiate(prefab, colPos, Quaternion.identity);
                colGO.transform.SetParent(this.transform);

                var col = colGO.GetComponent<Platform>();
                col.setToRandomColor();

                platformList.Add(col);
            }


        }
    }
}
