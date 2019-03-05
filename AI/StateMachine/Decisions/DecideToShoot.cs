using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="PluggableAI/Enemy Decisions/Decide To Shoot")]
public class DecideToShoot : Decision {

    public override bool Decide(StateController controller)
    {
        if (controller.InProjectileRange
            && controller.EnemySightScript.PlayerInSight)
        {
            //AnimatorController.SetBool(TagsHash.InProjectileRange, InProjectileRange);
            controller.AnimatorController.SetBool(controller.TagsHash.InProjectileRange, controller.InProjectileRange);
            return true;
        }
        else
        {
            controller.AnimatorController.SetBool(controller.TagsHash.InProjectileRange, controller.InProjectileRange);
            return false;
        }
    }
}
