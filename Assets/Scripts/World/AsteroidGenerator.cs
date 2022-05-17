using System.Collections.Generic;
using UnityEngine;

namespace Scripts.World
{
    public class AsteroidGenerator : MonoBehaviour
    {
        [SerializeField] public float spawnRange;
        [SerializeField] private float amountToSpawn;

        [SerializeField] private GameObject asteroid;

        [SerializeField] private float startSafeRange;
        [SerializeField] private bool randomizeRotations;
        [SerializeField] private float randomFloatingSpeed;

        private readonly List<GameObject> objectsToPlace = new();
        private Vector3 spawnPoint;

        // Start is called before the first frame update
        private void Start()
        {
            var asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
            SpawnAsteroids();
        }

        private void SpawnAsteroids()
        {
            for (var i = 0; i < amountToSpawn; i++)
            {
                PickSpawnPoint();

                //pick new spawn point if too close to player start
                while (Vector3.Distance(spawnPoint, Vector3.zero) < startSafeRange) PickSpawnPoint();

                objectsToPlace.Add(Instantiate(asteroid, spawnPoint,
                    Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))));

                if (randomizeRotations)
                    objectsToPlace[i].transform.Rotate(Random.Range(0f, 360f), Random.Range(0f, 360f),
                        Random.Range(0f, 360f));
                // Make them float randomly
                objectsToPlace[i].GetComponent<Rigidbody>().velocity = new Vector3(
                    Random.Range(-randomFloatingSpeed, randomFloatingSpeed),
                    Random.Range(-randomFloatingSpeed, randomFloatingSpeed),
                    Random.Range(-randomFloatingSpeed, randomFloatingSpeed));


                objectsToPlace[i].transform.parent = transform;
            }

            asteroid.SetActive(false);
        }


        private void PickSpawnPoint()
        {
            spawnPoint = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f));

            if (spawnPoint.magnitude > 1) spawnPoint.Normalize();

            spawnPoint *= spawnRange;
        }
    }
}