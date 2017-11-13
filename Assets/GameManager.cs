using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

    public LevelGenerator levelGenerator;
    public TextMeshProUGUI scoreText;

    public static GameManager Instance;

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateScoreText(int score, int combo)
    {
        scoreText.text = "Score: " + score + "\nCombo: " + combo;
    }
}
