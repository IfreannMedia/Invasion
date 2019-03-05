
using UnityEngine;
//------------------------------------------------------------------------------------------------
// CLASS:       AutoDisableObject
// Description: This class is used to deactivate pooled objects automatically. This is done so that
//              When a class calls on the ObjectPooler.cs class to get an object from a pool, 
//              It will find some objects that are inactive in the scene. Otherwise objects 
//              remain in scene though not in use
//-------------------------------------------------------------------------------------------------
public class AutoDisableObject : MonoBehaviour
{

    public float TimeToDeactivate = 3.5f;

    //----------------------------------------------------------------------
    // OnEnable()
    // Sets up disabling the object every time it is used in game
    //----------------------------------------------------------------------
    private void OnEnable()
    {
        Invoke("DisableObject", TimeToDeactivate);
    }

    //----------------------------------------------------------------------
    // DisableObject()
    // Disables the object
    //----------------------------------------------------------------------
    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
    //----------------------------------------------------------------------
    // OnDisable()
    // Cancels the invoked method in case of edge case errors like DisableObject()
    // Being called twice
    //----------------------------------------------------------------------
    private void OnDisable()
    {
        CancelInvoke("DisableObject");
    }
}
