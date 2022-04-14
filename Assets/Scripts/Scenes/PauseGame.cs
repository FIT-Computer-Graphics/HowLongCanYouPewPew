using Scripts.Scenes;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    // Start is called before the first frame update

    private bool Paused;

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Time.timeScale = 1.0f;
                pauseMenu.gameObject.SetActive(false);
                LockCursor();
                Paused = false;
            }
            else
            {
                Time.timeScale = 0.0f;
                pauseMenu.gameObject.SetActive(true);
                Paused = true;
            }
        }
    }

    private static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseMenu.gameObject.SetActive(false);
    }

    public void ChangeSceneAfterPause()
    {
        Time.timeScale = 1.0f;
        SceneChanger.ChangeScene("MainMenu");
    }

    public void QuitGame() //radit ce samo kad se builda game
    {
        Application.Quit();
    }
}