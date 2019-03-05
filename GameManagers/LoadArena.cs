using UnityEngine;
//------------------------------------------------------------------------------------------
// CLASS:       LoadArena
// DESCRIPTION: Triggers loading an arena when the player character enters the box collider
//------------------------------------------------------------------------------------------
public class LoadArena : MonoBehaviour
{
    public StartScreenUI SceneManager; // I store scene management code in the StartScreenUI script
    [HideInInspector] public enum LevelToLoad { Arena01, Arena02}
    public LevelToLoad Level; // Assign which level we wish to load in the inspector


    //----------------------------------------------------------------
    // Method:      OnTriggerEnter
    // Description: Loads the level we have assigned in the inspector
    //----------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(TagsHashIDs.Player))
        {
            PlayerStats.CanPauseGame = false; // make sure we do not try to pause during a loading sequence
            SceneManager.DeactivateThePlayer();
            switch (Level)
            {
                case LevelToLoad.Arena01:
                    GetComponentInParent<AudioSource>().Play();
                    SceneManager.LoadTheFirstArena();
                    break;
                case LevelToLoad.Arena02:
                    GetComponentInParent<AudioSource>().Play();
                    SceneManager.LoadTheSecondArena();
                    break;
                default:
                    break;
            }
        }
    }
}
