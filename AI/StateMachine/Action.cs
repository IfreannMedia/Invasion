using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// -------------------------------------------------------------
// CLASS: Action
// DESC:  Abstract base class for all other actions
// -------------------------------------------------------------
public abstract class Action : ScriptableObject
{
    // Abstract method to be overriden, takes a stateController parameter, will be executed in update loop
    public abstract void Act(StateController controller);

}
