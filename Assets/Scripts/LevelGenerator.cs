using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{

    public GameObject columnPrefab;
    public GameObject railPrefab;

    public bool forceRegen;

    public List<Platform> platformList = new List<Platform>();

    private int colorIncrementer = 0;

    // Use this for initialization
    void Awake()
    {
        foreach (var transform in GetComponentsInChildren<Transform>())
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
    void Update()
    {
        if (forceRegen)
        {
            GenerateLevel();
            forceRegen = false;
        }
    }


    public void GenerateFullColumns(float zCalc)
    {

        int num = 7;
        float spacing = 10;
        float heightOffset = 125;
        float heightVariance = 10;
        float columnChance = .1f;

        bool columnExists = false;
        for (int index = 0; index < num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = spacing * (index - num / 2.0f) + spacing / 2.0f;
            yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

            var platGO = Instantiate(columnPrefab, platPos, Quaternion.identity);
            platGO.transform.SetParent(this.transform);

            if (UnityEngine.Random.value < columnChance & !columnExists)
            {
                platGO.transform.localScale = new Vector3(platGO.transform.localScale.x, 1000, platGO.transform.localScale.z);
                columnExists = true;
            }

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));

            platformList.Add(plat);
            colorIncrementer++;
        }
    }

    public void GenerateFullRails(float zCalc)
    {
        int num = 6;
        float spacing = 10;
        float spacingVariance = 5;
        float heightOffset = 0;
        float heightVariance = 10;

        for (int index = 0; index < num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = spacing * (index - num / 2.0f) + spacing / 2.0f + (UnityEngine.Random.value * spacingVariance - spacingVariance / 2.0f);
            yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

            var platGO = Instantiate(railPrefab, platPos, Quaternion.identity);
            platGO.transform.SetParent(this.transform);

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));


            platformList.Add(plat);
            colorIncrementer++;
        }
    }

    public void GenerateMiddleColumn(float zCalc)
    {
        float xVariance = 50;
        float heightOffset = 125;
        float heightVariance = 5;

        float xCalc;
        float yCalc;

        //Time to handle per row/type generation more elegantly
        xCalc = UnityEngine.Random.value * xVariance - xVariance / 2.0f;
        yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

        var platPos = transform.position + new Vector3(xCalc, yCalc, zCalc);

        var platGO = Instantiate(columnPrefab, platPos, Quaternion.identity);
        platGO.transform.SetParent(this.transform);

        var plat = platGO.GetComponent<Platform>();
        plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));

        platformList.Add(plat);
        colorIncrementer++;
    }

    //Probability table for the number of rows for each type
    private List<float> prob_countPerRowType;

    //Probability table for the specific row types
    private List<float> prob_generationType;

    const int ROW_SPACING = 20;

    public List<Action<float>> RowGenerationFuncs;

    public void GenerateLevel()
    {
        prob_countPerRowType = new List<float> 
        {
            .05f, //1
            .1f,  //2
            .225f, //3
            .225f, //4
            .4f //5
        };

        prob_generationType = new List<float> 
        {
            .3f,
            .3f,
            12.3f
         };

        RowGenerationFuncs = new List<Action<float>>();
        RowGenerationFuncs.Add(GenerateFullColumns);
        RowGenerationFuncs.Add(GenerateFullRails);
        RowGenerationFuncs.Add(GenerateMiddleColumn);

        //Clear out old columns
        foreach (var column in platformList)
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

        int tillNextToggle = 4;
        int generationType = 0;
        for (int rowPlacement = 0; rowPlacement < numRows; rowPlacement++)
        {
            --tillNextToggle;
            if (tillNextToggle <= 0)
            {
                tillNextToggle = RandomFromDistribution.RandomChoiceFollowingDistribution(prob_countPerRowType) + 1;

                var potentialGenType = RandomFromDistribution.RandomChoiceFollowingDistribution(prob_generationType);
                if (potentialGenType == generationType)
                    generationType = (generationType + 1) % RowGenerationFuncs.Count;
                else
                    generationType = potentialGenType;

            }

            RowGenerationFuncs[generationType](ROW_SPACING * rowPlacement);

        }
    }
}
