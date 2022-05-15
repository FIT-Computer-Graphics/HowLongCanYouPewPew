using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    public int score = 0;
    public int enemiesKilled = 0;
    public int currentWave = 0;
    public Text enemiesKilledText;
    public Text endScore;
    public Text WaveCounter;
    

    private void Update()
    {
        enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        endScore.text = "Score: " + score;
        WaveCounter.text = $"Wave: {currentWave}";
    }
}
