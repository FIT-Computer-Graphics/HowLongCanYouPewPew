using System;
using Scripts.Asteroids;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scripts.PlayerController
{
    public class SpaceShipController : MonoBehaviour, IDamageable
    {
        private void Start()
        {
            screenCenter.x = Screen.width * .5f;
            screenCenter.y = Screen.height * .5f;
            maxHealth = playerHealth;
            SetHealthBar();
            shipAudioSource.loop = true;
            postProcessing = GameObject.Find("PostProcessing");
            profile = postProcessing.GetComponent<PostProcessVolume>();
            chromaticAberration = profile.profile.GetSetting<ChromaticAberration>();
        }

        private void Update()
        {
            CalculateMovement();
            ToggleFiring();
            CalculateShooting();
        }

        private void FixedUpdate()
        {
            TestRegen();
            SetHealthBarVisibility();
            SetAudioOnMoving();
            TestWarningSound();
            SetHealthBar();
        }

        public void TakeDamage(int damage)
        {
            lastDamageTime = Time.time;
            playerHealth -= damage;
            if (!(playerHealth <= 0)) return;
            GetComponent<PlayerDestruction>().Die();
        }

        private void TestWarningSound()
        {
            if (playerHealth < maxHealth * .25f && !warningSound.isPlaying)
                warningSound.Play();
            else if (playerHealth >= maxHealth * .25f && warningSound.isPlaying)
                warningSound.Stop();
            else
                switch (warningSound.isPlaying)
                {
                    case true:
                        chromaticAberration.intensity.value += Time.deltaTime * 3;
                        break;
                    case false:
                        chromaticAberration.intensity.value -= Time.deltaTime * 3;
                        break;
                }
        }

        private void SetAudioOnMoving()
        {
            if (Input.GetKey(KeyCode.W))
            {
                var volume = jetAudioSource.volume + Time.deltaTime * .08f;
                jetAudioSource.volume = Mathf.Clamp(volume, 0, 0.08f);
                if (!jetAudioSource.isPlaying) jetAudioSource.Play();
            }
            else
            {
                jetAudioSource.volume -= Time.deltaTime * .1f;
                if (jetAudioSource.volume <= 0) jetAudioSource.Stop();
            }
        }

        private void SetHealthBarVisibility()
        {
            if (Math.Abs(healthBar.value - 1f) < 0.01f)
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
            foreach (var particle in muzzleFlashes) particle.Emit(1);

            foreach (var origin in raycastOrigin)
            {
                ray.origin = origin.position;
                ray.direction = origin.forward;

                // If it hits, make the laser go there
                if (Physics.Raycast(ray, out hitInfo, 100f, LayerMask.GetMask("Damageable")))
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
            shipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            shipAudioSource.PlayOneShot(shootSound);
            shipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            shipAudioSource.PlayOneShot(shootSound);
        }

        private void HandleDamage(RaycastHit raycastHit)
        {
            raycastHit.transform.GetComponent<IDamageable>()?.TakeDamage(gunDamage);
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


            var transform1 = transform;
            transform1.position += activeForwardSpeed * Time.deltaTime * transform1.forward +
                                   activeHoverSpeed * Time.deltaTime * transform1.up;
        }

        private void SetHealthBar()
        {
            healthBar.value = playerHealth / maxHealth;
        }

        #region Variables

        // VFX
        private GameObject postProcessing;
        private ChromaticAberration chromaticAberration;
        private PostProcessVolume profile;

        // Damage
        public int gunDamage = 10;

        // Sounds
        [FormerlySerializedAs("ShipAudioSource")] public AudioSource shipAudioSource;
        [FormerlySerializedAs("JetAudioSource")] public AudioSource jetAudioSource;
        [FormerlySerializedAs("WarningSound")] public AudioSource warningSound;
        [FormerlySerializedAs("ShootSound")] public AudioClip shootSound;

        // Movement
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float hoverSpeed;
        private float activeForwardSpeed, activeHoverSpeed;
        private const float ForwardAcceleration = 2.5f;


        // Looking
        [SerializeField] private float lookRateSpeed = 90f;
        public Vector2 mouseDistance;
        private Vector2 lookInput, screenCenter;

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