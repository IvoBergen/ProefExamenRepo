using UnityEngine;

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

    public void SetJumping(bool isJumping)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetTrigger("isJumping");
    }
}
