using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


    public class EnemyController : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject enemy;
        public int health;
        private int Counter = 0;
        public Text enemiesDestroyed;
        public Text Score;

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Destroy(enemy);
                enemiesDestroyed.text = $"Enemies destroyed: {Counter++}";//during game
                Score.text = $"Score: {Counter * 10}";
            }
        }
    }


