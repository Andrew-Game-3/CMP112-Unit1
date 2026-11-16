using UnityEngine;

public class RestartGame : MonoBehaviour
{
   public void LoadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
