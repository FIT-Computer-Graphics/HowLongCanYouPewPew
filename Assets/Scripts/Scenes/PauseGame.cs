using UnityEngine;

namespace Scripts.Scenes
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject spaceShip;
        private bool paused;

        private void Start()
        {
            CursorVisible(false);
        }

        private void Update()
        {
            if (spaceShip == null) return;


            //one escape pause
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            switch (paused)
            {
                case true:
                    Time.timeScale = 1.0f;
                    pauseMenu.gameObject.SetActive(false);
                    CursorVisible(false);
                    paused = false;
                    break;
                case false:
                    Time.timeScale = 0.0f;
                    pauseMenu.gameObject.SetActive(true);
                    CursorVisible(true);
                    paused = true;
                    break;
            }
        }

        private static void CursorVisible(bool locked) //visibility of cursor
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = locked;
        }

        public void Resume() //resume button
        {
            Time.timeScale = 1.0f;
            pauseMenu.gameObject.SetActive(false);
            CursorVisible(false);
            paused = false;
        }

        public void ChangeSceneAfterPause() //Main menu button
        {
            Time.timeScale = 1.0f;
            SceneChanger.ChangeScene("MainMenu");
        }
    }
}