using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void OnPlayClicked()
        {
            SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        }

        public void OnQuitClicked()
        {
#if !UNITY_EDITOR
        Application.Quit(0);
#else
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
