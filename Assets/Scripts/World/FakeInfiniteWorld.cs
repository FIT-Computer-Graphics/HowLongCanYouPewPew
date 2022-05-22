using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Scripts.World
{
    public class FakeInfiniteWorld : MonoBehaviour
    {
        [SerializeField] private Transform player;
        public GameObject PostProcessing;
        private AsteroidGenerator asteroidGenerator;
        private ChromaticAberration chromaticAberration;
        private LensDistortion lensDistortion;
        private float mapSize;
        private PostProcessVolume profile;

        private void Awake()
        {
            asteroidGenerator = GetComponentInChildren<AsteroidGenerator>();
            mapSize = asteroidGenerator.spawnRange * 0.8f;
            PostProcessing = GameObject.Find("PostProcessing");
            profile = PostProcessing.GetComponent<PostProcessVolume>();
            lensDistortion = profile.profile.GetSetting<LensDistortion>();
            chromaticAberration = profile.profile.GetSetting<ChromaticAberration>();
        }

        private void Update()
        {
            MoveToOtherEdge();
        }

        // Yes, we're cheating.
        private void MoveToOtherEdge()
        {
            var currentPosition = player.position;
            player.position = new Vector3(
                player.position.x > mapSize ? -mapSize : player.position.x < -mapSize ? mapSize : player.position.x,
                player.position.y > mapSize ? -mapSize : player.position.y < -mapSize ? mapSize : player.position.y,
                player.position.z > mapSize ? -mapSize : player.position.z < -mapSize ? mapSize : player.position.z
            );
            // if player moved more than 50 units
            if (Vector3.Distance(currentPosition, player.position) > 200f) StartCoroutine(DoPostProcessing());
        }

        public IEnumerator DoPostProcessing()
        {
            var LDintensity = lensDistortion.intensity.value;
            var CAintensity = chromaticAberration.intensity.value;
            var duration = 0.2f;
            var startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(CAintensity, 1f, (Time.time - startTime) / duration);
                lensDistortion.intensity.value = Mathf.Lerp(LDintensity, -50f, (Time.time - startTime) / duration);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            startTime = Time.time;
            duration = 0.7f;
            while (Time.time - startTime < duration)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1f, 0f, (Time.time - startTime) / duration);
                lensDistortion.intensity.value = Mathf.Lerp(-50f, 0f, (Time.time - startTime) / duration);
                yield return null;
            }
        }
    }
}