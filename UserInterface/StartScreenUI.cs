using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------------------------------------------------
// CLASS:       StartScreenUI
// DESCRIPTION: Toggles the visibility of the HowToPlay UI elements, and responds to user input on buttons. Triggers the screen to
//              fade in and out appropriately and loads levels
//--------------------------------------------------------------------------------------------------------------------------------
public class StartScreenUI : MonoBehaviour {

    // Public fields
    public Image         AlphaFade;
    public MoveScript    Player;
    public CameraManager CameraManager;
    public GameObject[] MenuItems = new GameObject[3]; 
    public GameObject[] ControlsItems = new GameObject[3]; 
    public GameObject LoadingText;
    public Transform[] StartAreas = new Transform[3]; // Occlusion culling would not work in this scene so I'll disable unused env objs manually
    public AudioSource StartMusic;

    //-------------------------------------------------------------------------------
    // Method:      Start()
    // Description: Fades the screen in, gets a ref to the player, unlocks the cursor
    //--------------------------------------------------------------------------------
    void Start () {

        AlphaFade.gameObject.SetActive(true); // I leave it disabled as standard because it gets in the way during scene development
        if (!Player)
        {
            Player = FindObjectOfType<MoveScript>();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject go in MenuItems)
        {
            if (GetComponent<Image>())
            {
                GetComponent<Image>().CrossFadeAlpha(1.0f, 1.0f, false);
            }
        }
    }

    //-------------------------------------------------------------------------------------
    // Method:      ActivateThePlayer()
    // Description: public method called when Start button is clicked. Activates the player
    //--------------------------------------------------------------------------------------
    public void ActivateThePlayer()
    {
        StartCoroutine(ActivatePlayer());
    }

    //-------------------------------------------------------------------------------------
    // Method:      LoadTheFirstArena()
    // Description: public method called from UI button, loads arena
    //--------------------------------------------------------------------------------------
    public void LoadTheFirstArena()
    {
        StartCoroutine(LoadArena01());
    }

    //-------------------------------------------------------------------------------------
    // Method:      LoadTheFirstArena()
    // Description: public method called from UI button, loads arena
    //--------------------------------------------------------------------------------------
    public void LoadTheSecondArena()
    {
        StartCoroutine(LoadArena02());
    }

    //-------------------------------------------------------------------------------------
    // Method:      LoadTheFirstArena()
    // Description: public method, disables user input on player
    //--------------------------------------------------------------------------------------
    public void DeactivateThePlayer()
    {
        Player.GetComponent<Animator>().enabled = false;
        Player.GetComponentInChildren<FPSRigAnimator>().enabled = false;
        Player.enabled = false;
    }

    //-------------------------------------------------------------------------------------
    // Method:      ShowMeHowToPlay()
    // Description: public method called from UI button, shows controls
    //--------------------------------------------------------------------------------------
    public void ShowMeHowToPlay()
    {
        foreach (GameObject go in MenuItems)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in ControlsItems)
        {
            go.SetActive(true);
        }

    }

    //-------------------------------------------------------------------------------------
    // Method:      BackToMain()
    // Description: public method called from UI button, goes back to main menu (hides controls)
    //--------------------------------------------------------------------------------------
    public void BackToMain()
    {
        foreach (GameObject go in MenuItems)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in ControlsItems)
        {
            go.SetActive(false);
        }
    }


    //-------------------------------------------------------------------------------------
    // Method:      QuitGame()
    // Description: Quits the game
    //--------------------------------------------------------------------------------------
    public void QuitGame()
    {
        Application.Quit();
    }

    /*
    public void OpenSurveyPage()
    {
        Application.OpenURL("https://warrenroche.typeform.com/to/ua6Dil");
    }
     */

    //-------------------------------------------------------------------------------------
    // IEnumerator: LoadArena01()
    // Description: loads the first arena with SFX and screen fade, activates loading text with hints
    //--------------------------------------------------------------------------------------
    private IEnumerator LoadArena01()
    {
        PlayerStats.CanPauseGame = false;
        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        while (AlphaFade.color.a != 1)
        {
            StartMusic.volume -= 0.1f;
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitUntil(() => AlphaFade.color.a == 1);
        LoadingText.SetActive(true);
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Arena4");
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

    }

    //-------------------------------------------------------------------------------------
    // IEnumerator: LoadArena01()
    // Description: loads the second arena with SFX and screen fade, activates loading text with hints
    //--------------------------------------------------------------------------------------
    private IEnumerator LoadArena02()
    {
        PlayerStats.CanPauseGame = false;
        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        while (AlphaFade.color.a != 1)
        {
            StartMusic.volume -= 0.1f;
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitUntil(() => AlphaFade.color.a == 1);
        LoadingText.SetActive(true);
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Arena2");
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

    }

    //--------------------------------------------------------------------------------------------
    // Method: ActivatePlayer(), IEnumerator
    // Desc:   Fades screen to black, deactivates cameras, activates player, fades screen to clear
    //---------------------------------------------------------------------------------------------
    private IEnumerator ActivatePlayer()
    {
        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitUntil(() => AlphaFade.color.a == 1f);
        foreach (GameObject go in MenuItems)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in ControlsItems)
        {
            go.SetActive(false);
        }
        foreach (Camera cam in CameraManager.GetComponentsInChildren<Camera>())
        {
            cam.gameObject.SetActive(false); // Deactivate the start screen cameras and their audio isteners
        }
        Player.gameObject.SetActive(true);
        AlphaFade.GetComponent<Animator>().SetTrigger("Fade");
        foreach (Transform Transform in StartAreas)
        {
            Transform.gameObject.SetActive(false);
        }
        yield return null;
    }
}
