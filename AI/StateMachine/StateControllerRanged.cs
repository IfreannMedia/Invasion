using UnityEngine;
// --------------------------------------------------------------------------------
// CLASS:       StateControllerRanged
// DESCRIPTION: Extends the normal StateController class for ranged enemy behaviour
//              Detects if AI can perform ranged attacks with pistol or flamethrower
//              Using Trigger event methods
// --------------------------------------------------------------------------------
public class StateControllerRanged : StateController
{

    // OnTriggerEnter
    // Overrides the base class method and extends it to allow this enemy to perform ranged and lamethrower attacks
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag(TagsHashIDs.ProjectileRange))
        {
           // InProjectileRange = true;
        }
        if (other.CompareTag(TagsHashIDs.FlamethrowerRange))
        {
            InFlamethrowerRange = true;
        }
    }


    // ----------------------------------------------------------------------------------
    // Method: OnTriggerStay
    // Desc:   This method tests if we are less than a certain distance from the player
    //         The ProjectileAttackRange float is randomized between 5 and 15 units for
    //         for each instance of the armoured enemy. This is done so armoured enemies do not
    //         all encircle the player form the same distance. Once the boolean is true
    //         We can transition to a shooting state
    //-------------------------------------------------------------------------------------
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.ProjectileRange))
        {
            if (Vector3.Distance(transform.position,Player.transform.position) <= ProjectileAttackRange)
            {
                InProjectileRange = true;
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag(TagsHashIDs.ProjectileRange))
        {
            InProjectileRange = false;
            AnimatorController.SetBool(TagsHash.InProjectileRange, InProjectileRange);
        }
        if (other.CompareTag(TagsHashIDs.FlamethrowerRange))
        {
            InFlamethrowerRange = false;
        }
    }

}
