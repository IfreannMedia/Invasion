
using UnityEngine;

public class HideCursor : MonoBehaviour
{

	// Update is called once per frame
	void Update () {
        if (Time.timeScale == 1f)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Time.timeScale < 1.0f)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
	}
}
