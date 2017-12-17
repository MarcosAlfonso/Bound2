using System;
using System.Linq;
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

    //Probability table for the number of rows for each type
    private List<float> prob_repeatRowType;

    //Probability table for the specific row types
    private Dictionary<Action, float> generationProbabilityTable;

    private int colorInc;

    // Use this for initialization
    void Awake()
    {
        lastParent = gameObject.transform;

        GenerateLevel();
    }

    void SetupGeneration()
    {
        //Weighted probability for how man identical row types in a row before switching
        prob_repeatRowType = new List<float> 
        {
            .05f, //1
            .1f,  //2
            .225f, //3
            .225f, //4
            .4f //5
        };

        //Weighted probability for the various generation algorithms
        generationProbabilityTable = new Dictionary<Action, float>();
        generationProbabilityTable.Add(GenerateFullColumns, .25f);
        generationProbabilityTable.Add(GenerateFullRails, .25f);
        generationProbabilityTable.Add(GenerateMiddleColumns, .25f);
        generationProbabilityTable.Add(GenerateMiddleRails, .25f);
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

    //Used to destroy all the existing platform groups
    private void DestroyAllChildren()
    {
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!Application.isPlaying)
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            else
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
        }
    }

    #region Generation Algorithms
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
            platGO.transform.parent = newParent;

            if (UnityEngine.Random.value < columnChance & !columnExists)
            {
                platGO.transform.localScale = new Vector3(platGO.transform.localScale.x, 1000, platGO.transform.localScale.z);
                columnExists = true;
            }

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor(getNextColor());

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
            platGO.transform.parent = newParent;

            var plat = platGO.GetComponent<Platform>();
            plat.initializeColor(getNextColor());

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
        platGO.transform.parent = newParent;

        var plat = platGO.GetComponent<Platform>();
        plat.initializeColor(getNextColor());

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
        platGO.transform.parent = newParent;        

        var plat = platGO.GetComponent<Platform>();
        plat.initializeColor(getNextColor());

        applyNextOffset(newParent);

    }
    #endregion


    //Gets the next color
    private Platform.PlatformColor getNextColor()
    {
        colorInc = colorInc + 1;
        return (Platform.PlatformColor)(colorInc % (Enum.GetValues(typeof(Platform.PlatformColor)).Length - 1));
    }

    //Offsets a row
    private void applyNextOffset(Transform newParent)
    {
        var rotationPerRow = 3;
        var offsetPerRow = 20;

        newParent.localPosition = lastParent.localPosition;
        newParent.localRotation = Quaternion.Euler(lastParent.localRotation.eulerAngles + new Vector3(0, rotationPerRow, 0));
        newParent.transform.Translate(new Vector3(0, 0, offsetPerRow));
        lastParent = newParent;
    }

    //Generats a level
    public void GenerateLevel()
    {
        DestroyAllChildren();

        SetupGeneration();

        //Reset "last parent" to the level generator so offsetting is reset
        lastParent = gameObject.transform;       

        //Column Placement
        var numRows = 100;

        int tillNextToggle = 0;
        int generationType = 0;
        for (int rowPlacement = 0; rowPlacement < numRows; rowPlacement++)
        {
            --tillNextToggle;
            if (tillNextToggle <= 0)
            {
                tillNextToggle = RandomFromDistribution.RandomChoiceFollowingDistribution(prob_repeatRowType) + 1;

                var potentialGenType = RandomFromDistribution.RandomChoiceFollowingDistribution(generationProbabilityTable.Values.ToList());
                if (potentialGenType == generationType)
                    generationType = (generationType + 1) % generationProbabilityTable.Keys.Count;
                else
                    generationType = potentialGenType;

            }

            generationProbabilityTable.Keys.ToList()[generationType]();

        }
    }
}
