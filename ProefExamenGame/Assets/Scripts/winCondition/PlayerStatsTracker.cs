using TMPro;
using UnityEngine;

/// <summary>
/// Keeps track of the stats of the player  
/// </summary>
public class PlayerStatsTracker : MonoBehaviour
{
    private int _deaths;
    private float _currentTime;
    private bool _hasEnded = false;

    [Header("Win Screen Text")]
    [SerializeField] private TMP_Text _winDeathText;
    [SerializeField] private TMP_Text _winTimeText;

    [Header("Lose Screen Text")]
    [SerializeField] private TMP_Text _loseDeathText;
    [SerializeField] private TMP_Text _loseTimeText;

    /// <summary>
    /// Adds one death to the counter
    /// </summary>
    public void Died()
    {
        _deaths += 1;
    }

    void Update()
    {
        if (!_hasEnded)
        {
            _currentTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Call when player wins
    /// </summary>
    public void Win()
    {
        if (_hasEnded) return;

        _hasEnded = true;

        int displayTime = Mathf.FloorToInt(_currentTime);

        if (_winDeathText != null)
            _winDeathText.text = _deaths.ToString();

        if (_winTimeText != null)
            _winTimeText.text = displayTime.ToString();
    }

    /// <summary>
    /// Call when player loses
    /// </summary>
    public void Lose()
    {
        if (_hasEnded) return;

        _hasEnded = true;

        int displayTime = Mathf.FloorToInt(_currentTime);

        if (_loseDeathText != null)
            _loseDeathText.text = _deaths.ToString();

        if (_loseTimeText != null)
            _loseTimeText.text = displayTime.ToString();
    }
}