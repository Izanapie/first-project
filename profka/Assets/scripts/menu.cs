using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public string gamescene = "SampleScene";

    public void playgame()
    {
        Time.timeScale = 1f; // на всякий случай
        SceneManager.LoadScene(gamescene);
    }

    public void quitgame()
    {
        Application.Quit(); 
    }
}
