using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Scenes
{
    
    public class SceneChanger : MonoBehaviour
    {
        
        public static void ChangeScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
        public void QuitGame() //quit game works only in build
        {
            Application.Quit();
        }
    }
    
}