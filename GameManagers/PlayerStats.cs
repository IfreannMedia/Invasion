using System.Collections;

// ----------------------------------------------------------------------------------------
// CLASS:       PlayerStats.cs
// DESCRIPTION: Simply a container for game statistics, like currently active enemies,
//              the player's health, the player's score, the enemies that were killed etc
// ----------------------------------------------------------------------------------------
public class PlayerStats
{

    
    private static float _playerHealth = 100f;
    private static int _enemiesKiled;
    private static float _soulPower = 20;// The ability player has to use telekinesis
    private  static int _score;
    private static int _headshots;
    private static int _activeEnemies = 0;
    private static int _activeArmouredEnemies = 0;
    private static int _activeFlamerEnemies = 0;
    private static bool _canPauseGame = true;

    public static float PlayerHealth        { get { return _playerHealth;   } set { _playerHealth  = value; } }
    public static int EnemiesKilled         { get { return _enemiesKiled;   } set { _enemiesKiled  = value; } } 
    public static float SoulPower           { get { return _soulPower;      } set { _soulPower     = value; } }
    public static int Score                 { get { return _score;          } set { _score         = value; } }
    public static int Headshots             { get { return _headshots;      } set { _headshots     = value; } }
    public static int ActiveEnemies         { get { return _activeEnemies;  } set { _activeEnemies = value; } }
    public static int ActiveArmouredEnemies { get { return _activeArmouredEnemies; } set { _activeArmouredEnemies = value; } }
    public static int ActiveFlamerEnemies   { get { return _activeFlamerEnemies; } set { _activeFlamerEnemies = value; } }
    public static bool CanPauseGame         { get { return _canPauseGame; } set { _canPauseGame = value; } }
}
