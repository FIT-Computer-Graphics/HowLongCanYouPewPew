using System.Collections;
using System.Collections.Generic;
using Scripts.Asteroids;
using Scripts.PlayerController;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [FormerlySerializedAs("EscapeDirections")] public List<Vector3> escapeDirections = new();

        //AI Checks
        public bool chasing;

        //AI stuff
        public float rotationSpeed;
        public float maxDist;
        public float minDist;
        public float fireDist;

        //AI Speeds
        public float moveSpeed;

        // Shooting

        public ParticleSystem[] muzzleFlashes;
        public Transform[] raycastOrigin;
        public ParticleSystem hitEffect;
        public TrailRenderer[] tracerEffects;
        public float fireRandomness = 2f;
        public int fireRate = 10;
        public ParticleSystem shotEffect;

        // Sound

        [FormerlySerializedAs("ShipAudioSource")] public AudioSource shipAudioSource;
        [FormerlySerializedAs("ShootSound")] public AudioClip shootSound;
        [SerializeField] private int enemyDamage = 8;
        public AudioClip[] hitSoundEffects;
        private float accumulatedTime;
        private int bulletsFired;
        private RaycastHit hitInfo;
        private Transform myTransform;
        private Vector3 newTargetPos;
        private Transform obstacle;
        private bool overrideTarget;
        private SpaceShipController player;
        private AudioSource playerHitEffectSource;
        private Ray ray;
        private bool savePos;

        private Vector3 storeTarget;
        private Transform target;

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            myTransform = transform;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<SpaceShipController>();
            // It's an object called HitEffectSource thats is a child of target
            playerHitEffectSource = target.Find("HitEffectSource").GetComponent<AudioSource>();
            StartCoroutine(HeatDissipate());
        }


        private void FixedUpdate()
        {
            //Find Distance to target
            var distance = (target.position - myTransform.position).magnitude;

            if (chasing)
            {
                //Rotate to look at player
                var position1 = myTransform.position;
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                    Quaternion.LookRotation(target.position - position1), rotationSpeed * Time.deltaTime);
                //Move towards the player
                position1 += myTransform.forward * (moveSpeed * Time.deltaTime);
                myTransform.position = position1;

                //Moves too far away from player
                if (distance > maxDist) chasing = false;

                //Attack if close enough
                if (distance < fireDist && distance > minDist) CalculateShooting();

                if (distance < minDist)
                {
                    var position = myTransform.position;
                    myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                        Quaternion.LookRotation(position - target.position), rotationSpeed * Time.deltaTime);
                    position += myTransform.forward * (moveSpeed * Time.deltaTime);
                    myTransform.position = position;
                }
            }
            else
            {
                if (distance < maxDist) chasing = true;
            }

            ObstacleAvoidance(transform.forward, 0);
        }

        private void CalculateShooting()
        {
            if (bulletsFired >= fireRate) return;

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime < 1.0f / fireRate) return;
            accumulatedTime = 0;

            FireBullet();
        }

        private IEnumerator HeatDissipate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (bulletsFired > 0) bulletsFired--;
            }
        }

        private void FireBullet()
        {
            bulletsFired++;
            
            // Inaccuracy
            gameObject.transform.LookAt(target.transform.position +
                                        new Vector3(Random.Range(-fireRandomness, fireRandomness),
                                            Random.Range(-fireRandomness, fireRandomness),
                                            Random.Range(-fireRandomness, fireRandomness)));
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
                    
                    SpawnSmoke(hitInfo);
                    
                    // Play a random hit sound once
                    playerHitEffectSource.PlayOneShot(hitSoundEffects[Random.Range(0, hitSoundEffects.Length)]);
                    
                    SpawnTracers();

                    HandleDamage(hitInfo);
                }
                // If it doesn't, just make it go forward I guess idk
                else SpawnTracers(true);
            }
            
            shipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            shipAudioSource.PlayOneShot(shootSound);
            shipAudioSource.pitch = Random.Range(0.7f, 0.95f);
            shipAudioSource.PlayOneShot(shootSound);
        }

        private void SpawnSmoke(RaycastHit raycastHit)
        {
            // 10% of spawning smoke
            if (Random.Range(0, 101) > 10) return;
            var effect = Instantiate(shotEffect, raycastHit.point, Quaternion.identity);
            var transform1 = effect.transform;
            transform1.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            transform1.parent = target.transform;
            Destroy(effect, 15f);
        }

        private void HandleDamage(RaycastHit raycastHit)
        {
            raycastHit.transform.GetComponent<IDamageable>()?.TakeDamage(enemyDamage);
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

        private void ObstacleAvoidance(Vector3 direction, float offsetX)
        {
            var hit = Rays(direction, offsetX);

            for (var i = 0; i < hit.Length - 1; i++)
            {
                //So we don't detect ourself as a hit collision
                if (hit[i].transform.root.gameObject == gameObject) continue;
                if (!savePos)
                {
                    storeTarget = target.position;
                    obstacle = hit[i].transform;
                    savePos = true;
                }

                FindEscapeDirections(hit[i].collider);
            }

            if (escapeDirections.Count > 0)
                if (!overrideTarget)
                {
                    newTargetPos = GetClosests();
                    overrideTarget = true;
                }

            var distance = Vector3.Distance(transform.position, target.position);

            if (!(distance < 5)) return;
            if (savePos)
            {
                //if we reach the target
                target.position = storeTarget;
                savePos = false;
            }

            overrideTarget = false;
            escapeDirections.Clear();
        }

        private Vector3 GetClosests()
        {
            var clos = escapeDirections[0];
            var distance = Vector3.Distance(transform.position, escapeDirections[0]);

            foreach (var dir in escapeDirections)
            {
                var tempDistance = Vector3.Distance(transform.position, dir);
                if (!(tempDistance < distance)) continue;
                distance = tempDistance;
                clos = dir;
            }

            return clos;
        }

        private void FindEscapeDirections(Collider col)
        {
            //Check for obstacles above
            var direct1 = col.transform.position + new Vector3(0, col.bounds.extents.y * 2 + 5, 0);
            if (!Physics.Raycast(col.transform.position, col.transform.up, out _, col.bounds.extents.y * 2 + 5))
            {
                //if there is something above

                if (!escapeDirections.Contains(direct1)) escapeDirections.Add(direct1);
            }

            //Check for obstacles below
            if (!Physics.Raycast(col.transform.position, -col.transform.up, out _, col.bounds.extents.y * 2 + 5))
            {
                //if there is something below
                var dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2 - 5, 0);

                if (!escapeDirections.Contains(dir)) escapeDirections.Add(dir);
            }
            //Check for obstacles to the Right
            var direct2 = col.transform.position + new Vector3(col.bounds.extents.x * 2 + 5, 0, 0);
            if (!Physics.Raycast(col.transform.position, col.transform.right, out _, col.bounds.extents.x * 2 + 5))
            {
                //if there is something to the right

                if (!escapeDirections.Contains(direct2)) escapeDirections.Add(direct2);
            }
            //Check for obstacles to the Left
            if (!Physics.Raycast(col.transform.position, -col.transform.right, out _, col.bounds.extents.x * 2 + 5))
            {
                //if there is something to the right
                direct2 = col.transform.position + new Vector3(-col.bounds.extents.x * 2 - 5, 0, 0);

                if (!escapeDirections.Contains(direct2)) escapeDirections.Add(direct2);
            }
        }

        private RaycastHit[] Rays(Vector3 direction, float offsetX)
        {
            var position = transform.position;
            var ray = new Ray(position + new Vector3(offsetX, 0, 0), direction);
            var distanceToLookAhead = moveSpeed * 5;
            var results = new RaycastHit[] { };
            Physics.SphereCastNonAlloc(ray, 5, results, distanceToLookAhead);
            return results;
        }
    }
}