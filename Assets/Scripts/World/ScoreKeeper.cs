using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    public int score;
    public int enemiesKilled;
    public int currentWave;
    public Text enemiesKilledText;
    public Text endScore;
    [FormerlySerializedAs("WaveCounter")] public Text waveCounter;


    private void Update()
    {
        enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        endScore.text = "Score: " + score;
        waveCounter.text = $"Wave: {currentWave}";
    }
}