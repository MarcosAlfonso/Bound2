using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{

    public GameObject columnPrefab;
    public GameObject railPrefab;

    private Transform lastParent;

    public bool forceRegen;

    public List<Platform> platformList = new List<Platform>();

    private int colorIncrementer = 0;

    // Use this for initialization
    void Awake()
    {
        lastParent = gameObject.transform;

        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!Application.isPlaying)
                    DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
                else
                    Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (forceRegen)
        {
            for (var i = 0; i < gameObject.transform.GetChildCount(); i++)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
                else
                    Destroy(gameObject.transform.GetChild(i).gameObject);
            }

            GenerateLevel();
            forceRegen = false;
        }
    }


    public void GenerateFullColumns()
    {

        int num = 7;
        float spacing = 10;
        float heightOffset = 125;
        float heightVariance = 10;
        float columnChance = .1f;

        bool columnExists = false;

        var newParent = new GameObject("Full Columns").GetComponent<Transform>();
        newParent.parent = gameObject.transform;

        for (int index = 0; index < num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = spacing * (index - num / 2.0f) + spacing / 2.0f;
            yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, 0);

            var platGO = Instantiate(columnPrefab, platPos, Quaternion.identity);

            if (UnityEngine.Random.value < columnChance & !columnExists)
            {
                platGO.transform.localScale = new Vector3(platGO.transform.localScale.x, 1000, platGO.transform.localScale.z);
                columnExists = true;
            }

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));

            platformList.Add(plat);
            colorIncrementer++;

            platGO.transform.parent = newParent;
        }

        applyNextOffset(newParent);

    }

    public void GenerateFullRails()
    {
        int num = 6;
        float spacing = 10;
        float spacingVariance = 5;
        float heightOffset = 0;
        float heightVariance = 10;

        var newParent = new GameObject("Full Rails").GetComponent<Transform>();
        newParent.parent = gameObject.transform;

        for (int index = 0; index < num; index++)
        {
            float xCalc;
            float yCalc;

            //Time to handle per row/type generation more elegantly
            xCalc = spacing * (index - num / 2.0f) + spacing / 2.0f + (UnityEngine.Random.value * spacingVariance - spacingVariance / 2.0f);
            yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

            var platPos = transform.position + new Vector3(xCalc, yCalc, 0);

            var platGO = Instantiate(railPrefab, platPos, Quaternion.identity);

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));


            platformList.Add(plat);
            colorIncrementer++;

            platGO.transform.parent = newParent;
        }

        applyNextOffset(newParent);


    }

    public void GenerateMiddleColumns()
    {
        float xVariance = 42;
        float heightOffset = 125;
        float heightVariance = 5;

        float xCalc;
        float yCalc;

        var newParent = new GameObject("Middle Column").GetComponent<Transform>();
        newParent.parent = gameObject.transform;

        //Time to handle per row/type generation more elegantly
        xCalc = UnityEngine.Random.value * xVariance - xVariance / 2.0f;
        yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

        var platPos = transform.position + new Vector3(xCalc, yCalc, 0);

        var platGO = Instantiate(columnPrefab, platPos, Quaternion.identity);
        platGO.transform.SetParent(this.transform);

        var plat = platGO.GetComponent<Platform>();
        plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));

        platformList.Add(plat);
        colorIncrementer++;

        platGO.transform.parent = newParent;

        applyNextOffset(newParent);

    }

    public void GenerateMiddleRails()
    {
        float spacingVariance = 5;
        float heightOffset = 0;
        float heightVariance = 11;

        float xCalc;
        float yCalc;

        var newParent = new GameObject("Middle Rail").GetComponent<Transform>();
        newParent.parent = gameObject.transform;

        //Time to handle per row/type generation more elegantly
        xCalc = (UnityEngine.Random.value * spacingVariance - spacingVariance / 2.0f);
        yCalc = -heightOffset + UnityEngine.Random.value * heightVariance - heightVariance / 2.0f;

        var platPos = transform.position + new Vector3(xCalc, yCalc, 0);

        var platGO = Instantiate(railPrefab, platPos, Quaternion.identity);
        platGO.transform.SetParent(this.transform);

        var plat = platGO.GetComponent<Platform>();
        plat.initializeColor((Platform.PlatformColor)(colorIncrementer % 4));

        platformList.Add(plat);
        colorIncrementer++;

        platGO.transform.parent = newParent;

        applyNextOffset(newParent);

    }

    public void applyNextOffset(Transform newParent)
    {
        var rotationPerRow = 3;
        var offsetPerRow = 20;

        newParent.localPosition = lastParent.localPosition;
        newParent.localRotation = Quaternion.Euler(lastParent.localRotation.eulerAngles + new Vector3(0, rotationPerRow, 0));
        newParent.transform.Translate(new Vector3(0, 0, offsetPerRow));
        lastParent = newParent;
    }

    //Probability table for the number of rows for each type
    private List<float> prob_countPerRowType;

    //Probability table for the specific row types
    private List<float> prob_generationType;

    const int ROW_SPACING = 20;

    public List<Action> RowGenerationFuncs;

    public void GenerateLevel()
    {
        lastParent = gameObject.transform;

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
            .25f,
            .25f,
            .25f,
            .25f
         };

        RowGenerationFuncs = new List<Action>();
        RowGenerationFuncs.Add(GenerateFullColumns);
        RowGenerationFuncs.Add(GenerateFullRails);
        RowGenerationFuncs.Add(GenerateMiddleColumns);
        RowGenerationFuncs.Add(GenerateMiddleRails);

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

            RowGenerationFuncs[generationType]();

        }
    }
}
