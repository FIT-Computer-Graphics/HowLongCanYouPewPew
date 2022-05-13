using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class EnemyController : MonoBehaviour
    {
        // Start is called before the first frame update
        public int health;
        public int scoreValue = 1;
        public ScoreKeeper score;
        public ParticleSystem explosionEffect;
        public AudioClip[] explosionSounds;
        
        private void Awake()
        {
            score = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health > 0) return;
            
            score.score += scoreValue;
            score.enemiesKilled += 1;
            PlayAudio();
            PlayExplosionEffect();
            Destroy(gameObject);
        }
        
        private void PlayExplosionEffect()
        {
            var explosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            explosion.Emit(1);
        }

        private void PlayAudio()
        {
            var audioSource =
                Instantiate(new GameObject().AddComponent<AudioSource>(), transform.position, Quaternion.identity);
            audioSource.spatialBlend = 1;
            audioSource.volume = 1;
            audioSource.spread = 360;
            audioSource.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)]);
            Destroy(audioSource.gameObject, 5f);
        }
    }


