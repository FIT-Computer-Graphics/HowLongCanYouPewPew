using System;
using Scripts.Asteroids;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts.PlayerController
{
    public class SpaceShipController : MonoBehaviour
    {
        private void Start()
        {
            screenCenter.x = Screen.width * .5f;
            screenCenter.y = Screen.height * .5f;
            maxHealth = playerHealth;
            SetHealthBar();
        }

        private void Update()
        {
            CalculateMovement();
            ToggleFiring();
            SetHealthBar();
            CalculateShooting();
            TestRegen();
            SetHealthBarVisibility();

            // TESTTESTTEST
            //if (Input.GetKeyDown(KeyCode.Space)) EnemySpawner.GetComponent<EnemySpawner>().SpawnEnemies(10, 50);
        }

        private void SetHealthBarVisibility()
        {
            if (Math.Abs(healthBar.value - 1f) < 0.2f)
            {
                if (healthBarTimer == 0) healthBarTimer = Time.time;
                if (Time.time - healthBarTimer > 5) healthBar.gameObject.SetActive(false);
            }
            else
            {
                healthBarTimer = 0;
                if (!healthBar.IsActive()) healthBar.gameObject.SetActive(true);
            }
        }

        private void TestRegen()
        {
            // regen if last damage was more than 10 seconds ago
            isRegenerating = Time.time - lastDamageTime > 5;
            if (!isRegenerating || !(playerHealth < maxHealth)) return;
            playerHealth += Time.deltaTime * 0.50f * playerHealth;
            if (playerHealth > maxHealth) playerHealth = maxHealth;
        }

        private void ToggleFiring()
        {
            if (Input.GetButtonDown("Fire1")) isFiring = true;
            if (Input.GetButtonUp("Fire1")) isFiring = false;
        }

        private void CalculateShooting()
        {
            if (!isFiring) return;

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime < 1.0f / fireRate) return;
            accumulatedTime = 0;

            FireBullet();
        }


        private void FireBullet()
        {
            // Muzzle Flash Stuff
            foreach (var particle in muzzleFlashes) particle.Emit(1);

            // Pew Pew
            foreach (var origin in raycastOrigin)
            {
                ray.origin = origin.position;
                ray.direction = origin.forward;

                // If it hits, make the laser go there
                if (Physics.Raycast(ray, out hitInfo, 100f))
                {
                    var transform1 = hitEffect.transform;
                    transform1.position = hitInfo.point;
                    transform1.forward = hitInfo.normal;
                    hitEffect.Emit(1);

                    SpawnTracers();
                    HandleDamage(hitInfo);
                }
                // If it doesn't, just make it go forward I guess idk
                else
                {
                    SpawnTracers(true);
                }
            }

            // Play the sound
            ShipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            ShipAudioSource.PlayOneShot(ShootSound);
            ShipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            ShipAudioSource.PlayOneShot(ShootSound);
        }

        private void HandleDamage(RaycastHit raycastHit)
        {
            switch (raycastHit.transform.gameObject.tag)
            {
                case "Asteroid":
                    hitInfo.transform.GetComponent<AsteroidController>().TakeDamage();
                    break;
                case "Enemy":
                    hitInfo.transform.GetComponent<EnemyController>().TakeDamage(gunDamage);
                    break;
            }
        }

        private void SpawnTracers(bool infinite = false)
        {
            foreach (var tracerEffect in tracerEffects)
            {
                var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
                tracer.AddPosition(ray.origin);
                if (infinite) tracer.AddPosition(ray.GetPoint(100f));
                else tracer.transform.position = hitInfo.point;
            }
        }


        private void CalculateMovement()
        {
            lookInput.x = Input.mousePosition.x;
            lookInput.y = Input.mousePosition.y;

            mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
            mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
            mouseDistance = Vector2.ClampMagnitude(mouseDistance, 0.8f);

            rollInput = Mathf.Lerp(rollInput, -Input.GetAxis("Horizontal"), RollAcceleration * Time.deltaTime);

            transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime,
                mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * RollSpeed * Time.deltaTime, Space.Self);

            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxis("Vertical") * forwardSpeed,
                ForwardAcceleration * Time.deltaTime);

            activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxis("Hover") * hoverSpeed, HoverAcceleration);

            var transform1 = transform;
            transform1.position += activeForwardSpeed * Time.deltaTime * transform1.forward +
                                   activeHoverSpeed * Time.deltaTime * transform1.up;
        }

        private void SetHealthBar()
        {
            healthBar.value = playerHealth / maxHealth;
        }

        public void TakeDamage(int damage)
        {
            lastDamageTime = Time.time;
            playerHealth -= damage;
            if (!(playerHealth <= 0)) return;
            var playerDestruction = GetComponent<PlayerDestruction>();
            playerDestruction.Die();
        }

        #region Variables

        // Damage
        public int gunDamage = 10;

        // Sounds
        public AudioSource ShipAudioSource;
        public AudioClip ShootSound;

        // Movement
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float hoverSpeed;
        private float activeForwardSpeed, activeHoverSpeed;
        private const float ForwardAcceleration = 2.5f;
        private const float HoverAcceleration = 2;

        // Looking
        [SerializeField] private float lookRateSpeed = 90f;
        private Vector2 lookInput, screenCenter, mouseDistance;

        private float rollInput;
        private const float RollSpeed = 90f;
        private const float RollAcceleration = 3.5f;

        // Shooting
        public ParticleSystem[] muzzleFlashes;
        public Transform[] raycastOrigin;
        public ParticleSystem hitEffect;
        public TrailRenderer[] tracerEffects;

        private bool isFiring;
        public int fireRate = 25;
        private float accumulatedTime;
        private Ray ray;
        private RaycastHit hitInfo;

        [SerializeField] private Slider healthBar;
        // Player Health

        [SerializeField] private float playerHealth;
        private float maxHealth;
        private bool isRegenerating;
        private float lastDamageTime;
        private float healthBarTimer;

        #endregion
    }
}