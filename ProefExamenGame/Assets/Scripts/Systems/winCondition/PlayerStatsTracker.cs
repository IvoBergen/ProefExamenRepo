using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsTracker : MonoBehaviour
{
    /// <summary>
    /// Keeps track of the stats of the player  
    /// </summary>
    private int _deaths;
    private float _currentTime;
    private bool _hasWon = false;
    [SerializeField] private Text _deathText;
    [SerializeField] private Text _WinTime;

    /// <summary>
    /// Adds one death to deaths
    /// </summary>
    public void died()
    {
        _deaths += 1;
    }
    /// <summary>
    /// keeps track of time.
    /// </summary>
    void Update()
    {
        if (!_hasWon)
        {
            _currentTime += Time.deltaTime;
        }
        else { Time.timeScale = 0f; }
    }
    /// <summary>
    /// Wins the game. activates Winscreen and shows deaths and time it took to complete 
    /// </summary>
    public void win()
    {
        _hasWon = true;

        _deathText.text = "deathCount: " + _deaths.ToString();

        _currentTime = Mathf.FloorToInt(_currentTime % 60);
        _WinTime.text = "Completed In: " + _currentTime.ToString();
        {
            switch (_currentTime)
            {
                case 0:
                    Debug.Log("minder dan 10");
                    break;

                case 1:
                    Debug.Log("minder dan 30");
                    break;

                case 2:
                    Debug.Log("120");
                    break;

                default:
                    break;
            }
        }
    }



}
