using UnityEngine;

// Detects when the flamer enemy enters into a flame attack sub state machine, and passes
// this boolean value to the enemy StateController.cs file
public class DetectFlameAttackState : StateMachineBehaviour
{
	// OnStateMachineEnter is called when entering a statemachine via its Entry Node
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
        animator.GetComponent<StateController>().IsRangedAttacking = true;
	}

	// OnStateMachineExit is called when exiting a statemachine via its Exit Node
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        animator.GetComponent<StateController>().IsRangedAttacking = false;
    }
}
