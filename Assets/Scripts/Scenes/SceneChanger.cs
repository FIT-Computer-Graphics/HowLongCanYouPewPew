using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Scenes
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(string scene) => SceneManager.LoadScene(scene);
    }
}

