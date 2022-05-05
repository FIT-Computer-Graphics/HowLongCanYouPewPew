using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemy;
        public Transform player;
        private int amount = 1;
        private float timeLeft=5 ;
        public void SpawnEnemies(int amount, float rangeAroundPlayer)
        {
            // Randomly spawn enemies around the player
            if (Timer()<0)
            {
                for (var i = 0; i < amount; i++)
                {
                    var position = player.position;
                    var spawnPosition = new Vector3(position.x + Random.Range(-rangeAroundPlayer, rangeAroundPlayer),
                        position.y, position.z + Random.Range(-rangeAroundPlayer, rangeAroundPlayer));
                    Instantiate(enemy, spawnPosition, Quaternion.identity);
                }
                this.amount = this.amount + 1;
                timeLeft = 5;
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
                SpawnEnemies(amount,10);
            }
        }
    }
}