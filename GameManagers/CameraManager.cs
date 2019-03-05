using UnityEngine;

// -----------------------------------------------------------------------------------------------------------------------------
// CLASS: CameraManager
// DESCRIPTION: Chooses a random camera from the cameras childed to this transform, and sets it as the active camera
//              This is done so that when the user returns to the start screen, they are presented with a different view of some
//              level geometry
// ------------------------------------------------------------------------------------------------------------------------------
public class CameraManager : MonoBehaviour
{

 

    private void Awake()
    {
        Camera[] _cameras = new Camera[transform.childCount-1];
        _cameras = GetComponentsInChildren<Camera>();

        foreach (Camera cam in _cameras)
        {
            cam.enabled = false;
            cam.GetComponent<AudioListener>().enabled = false;
        }

        int x = Random.Range(0, transform.childCount);
        _cameras[x].enabled = true;
        _cameras[x].GetComponent<AudioListener>().enabled = true;

    }


}
