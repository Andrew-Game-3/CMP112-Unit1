using UnityEngine;

public class RestartGame : MonoBehaviour
{
   public void LoadCurrentScene()
    {
        //reloads our scene to le player do level again
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        //unfreezes time
        Time.timeScale = 1;
    }
}
