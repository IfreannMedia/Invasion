using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------------------
// CLASS:       RecordingCamerasManager
// DESCRIPTION: This class us used for camera switching between an array of cams at runtime
//              these cams are used for recording shots to go into the promotional video for MTM
//----------------------------------------------------------------------------------------
public class RecordingCamerasManager : MonoBehaviour
{
    private Camera[] cameras;
    public bool iterate = true;


    void Awake ()
    {
        cameras = GetComponentsInChildren<Camera>(true); // get all cams

	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerStats.PlayerHealth = 10000;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[0].enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[1].enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[2].enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[3].enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[4].enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (Camera cam in cameras)
            {
                cam.enabled = false;
                cameras[5].enabled = true;
            }
        }
    }
}
