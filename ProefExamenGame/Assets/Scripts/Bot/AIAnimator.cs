using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    /// <summary>
    /// handels the animations of the AI 
    /// </summary>
    [Header("Animation")]
    [SerializeField] private Animator _animator;

    public void SetBool(string paramName, bool value)
    {
        if (_animator != null)
            _animator.SetBool(paramName, value);
    }

    public void SetTrigger(string paramName)
    {
        if (_animator != null)
            _animator.SetTrigger(paramName);
    }
}