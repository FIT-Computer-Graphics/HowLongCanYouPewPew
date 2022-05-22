using System.Collections;
using UnityEngine;

namespace Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject[] enemies;
        public Transform player;
        public ScoreKeeper score;
        public int amount ;
        private float timeLeft = 5;
        public AudioSource audioSource;

        private void Awake()
        {
            score = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        }

        private void Update()
        {
            if (GameObject.FindGameObjectWithTag(enemies[0].tag) == null) SpawnEnemies(amount, 40);
        }

        private void SpawnEnemies(int amountOfEnemies, float rangeAroundPlayer)
        {
            // Randomly spawn enemies around the player
            if (!(Timer() < 0)) return;
            audioSource.Play();
            
            // range can be -rangeAroundPlayer to +rangeAroundPlayer but not between -10 and 10
            var randomRange = Random.Range(-rangeAroundPlayer, rangeAroundPlayer);
            if (randomRange is > -40 and < 40) randomRange += 40;
            StartCoroutine(SpawnThem(amountOfEnemies, randomRange));

            amount += 1;
            timeLeft = 5;

            score.currentWave += 1;
        }

        private IEnumerator SpawnThem(int _amount, float randomRange)
        {
            for (var i = 0; i < _amount; i++)
            {
                var position = player.position;
                var spawnPosition = new Vector3(position.x + randomRange,
                    position.y - randomRange, position.z + randomRange);
                // Pick a random enemy
                Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPosition, Quaternion.identity);
                yield return new WaitForSeconds(0.4f);
            }
        }


        private float Timer()
        {
            timeLeft -= Time.deltaTime;
            return timeLeft;
        }
    }
}