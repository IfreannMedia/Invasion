using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// -----------------------------------------------------------------
// CLASS: Decision
// Desc:  Abstract base class for all decisions
// -----------------------------------------------------------------
public abstract class Decision : ScriptableObject
{
    // Decide will be overriden in derived classes, with the enemy instance passing in their own state controller as param
    public abstract bool Decide(StateController controller);

}
