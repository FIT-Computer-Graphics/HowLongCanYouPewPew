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
            if (health <= 0) return;
            health -= damagePerShot;
            if (health > 0) return;
            Die();
        }

        private void Die()
        {
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
}