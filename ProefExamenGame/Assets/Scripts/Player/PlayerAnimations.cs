using UnityEngine;

/// <summary>
/// <c>PlayerAnimations</c> Controls player animations.
/// </summary>
public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isWalking", isWalking);
    }

    public void SetIdle(bool isIdle)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isIdle", isIdle);
    }

    public void SetJumping(bool isJumping)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isJumping", isJumping);
    }

    public void SetDashing(bool isDashing)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isDashing", isDashing);
    }

    public void SetFalling(bool isFalling)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isFalling", isFalling);
    }

    public void SetGettingUp(bool isGettingUp)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isGettingUp", isGettingUp);
    }

    public void SetDeath(bool isDead)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("isDead", isDead);
    }
}
