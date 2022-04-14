using UnityEngine;

namespace Scripts.Asteroids
{
    public class AsteroidController : MonoBehaviour
    {
        public int health = 100;
        public int damagePerShot = 30;
        public ParticleSystem explosionEffect;
        
        public void TakeDamage()
        {
            health -= damagePerShot;
            if (health > 0) return;
            Destroy(gameObject);
            var explosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            explosion.Emit(1);
        }



    }
}

