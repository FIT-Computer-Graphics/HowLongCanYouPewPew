using System.Collections.Generic;
using UnityEngine;

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

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            myTransform = transform;
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
                if (distance < fireDist)
                {
                    //insert attack commands here when ready
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