using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// --------------------------------------------------------------------------------------------------------
// Class: Transition
// Desc: Transitions are used to transition between states, depending on a decision
// They return true or false, and depending on the return value, we move to the true state, or false state
// --------------------------------------------------------------------------------------------------------
[System.Serializable]
public class Transition
{
    public Decision decision;
    public State trueState;
    public State falseState;
}
