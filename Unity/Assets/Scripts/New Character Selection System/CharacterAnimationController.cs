using UnityEngine;

public class CharacterAnimationController : MonoBehaviour 
{
    private int interactHash;
    private Animator animator;
    [HideInInspector] public bool performingAction;

    private void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (interactHash  == 0)
        {
            interactHash = Animator.StringToHash("isInteracting");
        }
    }

    private void Update()
    {
        performingAction = animator.GetBool(interactHash);
    }

    public void PlayTargetAnimation(string targetAnimation, bool performAction, float transitionDuration = 0.1f)
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.applyRootMotion = performAction;
        animator.SetBool(interactHash, performAction);
        animator.CrossFade(targetAnimation, transitionDuration);
    }
}
