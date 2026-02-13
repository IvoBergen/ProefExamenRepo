using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsTracker : MonoBehaviour
{
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
    void Update()
    {
        if (!_hasWon)
        {
            _currentTime += Time.deltaTime;
        }
    }
    /// <summary>
    /// Wins the game.
    /// </summary>
    public void win()
    {
        _hasWon = true;

        _deathText.text = "deathCount: " + _deaths.ToString();

        _currentTime = Mathf.FloorToInt(_currentTime % 60);
        _WinTime.text = "current time: " + _currentTime.ToString();
    }



}
