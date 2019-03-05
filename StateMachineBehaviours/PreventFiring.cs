using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventFiring : StateMachineBehaviour
{

    // When we enter the state machine, tell the FPSRigAnimator.cs that we cannot enter into
    // A fire animation
    // Also set the boolean value for "UsingTwoHands" parameter so that the locomoation blend tree does not 
    // affect reload animations - I tried achieving this with the AutoSetLayerWeight.cs file, but it would not 
    // set the layer weighting of the locomotion blend tree layer to 0.0f for some reason
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
        animator.GetComponent<FPSRigAnimator>().CanFire = false;
        animator.SetBool("UsingTwoHands", true);
	}

	// reset the values
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        animator.GetComponent<FPSRigAnimator>().CanFire = true;
        animator.SetBool("UsingTwoHands", false);
    }
}
