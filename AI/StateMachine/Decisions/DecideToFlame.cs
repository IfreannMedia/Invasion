using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Enemy Decisions/Decide To Flame")]
public class DecideToFlame : Decision
{

    public override bool Decide(StateController controller)
    {
        if (controller.InFlamethrowerRange
            && controller.EnemySightScript.PlayerInSight)
        {
            controller.AnimatorController.SetBool(controller.TagsHash.InFlamethrowerRange, controller.InFlamethrowerRange);
            return true;
        }
        else
        {
            controller.AnimatorController.SetBool(controller.TagsHash.InFlamethrowerRange, controller.InFlamethrowerRange);
            return false;
        }
    }
}
