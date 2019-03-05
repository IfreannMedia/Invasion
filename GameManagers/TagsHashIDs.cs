using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------------------------------
// CLASS: TagsHashIDs
// Desc:  Stores the strings for tags as consts to be easily used by other classes
//        this will help avoid mispelled entries. Also stores useful data like hash IDs
//        which belong to animators, for example of the player and enemy
// -------------------------------------------------------------------------------
public class TagsHashIDs : MonoBehaviour
{
    // TAGS
    public const string Player = "Player";
    public const string Range = "Range";
    public const string EnemyHead = "Enemy Head";
    public const string LevelParts = "Level Parts";
    public const string Waypoint = "Waypoint";
    public const string Gun = "Gun";
    public const string PauseMenu = "PauseMenu";
    public const string Enemy = "Enemy";
    public const string Projectile = "Projectile";
    public const string Ground = "Ground";
    public const string MeleeRange = "MeleeRange";
    public const string ProjectileRange = "ProjectileRange";
    public const string Sound = "Sound Emission";
    public const string Destructible = "Destructible";
    public const string EnvTrap = "Env Trap";
    public const string Shootable = "Shootable";
    public const string FlamethrowerRange = "FlamethrowerRange";
    public const string EnergyPack = "EnergyPack";
    public const string HandgunPickup = "HandgunPickup";
    public const string ShotgunPickup = "ShotgunPickup";
    public const string PlasmaRiflePickup = "PlasmaRiflePickup";
    public const string HealthPickup = "HealthPickup";
    public const string AmmoPickup = "AmmoPickup";
    public const string MeleeEnemy = "MeleeEnemy";
    public const string ArmouredEnemy = "ArmouredEnemy";
    public const string FlamerEnemy = "FlamerEnemy";
    public const string SpawnArea = "SpawnArea";

    // LAYERS
    public const string EnemyBodypart = "Enemy Bodypart";
    public const string AITrigger = "AITrigger";

    // LAYER INTEGERS
    private int AITriggerLayerInt;
    private int AISensorLayerInt;
    private int EnemyLayerInt;
    private int DefaultLayerInt;
    private int PlayerLayerInt;
    private int ShootableLayerInt;
    private int IgnoreRaycastLayerInt; // This is used by the EneySightScript

    // LAYER MASKS
    public int PlayerLayerMask, EnemyLayerMask, ShootableLayerMask, AISensorLayerMask, 
               AITriggerLayerMask, DefaultLayerMask, IgnoreRaycastLayerMask;

    
    // HASH IDS of ENEMY
    public int Speed; // float
    public int Angle; // float
    public int SeesPlayer; // Boolean
    public int PlayerInRange; // Boolean
    public int IsDead; // Trigger
    public int VelX; // Float
    public int VelY; // Float
    public int Draining; // Boolean
    public int KnockBack; // Trigger
    public int Hit; // Trigger
    public int Alive; // Boolean
    public int AttackType; // Int
    public int Seeking;
    public int InProjectileRange;
    public int InFlamethrowerRange;
    public int HasSeenPlayer;
    public int IsJumping; // Boolean for detecting when a jump animation should play if enemy is on an offmesh link
    // Animation State tags for Enemy:
    public const string IdleTag = "Idle";
    public const string TurnLeft = "Left Turn";
    public const string TurnRight = "Right Turn";
    // ALPHA FADE ID
    public int Fade; // Trigger
    
    // TORCH ANIMATOR
    public int TorchActivate; // Trigger

    // FPS RIG
    public int Attack; // Trigger attack
    public int FlingLeft; // Boolean fling to the left
    public int TeleAttack; // Boolean are we attacking
    public int TeleLockon; // Boolean are we locked on
    public int HasEnemy; // Boolean do we have an enemy
    public int FlingEnemy; // Trigger to fling enemy
    public int SwitchWeapon; // Trigger to force push
    public int Reload; // Trigger to reload
    public int FireGun; // Trigger to fire any Weapon
    public int SelectedWeapon; // Int value of weapon parameter for animator
    public int VelocityX;
    public int usingTwoHands;
    public int MeleeAttack;
    public int IsMeleeAttacking;

    // -------------------------------------------------------------------------------
    //  METHOD: Awake: Used to set up the animator HashIDs for other scripts
    //          This way each enemy instance does not need to cache their own Hash IDs,
    //          we just do it once here and then refer to it from other scripts
    // -------------------------------------------------------------------------------
    private void Awake()
    {
        // Cahche Enemy ID's:
        Speed               = Animator.StringToHash("Speed");
        Angle               = Animator.StringToHash("Angle");
        SeesPlayer          = Animator.StringToHash("SeesPlayer");
        PlayerInRange       = Animator.StringToHash("PlayerInRange");
        IsDead              = Animator.StringToHash("IsDead");
        VelX                = Animator.StringToHash("VelX");
        VelY                = Animator.StringToHash("VelY");
        Draining            = Animator.StringToHash("Draining");
        KnockBack           = Animator.StringToHash("KnockBack");
        Hit                 = Animator.StringToHash("Hit");
        Alive               = Animator.StringToHash("Alive");
        AttackType          = Animator.StringToHash("AttackType");
        Seeking             = Animator.StringToHash("Seeking");
        InProjectileRange   = Animator.StringToHash("InProjectileRange");
        InFlamethrowerRange = Animator.StringToHash("InFlamethrowerRange");
        HasSeenPlayer       = Animator.StringToHash("HasSeenPlayer");
        IsJumping           = Animator.StringToHash("IsJumping");
        // Cache FADE OUT and TORCH IDs:
        Fade                = Animator.StringToHash("Fade");
        TorchActivate       = Animator.StringToHash("TorchActivate");
        // Cache Player HashIDs:
        Attack               = Animator.StringToHash("Attack");
        FlingLeft            = Animator.StringToHash("FlingLeft");
        TeleAttack           = Animator.StringToHash("TeleAttack");
        TeleLockon           = Animator.StringToHash("TeleAttack");
        HasEnemy             = Animator.StringToHash("HasEnemy");
        FlingEnemy           = Animator.StringToHash("FlingEnemy");
        SwitchWeapon         = Animator.StringToHash("SwitchWeapon");
        Reload               = Animator.StringToHash("Reload"); 
        FireGun              = Animator.StringToHash("FireGun");
        SelectedWeapon       = Animator.StringToHash("CurrentWeapon");
        VelocityX            = Animator.StringToHash("VelocityX");
        usingTwoHands        = Animator.StringToHash("UsingTwoHands");
        MeleeAttack          = Animator.StringToHash("MeleeAttack");
        IsMeleeAttacking     = Animator.StringToHash("IsMeleeAttacking");
        // Get the layer ints through script in case they are changed later:
        AITriggerLayerInt = LayerMask.NameToLayer("AI Trigger");
        AISensorLayerInt = LayerMask.NameToLayer("AI Sensor");
        EnemyLayerInt = LayerMask.NameToLayer("Enemy");
        DefaultLayerInt = LayerMask.NameToLayer("Default");
        PlayerLayerInt = LayerMask.NameToLayer("Player");
        ShootableLayerInt = LayerMask.NameToLayer("Shootable");
        IgnoreRaycastLayerInt = LayerMask.NameToLayer("Ignore Raycast");

        // Bit Shift the layer masks for use in other scripts:
        PlayerLayerMask = 1 << PlayerLayerInt;
        EnemyLayerMask = 1 << EnemyLayerInt;
        AITriggerLayerMask = 1 << AITriggerLayerInt;
        AISensorLayerMask = 1 << AISensorLayerInt;
        ShootableLayerMask = 1 << ShootableLayerInt;
        DefaultLayerMask = 1 << DefaultLayerInt;
        IgnoreRaycastLayerMask = 1 << IgnoreRaycastLayerInt;
    }
}
