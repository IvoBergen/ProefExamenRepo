using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// <c>PauseMenu</c> Controls the behavior of the pause menu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private void Start()
    {
        _pauseMenu.SetActive(false);
    }

    /// <summary>
    /// <c>StartPauseMenu</c> Opens the pause menu and stops the game with the timescale
    /// </summary>
    public void StartPauseMenu()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// <c>ResumeGame</c> Resumes the game and raises the time.scale to unpause the game
    /// </summary>
    public void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// <c>ReturnToMainMenu</c>
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// <c>ExitGame</c> Exits the application 
    /// </summary>
    public void ExitGame()
    {
        Debug.LogWarning("Quitting the game");
        Application.Quit();
    }
}
