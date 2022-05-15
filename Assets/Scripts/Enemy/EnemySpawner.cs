using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemy;
        public Transform player;
        private int amount = 50;
        private float timeLeft=5 ;
        public ScoreKeeper score;
        
        private void Awake()
        {
            score = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        }
        
        public void SpawnEnemies(int amount, float rangeAroundPlayer)
        {
            // Randomly spawn enemies around the player
            if (Timer()<0)
            {
                // range can be -rangeAroundPlayer to +rangeAroundPlayer but not between -10 and 10
                var randomRange = Random.Range(-rangeAroundPlayer, rangeAroundPlayer);
                if (randomRange is > -10 and < 10)
                {
                    randomRange += rangeAroundPlayer;
                }
                for (var i = 0; i < amount; i++)
                {
                    var position = player.position;
                    var spawnPosition = new Vector3(position.x + Random.Range(-rangeAroundPlayer, rangeAroundPlayer),
                        position.y, position.z + Random.Range(-rangeAroundPlayer, rangeAroundPlayer));
                    Instantiate(enemy, spawnPosition, Quaternion.identity);
                }
                this.amount = this.amount + 1;
                timeLeft = 5;
                
                score.currentWave += 1;
                
            }
        }

    
        private float Timer()
        {
            timeLeft -= Time.deltaTime;
            return timeLeft;
        }
        private void Update()
        {
            if (GameObject.FindGameObjectWithTag(enemy.tag)==null)
            {
                SpawnEnemies(amount,40);
            }
        }
    }
}