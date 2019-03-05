using System.Collections.Generic;
using UnityEngine;
//-------------------------------------------------------------------------------
// CLASS:       DetectSpawnAreas
// DESCRIPTION: Detects which spawn areas (mesh convex trigger colliders) the player is 
//              Inside of, and adds them to a public list, for the EnemySpawner.cs file to read
//--------------------------------------------------------------------------------
public class DetectSpawnAreas : MonoBehaviour
{
    [HideInInspector] public List<Collider> ActiveSpawnAreas; // I use a list to contain the active spawn areas so they can easily be added / removed

    //-------------------------------------------------------------------------------
    // Method: OnTriggerEnter()
    // Desc:   Detects if player is inside a spawn area and adds that area to the list
    //--------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.SpawnArea))
        {
            ActiveSpawnAreas.Add(other);
        }
    }

    //-------------------------------------------------------------------------------
    // Method: OnTriggerExit()
    // Desc:   Detects if player is outside a spawn area and removes that area from the list
    //--------------------------------------------------------------------------------
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.SpawnArea))
        {
            if (ActiveSpawnAreas.Contains(other))
            {

                ActiveSpawnAreas.Remove(other);
            }
        }
    }
}
