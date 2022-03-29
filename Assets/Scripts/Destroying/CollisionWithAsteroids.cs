using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Destroying
{
    public class CollisionWithAsteroids : MonoBehaviour
    {

        [SerializeField] private GameObject spaceShip;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != spaceShip) return;
            SceneManager.LoadScene("RestartMenu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}


