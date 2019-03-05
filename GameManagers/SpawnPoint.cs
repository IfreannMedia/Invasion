using UnityEngine;
// ----------------------------------------------------------------------------------------------------
// CLASS:       SpawnPoint
// Description: This class is attached to each spawnpoint. Spawnpoints
//              Are childed to the EnemySpawner in the scene heirarchy
//              Spawnpoints detect if the player is in range, and then allow an enemy to be spawned
// -----------------------------------------------------------------------------------------------------
public class SpawnPoint : MonoBehaviour {

    public bool PlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag(TagsHashIDs.Player))
            PlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.Player))
            PlayerInRange = false;
    }
}
