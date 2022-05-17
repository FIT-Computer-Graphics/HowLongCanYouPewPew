using UnityEngine;

namespace Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject[] enemies;
        public Transform player;
        public ScoreKeeper score;
        public int amount = 50;
        private float timeLeft = 5;

        private void Awake()
        {
            score = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        }

        private void Update()
        {
            if (GameObject.FindGameObjectWithTag(enemies[0].tag) == null) SpawnEnemies(amount, 40);
        }

        private void SpawnEnemies(int _amount, float rangeAroundPlayer)
        {
            // Randomly spawn enemies around the player
            if (!(Timer() < 0)) return;
            
            // range can be -rangeAroundPlayer to +rangeAroundPlayer but not between -10 and 10
            var randomRange = Random.Range(-rangeAroundPlayer, rangeAroundPlayer);
            if (randomRange is > -10 and < 10) randomRange += rangeAroundPlayer;
            for (var i = 0; i < _amount; i++)
            {
                var position = player.position;
                var spawnPosition = new Vector3(position.x + Random.Range(-rangeAroundPlayer, rangeAroundPlayer),
                    position.y, position.z + Random.Range(-rangeAroundPlayer, rangeAroundPlayer));
                // Pick a random enemy
                Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPosition, Quaternion.identity);
            }

            amount += 1;
            timeLeft = 5;

            score.currentWave += 1;
        }


        private float Timer()
        {
            timeLeft -= Time.deltaTime;
            return timeLeft;
        }
    }
}