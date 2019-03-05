using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// --------------------------------------------------------------------------------------------------------------------
// CLASS: State
// DESC:  Scriptable Object State, used to create new states in the project folder for enemy AI
//        Each created state contains possible actions and transitions, which are looped through
//        StateController.cs calls UpdateState() every frame update. UpdateState() loops through actions and possible transitions
// --------------------------------------------------------------------------------------------------------------------
[CreateAssetMenu(menuName = "PluggableAI/State")]
public class State : ScriptableObject
{  

    public Action[] actions;
    public Transition[] transitions; // Transitions contain decisions, and facilitate state switching
    // Color for gizmo
    public Color sceneGizmoColor;

    // UpdateState()
    // This method is called in the Update() method of the StateController.cs script
    // so every update we loop through actions and transitions
    public void UpdateState(StateController controller)
    {
       
        DoActions(controller);
        CheckTransitions(controller);
    }

    // DoActions()
    // Perform each action
    private void DoActions(StateController controller)
    {
        // Loop through our actions and then do them according to the controller we pass in
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    // CheckTransitions()
    // Transions contain Decisions, which are methods that return a boolean value
    // Positive will transition to a tansitions "trueState", and false transitions to a state's "falseState"
    // The enemies in the game typically use one state, and so the decision returns false
    // A return of false means the enemies remain in their current state
    private void CheckTransitions(StateController controller)
    {
        // Loop through all of our transitions
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionsSucceeded = transitions[i].decision.Decide(controller);

            if (decisionsSucceeded)
            {
                controller.TransitionToState(transitions[i].trueState);
            }
            else
            {
                controller.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
