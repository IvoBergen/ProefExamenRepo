using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <c>MainMenu</c> Controls the behavior of the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _customizationMenu;
    [SerializeField] private GameObject _mainMenu;

    /// <summary>
    /// <c>StartGame</c> Switches the game 
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("starting game");
    }

    /// <summary>
    /// <c>CustomizationMenuActivate</c> Open the character customization menu
    /// </summary>
    public void CustomizationMenuActivate()
    {
        _customizationMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    /// <summary>
    /// <c>CustomizationMenuDeactivate</c> Closes the character customization menu
    /// </summary>
    public void CustomizationMenuDeactivate()
    {
        _customizationMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    /// <summary>
    /// <c>ExitGame</c> Exits the applications 
    /// </summary>
    public void ExitGame()
    {
        Debug.LogWarning("Quit Game!");
        Application.Quit();
    }
}
