using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------------------------
// CLASS: GetInRange
// DESC:  Gets the enemy in range for an attack - so needs to update the destination basically
[CreateAssetMenu(menuName = "PluggableAI/Enemy Actions/Basic Get In Range")]
public class GetInRange : Action
{

    public override void Act(StateController controller)
    {
        // As we are setting the navagent destination, use a timer to avoid doing it every Update
        // I tested and the 0.25f time interval was enough without sacrificing gameplay believablility
        if (controller.CheckIfCountdownElapsed(0.25f))
        {
            MoveToPlayer(controller);
        }
    }

    // GetInRange()
    // Each enemy must get in range before attacking - but I code each enemies GetInRange() method seperately
    // Because they each have different conditional logic to process in order to know if they are in range
    // This method checks if there is no path pending, and that the agent is on a navmesh:
    private void MoveToPlayer(StateController controller)
    {
        if (!controller.NavAgent.pathPending && controller.NavAgent.isOnNavMesh)
        {
            controller.NavAgent.isStopped = false;
            // controller.NavAgent.destination = controller.Player.transform.position;
            if (controller.Player.grounded) 
                controller.NavAgent.SetDestination(controller.Player.transform.position);

        }
    }
}
