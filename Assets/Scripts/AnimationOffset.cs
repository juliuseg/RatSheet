using UnityEngine;

public class AnimationOffset : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Randomize the start time of the animation
            animator.Play(0, -1, Random.Range(0f, 1f)); // Layer -1 means the current layer
        }
    }
}
