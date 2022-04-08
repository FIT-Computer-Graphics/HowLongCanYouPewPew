using System;
using Scripts.PlayerController;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Destroying
{
    public class CollisionWithAsteroids : MonoBehaviour
    {

        [SerializeField] private GameObject spaceShip;
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject Effect;
        private float timeRemaining = 3f;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != spaceShip) return;
            Destroy(spaceShip);
            var explosion=Instantiate(Effect,Player.transform.position,Quaternion.identity);
            Destroy(explosion,1.5f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (!spaceShip)//ucita scenu nakon odredjenog vremena , mozemo dodat kasnije animaciju
            {
                Player.GetComponent<SpaceShipController>().enabled = false;
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    SceneManager.LoadScene("RestartMenu");
                }
            }
        }
    }
}


