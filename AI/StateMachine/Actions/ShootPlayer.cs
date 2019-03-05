using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Enemy Actions/Shoot Player")]
public class ShootPlayer : Action
{

    public override void Act(StateController controller)
    {
        ShootAtPlayer(controller);
    }

    private void ShootAtPlayer(StateController controller)
    {
       

        if (!controller.InProjectileRange)
        {
            return;
        }
        // Tell the animator controler if we are in range for attacks or not:
        //controller.AnimatorController.SetBool(controller.TagsHash.InProjectileRange, controller.InProjectileRange);

        //controller.AnimatorController.SetBool(controller.TagsHash.PlayerInRange, controller.InMeleeRange);
        //Debug.Log("Ranged Attack method");
        Vector3 _direction = controller.EnemySightScript._lastKnownPosition - controller.transform.position;
        float _angle = Vector3.Angle(_direction, controller.transform.forward);
       // Debug.Log("calculated values");
        // Attack if we are in range, have line of sight,are facing the correct angle, and no attack anim is currently playing:
             
        if (_angle <= controller.AttackAngleThreshold)
        // && controller.EnemySightScript.PlayerInSight
        //&& controller.IsRangedAttacking
        {
            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, Random.Range(1, 3));
        }
        // reset the params if we are out of range:
        else
        {
            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, 0);
        }
    }
}
