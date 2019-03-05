using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Enemy Decisions/Can Melee Attack")]
public class TransitionToMeleeAttack : Decision
{

    public override bool Decide(StateController controller)
    {
        if (controller.InMeleeRange)
        {
            controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, controller.InMeleeRange);
            return true;
        }
        else if (!controller.InMeleeRange)
        {
            controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, controller.InMeleeRange);
            return false;
        }
        else
        {
            controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, controller.InMeleeRange);
            return false;
        }
    }
}
