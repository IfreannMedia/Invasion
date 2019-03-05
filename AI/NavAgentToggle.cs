using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[SharedBetweenAnimators]
public class NavAgentToggle : StateMachineBehaviour {

    NavMeshAgent agent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //agent = animator.gameObject.GetComponent<NavMeshAgent>();
        //if (agent)
        //animator.gameObject.GetComponent<NavMeshAgent>().enabled = false;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //if (animator.GetNextAnimatorStateInfo(0).IsTag("UseRootRotation"))
        //{
        //    agent.enabled = false;

        //}
        //else { agent.enabled = true; }
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
