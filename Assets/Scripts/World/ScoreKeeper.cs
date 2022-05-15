using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    public int score;
    public int enemiesKilled;
    public int currentWave;
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