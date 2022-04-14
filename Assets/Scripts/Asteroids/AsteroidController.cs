using System;
using UnityEngine;

namespace Scripts.Asteroids
{
    public class AsteroidController : MonoBehaviour
    {
        public int health = 100;
        public int damagePerShot = 30;
        public ParticleSystem explosionEffect;
        public AudioClip[] explosionSounds;


        public void TakeDamage()
        {
            if (gameObject == null) return;
            health -= damagePerShot;
            if (health > 0) return;
            Destroy(gameObject);
            
            var emptyGameObject = new GameObject().AddComponent<AudioSource>();
            emptyGameObject = Instantiate(emptyGameObject, transform.position, Quaternion.identity);
            
            var audioSource = emptyGameObject.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1;
            audioSource.volume = 1;
            audioSource.spread = 360;
            audioSource.PlayOneShot(explosionSounds[UnityEngine.Random.Range(0, explosionSounds.Length)]);
            
            Destroy(emptyGameObject, audioSource.clip.length + 1);
            
            var explosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            explosion.Emit(1);

        }

    }
}

