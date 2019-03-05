using UnityEngine;
using TMPro;
// --------------------------------------------------------------------------
// CLASS: DisplayFinalScore
// Desc:  Displays the final score and kill count
// --------------------------------------------------------------------------
public class DisplayFinalScore : MonoBehaviour
{

    public TextMeshProUGUI Score, Killcount, FinalScore;

    private void OnEnable()
    {
        Score.text = PlayerStats.Score.ToString();
        Killcount.text = PlayerStats.EnemiesKilled.ToString();
        UpdateHighScore();
        FinalScore.text = UpdateHighScore().ToString();
    }


    //------------------------------------------------------------------------------
    // Method: UpdateHighScore()
    // Desc:   If we have a score higher than the previously saved score, update it
    //         Do this by seeing if we have the HighScore key, and setting/ getting it appropriately
    //         If there is no key, it's the first playthrough, so create one
    //------------------------------------------------------------------------------
    private int UpdateHighScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (PlayerPrefs.GetInt("HighScore") < PlayerStats.Score)
            {
                PlayerPrefs.SetInt("HighScore", PlayerStats.Score);
                PlayerPrefs.Save();
                return PlayerPrefs.GetInt("HighScore");
            }
            else
            {
                return PlayerPrefs.GetInt("HighScore");
            }
        }
        else // We are setting the first highscore:
        {
            PlayerPrefs.SetInt("HighScore", PlayerStats.Score);
            PlayerPrefs.Save();
            return PlayerStats.Score;
        }


    }


}
