using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private ButterflyController butterflyController;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        butterflyController = GetComponent<ButterflyController>();
    }

    void Update()
    {
        // Get the player's movement input
        Vector2 movementInput = butterflyController.GetMovementInput();
        bool isGrounded = butterflyController.IsGrounded();
        bool isAscending = butterflyController.IsAscending();

        // Set the parameters based on the player's state
        animator.SetBool("NoInput", butterflyController.NoInput());
        animator.SetBool("IsGrounded", isGrounded);
        if (isGrounded)
        {
            animator.SetBool("NoInput", true);
        }
        animator.SetBool("IsAscending", isAscending);
        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);


        // Determine the animation state
        // if (isGrounded)
        // {
        //     if (movementInput == Vector2.zero)
        //     {
        //         animator.Play("Idle");
        //     }
        //     else if (movementInput.y > 0)
        //     {
        //         animator.Play("Straight");
        //     }
        //     else if (movementInput.y < 0)
        //     {
        //         animator.Play("Back");
        //     }
        //     else if (movementInput.x > 0)
        //     {
        //         animator.Play("Right");
        //     }
        //     else if (movementInput.x < 0)
        //     {
        //         animator.Play("Left");
        //     }
        // }
        // else
        // {
        //     if (isAscending)
        //     {
        //         animator.Play("Up");
        //     }
        //     else if (movementInput == Vector2.zero)
        //     {
        //         animator.Play("Glide");
        //     }
        // }
    }
}