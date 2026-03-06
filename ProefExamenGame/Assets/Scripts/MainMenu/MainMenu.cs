using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _customizationMenu;
    [SerializeField] private GameObject _mainMenu;
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("starting game");
    }

    public void CustomizationMenuActivate()
    {
        _customizationMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void CustomizationMenuDeactivate()
    {
        _customizationMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.LogWarning("Quit Game!");
        Application.Quit();
    }
}
