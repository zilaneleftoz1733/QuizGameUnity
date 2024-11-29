using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FinalAssignment.Main
{

    public class MainMenu : MonoBehaviour
    {
        
        public void LoadGame(string sceneName)
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);  
        }

        public void ExitApp()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Debug.Log("Exit");
            Application.Quit();
#endif
        }
    }
}