using UnityEngine;
// ---------------------------------------------------------------------------------------------------------------------------
// CLASS:       AutoSetLayerWeight : StateMachineBehaviour
// DESCRIPTION: I like to asigne this script to sub-state machines in the player's animator
//              and can then set in the inspector which layers should be weighted fully and which not at all
//              For example when the playe enters into a reload-animation sub-state machine, I want to disable all other layers
// -----------------------------------------------------------------------------------------------------------------------------
public class AutoSetLayerWeight : StateMachineBehaviour
{
    [SerializeField] private int LayerToHide;
    [SerializeField] private int LayerToShow;
    [SerializeField] private bool HideAllLayers; // Hides all layers except current
    //private int[] _hiddenLayers = new int[6];

    
        // Here I'll automatically set the layer weighting so as to void glitchy behaviour
	// OnStateMachineEnter is called when entering a statemachine via its Entry Node
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (HideAllLayers)
        {
            int[] layers = new int[animator.layerCount];

            foreach (int layerindex in layers)
            {
                animator.SetLayerWeight(layerindex, 0.0f);
            }
        }
        else
        {
            animator.SetLayerWeight(LayerToHide, 0.0f);
        }
        animator.SetLayerWeight(LayerToShow, 1.0f);
	}
    
	// OnStateMachineExit is called when exiting a statemachine via its Exit Node
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {

        animator.SetLayerWeight(LayerToShow, 0.0f);
        animator.SetLayerWeight(LayerToHide, 1.0f);
    }
}
