using UnityEngine;
//-------------------------------------------------------------------
// CLASS:       Display Hint
// Description: Displays a random hint on the loading screen
//-------------------------------------------------------------------
public class DisplayHint : MonoBehaviour
{

    private Transform[] _hints;

    private void Awake()
    {
        _hints = GetComponentsInChildren<Transform>(true);
        int HintToActivate = Random.Range(0, transform.childCount + 1);
        _hints[HintToActivate].gameObject.SetActive(true);
    }

}
