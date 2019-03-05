using UnityEngine;

// Prevents firing in an animation state - the same as the PreventFiring.cs state 
// except it sets the boolean on a single state rather than a subState machine
public class PreventFiringInState : StateMachineBehaviour {

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<FPSRigAnimator>().CanFire = false;
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<FPSRigAnimator>().CanFire = true;
    }

}
