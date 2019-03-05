using UnityEngine;
// -----------------------------------------------------------------------------------
// CLASS:       ChooseTargetAnimation
// DESCRIPTION: Asign this in an inspector to pass an integer value to the objects animator
//              this will asign an animation state to that object
//              Each red target in the start scene has it's own aninmation state, 
//              but they all share the same controller
// -----------------------------------------------------------------------------------
public class ChooseTargetAnimation : MonoBehaviour
{

    public int TargetAnim = 0;

    private void Awake()
    {
        GetComponent<Animator>().SetInteger("Anim", TargetAnim);
    }
}
