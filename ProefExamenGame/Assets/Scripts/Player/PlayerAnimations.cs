using UnityEngine;

/// <summary>
/// Controls player animations and applies them to the currently selected outfit.
/// </summary>
public class PlayerAnimations : MonoBehaviour
{
    [Header("Animators for each outfit")]
    [SerializeField] private Animator[] _animators;

    /// <summary>
    /// Returns the animator of the currently selected outfit
    /// </summary>
    private Animator CurrentAnimator
    {
        get
        {
            if (_animators.Length == 0)
                return null;

            int index = CharacterOutfitSwapper.SelectedOutfitIndex;

            if (index >= _animators.Length)
                return null;

            return _animators[index];
        }
    }

    private void SetBool(string parameter, bool value)
    {
        Animator animator = CurrentAnimator;

        if (animator == null)
            return;

        animator.SetBool(parameter, value);
    }

    public void SetWalking(bool isWalking)
    {
        SetBool("isWalking", isWalking);
    }

    public void SetIdle(bool isIdle)
    {
        SetBool("isIdle", isIdle);
    }

    public void SetJumping(bool isJumping)
    {
        SetBool("isJumping", isJumping);
    }

    public void SetDashing(bool isDashing)
    {
        SetBool("isDashing", isDashing);
    }

    public void SetFalling(bool isFalling)
    {
        SetBool("isFalling", isFalling);
    }

    public void SetGettingUp(bool isGettingUp)
    {
        SetBool("isGettingUp", isGettingUp);
    }

    public void SetDeath(bool isDead)
    {
        SetBool("isDead", isDead);
    }
}