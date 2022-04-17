using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Scripts.World
{
    public class FakeInfiniteWorld : MonoBehaviour
    {
        [SerializeField] private Transform player;
        private AsteroidGenerator asteroidGenerator;
        private float mapSize;
        // Post Processing
        //public GameObject PostProcessing;
        private void Awake()
        {
            asteroidGenerator = GetComponentInChildren<AsteroidGenerator>();
            mapSize = asteroidGenerator.spawnRange * 0.8f;
        }
        private void Update() => MoveToOtherEdge();

        // Yes, we're cheating.
        private void MoveToOtherEdge() => player.position = new Vector3(
            player.position.x > mapSize ? -mapSize : player.position.x < -mapSize ? mapSize : player.position.x,
            player.position.y > mapSize ? -mapSize : player.position.y < -mapSize ? mapSize : player.position.y,
            player.position.z > mapSize ? -mapSize : player.position.z < -mapSize ? mapSize : player.position.z
        );

        //private void PostProcess()
        //{
        //    var pp = PostProcessing.GetComponent<PostProcessVolume>();
        //    var ld = pp.profile.GetSetting<LensDistortion>();
        //    ld.intensity.value = Mathf.Lerp(ld.intensity, -80, Time.deltaTime);
        //}

    }
}