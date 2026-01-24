using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LVL_CristianTest");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
