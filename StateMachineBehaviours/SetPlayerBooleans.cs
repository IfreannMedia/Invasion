using UnityEngine;

// Sets the boolean for melee attacking when the player performs a punch attack
public class SetPlayerBooleans : StateMachineBehaviour
{

    
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("IsMeleeAttacking", true);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("IsMeleeAttacking", false);
    }
}
