using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------------------------------------------------------------------------------------------
// CLASS:       PerformMeleeAttack : Action
// DESCRIPTION: This class is used to create scriptable object so the enemies can perform a melee attack
//              This class is updated every frame when it is contained in a state, for example, the perform melee attack state
//              Like all actions, it inherits from the action.cs base class, and overrides the Act(StateController) method
//              The StateController file that calls and updates this action will pass itself in as the parameter
// ---------------------------------------------------------------------------------------------------------------------------
[CreateAssetMenu (menuName = "PluggableAI/Enemy Actions/Perform Melee Attack")]
public class PerformMeleeAttack : Action
{
    public override void Act(StateController controller)
    {
        MeleeAttack(controller);
    }

    private void MeleeAttack(StateController controller)
    {
        // If we are in range for a melee attack, and have not yet set the attack int parameter, set it to a random number now
        // So that the enemy can enter into a random melee attack animation:
        if (controller.InMeleeRange
            && controller.AnimatorController.GetInteger(controller.TagsHash.AttackType) == 0)
        {
            controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, true);

            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, Random.Range(1, 4));
        }
        // If we are not in Melee range, reset the boolean and integer value of the animator controller component:
        else if (!controller.InMeleeRange) 
        {
            controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, false);
            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, 0);
        }
    }
}
