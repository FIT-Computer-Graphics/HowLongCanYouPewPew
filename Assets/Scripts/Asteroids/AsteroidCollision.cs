using System.Linq;
using Scripts.PlayerController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Scripts.Asteroids
{
    public class AsteroidCollision : MonoBehaviour
    {
        [SerializeField] private GameObject spaceShip;

        [FormerlySerializedAs("Player")] [SerializeField]
        private GameObject player;

        [FormerlySerializedAs("Effect")] [SerializeField]
        private GameObject effect;

        private float timeRemaining = 3f;

        private void Update()
        {
            if (spaceShip) return;
            player.GetComponent<SpaceShipController>().enabled = false;

            if (timeRemaining > 0) timeRemaining -= Time.deltaTime;
            else SceneManager.LoadScene("RestartMenu");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.tag);
            if (collision.gameObject != spaceShip) return;
            Destroy(spaceShip);
            var explosion = Instantiate(effect, player.transform.position, Quaternion.identity);
            Destroy(explosion, 1.5f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}