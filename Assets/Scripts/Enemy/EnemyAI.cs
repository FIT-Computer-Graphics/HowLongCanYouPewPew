using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Scripts.Enemy;
using Scripts.PlayerController;
using UnityEngine.UI;

namespace Scripts.Enemy
{
    
    public class EnemyAI : MonoBehaviour
    {
        public List<Vector3> EscapeDirections = new List<Vector3>();

        //AI Checks
        public bool chasing;
        public bool evading;

        //AI stuff
        public float rotationSpeed;
        public float maxDist;
        public float minDist;
        public float fireDist;

        //AI Speeds
        public float moveSpeed;
        private Transform myTransform;
        private Vector3 newTargetPos;
        private Transform obstacle;
        private bool overrideTarget;
        private bool savePos;

        private Vector3 storeTarget;
        private Transform target;
        
        // Shooting
        
        public ParticleSystem[] muzzleFlashes;
        public Transform[] raycastOrigin;
        public ParticleSystem hitEffect;
        public TrailRenderer[] tracerEffects;
        
        public int fireRate = 10;
        private float accumulatedTime;
        private Ray ray;
        private RaycastHit hitInfo;
        private SpaceShipController player;
        
        // Sound
        
        public AudioSource ShipAudioSource;
        public AudioClip ShootSound;
        private int bulletsFired = 0;
        [SerializeField] private int enemyDamage = 8;
        
        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            myTransform = transform;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<SpaceShipController>();
            StartCoroutine(HeatDissipate());
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
                 if (bulletsFired > 0)
                 {
                     bulletsFired--;
                 }
             }
         }
         
        private void FireBullet()
        {
            bulletsFired++;
            // look at player with a random inaccuracy

            gameObject.transform.LookAt(target.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f)));
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
                case "Player":
                    player.TakeDamage(enemyDamage);
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
                if (distance < fireDist && distance > minDist)
                {
                    CalculateShooting();
                }

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

            if (EscapeDirections.Count > 0)
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
            EscapeDirections.Clear();
        }

        private Vector3 GetClosests()
        {
            var clos = EscapeDirections[0];
            var distance = Vector3.Distance(transform.position, EscapeDirections[0]);

            foreach (var dir in EscapeDirections)
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
            if (Physics.Raycast(col.transform.position, col.transform.up, out _, col.bounds.extents.y * 2 + 5))
            {
            }
            else
            {
                //if there is something above

                if (!EscapeDirections.Contains(direct1)) EscapeDirections.Add(direct1);
            }

            //Check for obstacles below
            if (Physics.Raycast(col.transform.position, -col.transform.up, out _, col.bounds.extents.y * 2 + 5))
            {
            }
            else
            {
                //if there is something below
                var dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2 - 5, 0);

                if (!EscapeDirections.Contains(dir)) EscapeDirections.Add(dir);
            }

            //Check for obstacles to the Right
            var direct2 = col.transform.position + new Vector3(col.bounds.extents.x * 2 + 5, 0, 0);
            if (Physics.Raycast(col.transform.position, col.transform.right, out _, col.bounds.extents.x * 2 + 5))
            {
            }
            else
            {
                //if there is something to the right

                if (!EscapeDirections.Contains(direct2)) EscapeDirections.Add(direct2);
            }

            //Check for obstacles to the Left
            if (Physics.Raycast(col.transform.position, -col.transform.right, out _, col.bounds.extents.x * 2 + 5))
            {
            }
            else
            {
                //if there is something to the right
                direct2 = col.transform.position + new Vector3(-col.bounds.extents.x * 2 - 5, 0, 0);

                if (!EscapeDirections.Contains(direct2)) EscapeDirections.Add(direct2);
            }
        }

        private RaycastHit[] Rays(Vector3 direction, float offsetX)
        {
            var position = transform.position;
            var ray = new Ray(position + new Vector3(offsetX, 0, 0), direction);
            Debug.DrawRay(position + new Vector3(offsetX, 0, 0), direction * (10 * moveSpeed), Color.red);

            var distanceToLookAhead = moveSpeed * 5;
            
            //Adjust 5 to proper radius around object to pick up raycast hits
            var results = new RaycastHit[] { };
            Physics.SphereCastNonAlloc(ray, 5, results, distanceToLookAhead);
            return results;
        }
    }
}