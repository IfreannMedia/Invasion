using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/Sees Player Decision")]
public class SeesPlayerDecision : Decision
{

    public override bool Decide(StateController controller)
    {
       return SeesPlayer(controller);
    }

    private bool SeesPlayer(StateController controller)
    {
        if (controller.EnemySightScript.PlayerInSight)
            return true;
        else
            return false;
    }
}
