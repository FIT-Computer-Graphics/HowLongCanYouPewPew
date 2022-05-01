using Scripts.Scenes;
using UnityEngine;

namespace Scripts.Scenes
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject spaceShip;
        private bool Paused;
        private void Start()
        {
            CursorVisible(false);
            
        }
    
        private void Update()
        {
            if (spaceShip == null) return;
                
            
           //one escape pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Paused==true)
                {
                    Time.timeScale = 1.0f;
                    pauseMenu.gameObject.SetActive(false);
                    CursorVisible(false);
                    Paused = false;
                }
                else if(Paused==false)
                {
                    Time.timeScale = 0.0f;
                    pauseMenu.gameObject.SetActive(true);
                    CursorVisible(true);
                    Paused = true;
                }
             
            }
        }
    
        private  void CursorVisible(bool locked)//visibiliti of cursor
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible =locked;
        }
        public void Resume()//resume button
        {
            Time.timeScale = 1.0f;
            pauseMenu.gameObject.SetActive(false);
            CursorVisible(false);
            Paused = false;
        }
    
        public void ChangeSceneAfterPause()//Main menu button
        {
            Time.timeScale = 1.0f;
            SceneChanger.ChangeScene("MainMenu");
        }
    
  
    }
}
