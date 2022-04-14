using UnityEngine;
using Scripts.Asteroids;
using Scripts.Enemy;
namespace Scripts.PlayerController
{
    public class SpaceShipController : MonoBehaviour
    {
        #region Variables
        
        // TESTTESTTEST
        public GameObject EnemySpawner;
        
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
        #endregion
        private void Start()
        {
            screenCenter.x = Screen.width * .5f;
            screenCenter.y = Screen.height * .5f;

        }

        private void Update()
        {
            CalculateMovement();
            ToggleFiring();
            CalculateShooting();
            
            // TESTTESTTEST
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EnemySpawner.GetComponent<EnemySpawner>().SpawnEnemies(10, 50);
            }
            
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
            ShipAudioSource.pitch = Random.Range(0.7f, 0.8f);
            ShipAudioSource.PlayOneShot(ShootSound);
        }

        private void HandleDamage(RaycastHit raycastHit)
        {
            Debug.Log($"Hit {raycastHit.transform.gameObject.tag}");
            switch (raycastHit.transform.gameObject.tag)
            {
                case "Asteroid":
                    hitInfo.transform.GetComponent<AsteroidController>().TakeDamage();
                    break;
                case "Enemy":
                    // Do stuff
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

            transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * RollSpeed * Time.deltaTime, Space.Self);

            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxis("Vertical") * forwardSpeed, ForwardAcceleration * Time.deltaTime);
            
            activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxis("Hover") * hoverSpeed, HoverAcceleration);

            var transform1 = transform;
            transform1.position += activeForwardSpeed * Time.deltaTime * transform1.forward +
                                   activeHoverSpeed * Time.deltaTime * transform1.up;
        }


      
    }
}
