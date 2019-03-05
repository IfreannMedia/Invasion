using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Enemy Actions/Flame Player")]
public class FlameAttackPlayer : Action {

    public override void Act(StateController controller)
    {
        EnterFlameAnimation(controller); // Enters into a flame animation state, which calls further methods in the EnemyAnimation.cs file
    }

    private void EnterFlameAnimation(StateController controller)
    {
        if (!controller.InFlamethrowerRange)
        {
            return;
        }

        Vector3 _direction; // Local var,for storing the direction to the player from the enemy
        float _angle;     // Local var, for calculating the angle between enemy forward vector and _direction

        _direction = controller.EnemySightScript._lastKnownPosition - controller.transform.position;
        _angle = Vector3.Angle(_direction, controller.transform.forward);

        // We may attack if we are in range, outside of melee range, have entered the ranged attack SubStateMachine
        // and the angle is less than the threshold set in inspector:
        if (controller.IsRangedAttacking
         && _angle <= controller.AttackAngleThreshold)
        {
            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, 1);
        }
        else
        {
            controller.AnimatorController.SetInteger(controller.TagsHash.AttackType, 0);
        }
    }
}
