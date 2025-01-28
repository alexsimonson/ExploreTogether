using UnityEngine;

public class TheManAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Example of toggling between Idle and Walk animations
        if (Input.GetKey(KeyCode.W)) // Press W to "Walk"
        {
            SetAnimationState("Walk");
        }
        else // Default to "Idle" if no input
        {
            SetAnimationState("Idle");
        }
    }

    private void SetAnimationState(string stateName)
    {
        if (animator != null)
        {
            animator.Play(stateName);
        }
    }
}
