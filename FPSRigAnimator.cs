using UnityEngine;

// ---------------------------------------------------------------------------------------------------------------
// CLASS:       FPSRigAnimator
// DESCRIPTION: This class is responsible for animating the FPS rig - it listens for user input and enters into
//              attack animations when appropriate. The WeaponManager.cs script also calls methods that are contained here
//              PLayer firing animations also call animation event methods, which are contained here
// ---------------------------------------------------------------------------------------------------------------
public class FPSRigAnimator : MonoBehaviour
{

    // Public Fields
    public TagsHashIDs              TagsHashIDs;    // Reference to cahced animator HashIDs and tags and layers etc
    public AudioSource              PunchSwing;
    [HideInInspector]public bool    CanFire = true; // Used for boolean switch
    [HideInInspector] public bool HasHandgun = false, HasShotgun = false, HasPlasmaRifle = false; // Does the player have the weapon

    // Private Fields
    private Animator        _fpsAnimator;
    private int             _noWeaponLayer;         // Integer value for the no weapons animation layer
    private int             _handgunLayer;          // Integer value for the handgun animation layer
    private int             _macheteLayer;
    private int             _shotgunLayer;
    private int             _plasmaRifleLayer;
    //private int             _pickupReloadLayer;
    private WeaponSet       _currentWeapon;         // The current weapon we are using
    private WeaponManager   _weaponManagerScript;   // Reference to weapon manager script
    private Handgun         _handgun;               // Reference to the actual handgun in the player's heirarchy
    private Shotgun         _shotgun;
    private PlasmaRifle     _plasmaRifle;
    private MoveScript      _playerMoveScript;
    private CharacterController _charCont;
    private PlayerMeleeAttack _meleeAttackScript;   // Script for melee Attacking

    // ------------------------------------------------------------------------------------------
    // METHOD:      Awake()
    // DESCRIPTION: Cache global variables and convert layer names to their integer values
    // ------------------------------------------------------------------------------------------
    void Awake () {
        TagsHashIDs = FindObjectOfType<TagsHashIDs>();

        _weaponManagerScript = GetComponent<WeaponManager>();
        _fpsAnimator = GetComponent<Animator>();
        _noWeaponLayer = _fpsAnimator.GetLayerIndex("NoWeaponLayer");
        _handgunLayer = _fpsAnimator.GetLayerIndex("HandgunLayer");
        _macheteLayer = _fpsAnimator.GetLayerIndex("MacheteLayer");
        _shotgunLayer = _fpsAnimator.GetLayerIndex("ShotgunLayer");
        _plasmaRifleLayer = _fpsAnimator.GetLayerIndex("PlasmaRifleLayer");
        //_pickupReloadLayer = _fpsAnimator.GetLayerIndex("PickupAndReload");



        _handgun = GetComponentInChildren<Handgun>(true);
        _shotgun = GetComponentInChildren<Shotgun>(true);
        _plasmaRifle = GetComponentInChildren<PlasmaRifle>(true);
        
    }

    // ------------------------------------------------------------------------------------------
    // METHOD:      Start()
    // DESCRIPTION: Cache global variables that cannot be cached in the Awake() method
    // ------------------------------------------------------------------------------------------
    private void Start()
    {
        _playerMoveScript = GetComponentInParent<MoveScript>();
        _charCont = GetComponentInParent<CharacterController>();
        _meleeAttackScript = GetComponentInChildren<PlayerMeleeAttack>();
    }


    // --------------------------------------------------------------------------------
    // METHOD:      Update
    // DESCRIPTION: Listens for user input and controls entering into animation states
    //              Code for firing, reloading, and weapon switching is contained here
    // --------------------------------------------------------------------------------
    void Update () {
        // The current weapon is contained in the WeaponManager.cs file
        // Make sure we have the correct reference to it:
        if (_currentWeapon != _weaponManagerScript.CurrentWeapon)
                _currentWeapon = _weaponManagerScript.CurrentWeapon; 

        // Code to trigger weapon firing animations:
        switch (_currentWeapon)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:
                // If weare usingthe handgun, have bullets in the mag, and the canfire bool is true:
                if (Input.GetButtonDown("Fire1") && _handgun.shotCount < _handgun.MagazineCapacity && CanFire)
                {
                    _fpsAnimator.SetTrigger(TagsHashIDs.FireGun); // Pass the relevant parameter to the animator
                }
                else if (Input.GetButtonDown("Fire1") && _handgun.shotCount == _handgun.MagazineCapacity)
                {
                    if (_handgun.CurrentAmmo > 0)
                    {
                        TriggerReload(WeaponSet.Handgun); // Reload the gun if we have no bullets in clip, but do have ammo in stock
                    }
                    else
                    {
                        // No Ammo
                    }
                }
                if ((Input.GetButtonDown("Reload") || _handgun.shotCount > _handgun.MagazineCapacity) && _handgun.CurrentAmmo > 0
                    && _handgun.shotCount > 0)
                {
                    TriggerReload(WeaponSet.Handgun);
                }
                break;

            case WeaponSet.Shotgun:

                if (Input.GetButtonDown("Fire1") && _shotgun.shotCount < _shotgun.MagazineCapacity && CanFire)
                {
                    _fpsAnimator.SetTrigger(TagsHashIDs.FireGun);
                }
                else if (Input.GetButtonDown("Fire1") && _shotgun.shotCount == _shotgun.MagazineCapacity)
                {
                    if (_shotgun.CurrentAmmo > 0)
                    {
                        TriggerReload(WeaponSet.Shotgun);
                    }
                    else
                    {
                        // No Ammo
                    }
                }
                if ((Input.GetButtonDown("Reload") || _shotgun.shotCount > _shotgun.MagazineCapacity) && _shotgun.CurrentAmmo > 0
                    && _shotgun.shotCount > 0)
                {
                    TriggerReload(WeaponSet.Shotgun);
                }

                break;

            case WeaponSet.PlasmaRifle:

                if (Input.GetButtonDown("Fire1") && _plasmaRifle.shotCount < _plasmaRifle.MagazineCapacity && CanFire)
                {
                    _fpsAnimator.SetTrigger(TagsHashIDs.FireGun);
                }
                else if (Input.GetButtonDown("Fire1") && _plasmaRifle.shotCount == _plasmaRifle.MagazineCapacity)
                {
                    if (_plasmaRifle.CurrentAmmo > 0)
                    {
                        TriggerReload(WeaponSet.PlasmaRifle);
                    }
                    else
                    {
                        // No Ammo
                    }
                }

                if ((Input.GetButtonDown("Reload") || _plasmaRifle.shotCount > _plasmaRifle.MagazineCapacity) && _plasmaRifle.CurrentAmmo >0
                    && _plasmaRifle.shotCount > 0)
                {
                    TriggerReload(WeaponSet.PlasmaRifle);
                }

                break;

            case WeaponSet.RailGun: // the RailGun was modelled and imported but not animated nor coded to shoot, the player cannot switch to it

                if (Input.GetButtonDown("Fire1"))
                {
                    _fpsAnimator.SetTrigger(TagsHashIDs.FireGun);
                }
                if (Input.GetButtonDown("Reload"))
                {
                    _fpsAnimator.SetTrigger(TagsHashIDs.Reload);
                }
                break;
            default:
                break;
        }

        // MELEE ATTACK CODE:
        if (Input.GetButtonDown("Fire2"))
        {
            // Make sure the player cannot punch when using both hands, like reloading
            if (!_fpsAnimator.GetBool(TagsHashIDs.usingTwoHands) && !_fpsAnimator.GetBool(TagsHashIDs.IsMeleeAttacking))
            {
                PunchSwing.pitch = Random.Range(0.60f, 1.01f);
                PunchSwing.PlayDelayed(0.1f);
                _fpsAnimator.SetTrigger(TagsHashIDs.MeleeAttack);
            }
        }
        // CODE FOR THE LOCOMOTION BLEND TREE
        // Has to be placed here because would not pass the float value if contained in the MoveScript.cs file for some reason
        if (_playerMoveScript.grounded)
        {
            _fpsAnimator.SetFloat(TagsHashIDs.VelocityX, _charCont.velocity.magnitude);
        }
    }

    // ------------------------------------------------------------------------------------
    // METHOD:      SwitchWeapon
    // DESCRIPTION: Cycles through available weapons, and calls UpdateAnimator()
    //              Detects which weapons are available to switch to based on boolean values
    // -------------------------------------------------------------------------------------
    public void SwitchWeapon()
    {
        switch (_currentWeapon)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:

                if (HasShotgun)
                {
                    _handgun.gameObject.SetActive(false);
                    _plasmaRifle.gameObject.SetActive(false);
                    _shotgun.gameObject.SetActive(true); // disable and enable the appropraite gameobjects
                    UpdateAnimator(WeaponSet.Shotgun);   // Update the layer weightings of the animator
                    _currentWeapon = WeaponSet.Shotgun;  // Assign the correct weapon
                }
                else if (!HasShotgun && HasPlasmaRifle)
                {
                    _handgun.gameObject.SetActive(false);
                    _plasmaRifle.gameObject.SetActive(true);
                    UpdateAnimator(WeaponSet.PlasmaRifle);
                    _currentWeapon = WeaponSet.PlasmaRifle;
                }
                break;

            case WeaponSet.Shotgun:

                if (HasPlasmaRifle)
                {
                    _shotgun.gameObject.SetActive(false);
                    _plasmaRifle.gameObject.SetActive(true);
                    UpdateAnimator(WeaponSet.PlasmaRifle);
                    _currentWeapon = WeaponSet.PlasmaRifle;
                }
                else if (!HasPlasmaRifle && HasHandgun)
                {
                    _shotgun.gameObject.SetActive(false);
                    _handgun.gameObject.SetActive(true);
                    UpdateAnimator(WeaponSet.Handgun);
                    _currentWeapon = WeaponSet.Handgun;
                }
                break;

            case WeaponSet.PlasmaRifle:

                if (HasHandgun)
                {
                    _shotgun.gameObject.SetActive(false);
                    _plasmaRifle.gameObject.SetActive(false);
                    _handgun.gameObject.SetActive(true);
                    UpdateAnimator(WeaponSet.Handgun);
                    _currentWeapon = WeaponSet.Handgun;
                }
                break;


            default:
                break;
        }
    }

    // --------------------------------------------------------------------
    // METHOD:      SwitchWeapon(WeaponToSwitchTo)
    // DESCRIPTION: This method is called from Pickup.cs, to handle automatically
    //              switching to the newly picked up weapon when appropraite
    // --------------------------------------------------------------------
    public void SwitchWeapon(WeaponSet weaponToSwitchTo)
    {
        switch (weaponToSwitchTo)
        {
            case WeaponSet.None:
                _fpsAnimator.SetTrigger("SwitchWeapon");// Trigger the switch weapon animation
                _shotgun.gameObject.SetActive(false);
                _plasmaRifle.gameObject.SetActive(false);
                _handgun.gameObject.SetActive(false); // enable and disable appropriate gameObjects
                UpdateAnimator(WeaponSet.None);       // Update animator layer weightings
                _currentWeapon = WeaponSet.None;      // Assign the correct weapon value
                break;
            case WeaponSet.Handgun:
                _fpsAnimator.SetTrigger(TagsHashIDs.SwitchWeapon);
                _shotgun.gameObject.SetActive(false);
                _plasmaRifle.gameObject.SetActive(false);
                _handgun.gameObject.SetActive(true);
                UpdateAnimator(WeaponSet.Handgun);
                _currentWeapon = WeaponSet.Handgun;
                break;
            case WeaponSet.Shotgun:
                _fpsAnimator.SetTrigger(TagsHashIDs.SwitchWeapon);
                _handgun.gameObject.SetActive(false);
                _plasmaRifle.gameObject.SetActive(false);
                _shotgun.gameObject.SetActive(true);
                UpdateAnimator(WeaponSet.Shotgun);
                _currentWeapon = WeaponSet.Shotgun;
                break;
            case WeaponSet.PlasmaRifle:
                _fpsAnimator.SetTrigger(TagsHashIDs.SwitchWeapon);
                _handgun.gameObject.SetActive(false);
                _shotgun.gameObject.SetActive(false);
                _plasmaRifle.gameObject.SetActive(true);
                UpdateAnimator(WeaponSet.PlasmaRifle);
                _currentWeapon = WeaponSet.PlasmaRifle;
                break;
            case WeaponSet.RailGun:

                break;
            default:
                break;
        }
    }


    //-----------------------------------------------------------------------------------
    // Method:      TriggerReload
    // Description: Triggers a reload for weaponToReload. Decided to encapsulate reload 
    //              code here because sometimes is useful to call from pickup.cs script
    //              For example the player has 0 bullets in the handgun and picks up a handgun
    //              a reload animation will automatically play
    //-----------------------------------------------------------------------------------
    public void TriggerReload(WeaponSet weaponToReload)
    {
        if (!CanFire)
        {
            return;
        }
        switch (weaponToReload)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:
                _handgun.Reload();
                _fpsAnimator.SetTrigger(TagsHashIDs.Reload);
                _handgun.ReloadAudio.pitch = Random.Range(0.95f, 1.05f);
                _handgun.ReloadAudio.Play();
                break;
            case WeaponSet.Shotgun:
                _shotgun.Reload();
                _fpsAnimator.SetTrigger(TagsHashIDs.Reload);
                _shotgun.ReloadAudio.pitch = Random.Range(0.95f, 1.05f);
                _shotgun.ReloadAudio.Play();
                break;
            case WeaponSet.PlasmaRifle:
                _plasmaRifle.Reload();
                _fpsAnimator.SetTrigger(TagsHashIDs.Reload);
                _plasmaRifle.ReloadAudio.pitch = Random.Range(0.95f, 1.05f);
                _plasmaRifle.ReloadAudio.Play();
                break;
            case WeaponSet.RailGun:
                break;
            default:
                break;
        }
    }


    // --------------------------------------------------------------------
    // METHOD:      UpdateAnimator
    // DESCRIPTION: Changes animation layer weightings when we switch weapon
    // --------------------------------------------------------------------
    /// <summary>
    /// Switches from current weapon to the "WeaponToSwitchTo"
    /// </summary>
    /// <param name="WeaponToSwitchTo"></param>
    public void UpdateAnimator(WeaponSet WeaponToSwitchTo)
    {

        switch (WeaponToSwitchTo)
        {
            case WeaponSet.None:
                _fpsAnimator.SetLayerWeight(_noWeaponLayer, 1.0f);  // Set the appropraite layer weightings
                _fpsAnimator.SetLayerWeight(_plasmaRifleLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_shotgunLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_macheteLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_handgunLayer, 0f);
                _fpsAnimator.SetInteger(TagsHashIDs.SelectedWeapon, (int)WeaponSet.None); // set the animator integer to the enum value using a cast
                break;
            case WeaponSet.Handgun:
                _fpsAnimator.SetLayerWeight(_noWeaponLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_plasmaRifleLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_shotgunLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_macheteLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_handgunLayer, 1f);
                _fpsAnimator.SetInteger(TagsHashIDs.SelectedWeapon, (int)WeaponSet.Handgun);
                break;
            case WeaponSet.Shotgun:
                _fpsAnimator.SetLayerWeight(_noWeaponLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_plasmaRifleLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_shotgunLayer, 1.0f);
                _fpsAnimator.SetLayerWeight(_macheteLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_handgunLayer, 0.0f);
                _fpsAnimator.SetInteger(TagsHashIDs.SelectedWeapon, (int)WeaponSet.Shotgun);
                break;
            case WeaponSet.PlasmaRifle:
                _fpsAnimator.SetLayerWeight(_noWeaponLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_plasmaRifleLayer, 1.0f);
                _fpsAnimator.SetLayerWeight(_shotgunLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_macheteLayer, 0.0f);
                _fpsAnimator.SetLayerWeight(_handgunLayer, 0.0f);
                _fpsAnimator.SetInteger(TagsHashIDs.SelectedWeapon, (int)WeaponSet.PlasmaRifle);
                break;
            case WeaponSet.RailGun:
                break;
            default:
                break;
        }


    }
    // ----------------------------------------------------------------------------------------
    // METHOD:      UpdateAmmoCount, Animation event
    // DESCRIPTION: Updates the ammo display of weapon, by calling the method in the appropriate script file
    // -----------------------------------------------------------------------------------------
    public void UpdateAmmoDisplay()
    {

        switch (_currentWeapon)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:
                _handgun.UpdateAmmoCount(_handgun._ammoText, _handgun.CurrentAmmo, _handgun.MagazineCapacity);
                break;
            case WeaponSet.Shotgun:
                _shotgun.UpdateAmmoCount(_shotgun._ammoText, _shotgun.CurrentAmmo, _shotgun.MagazineCapacity);
                break;
            case WeaponSet.PlasmaRifle:
                _plasmaRifle.UpdateAmmoCount(_plasmaRifle._ammoText, _plasmaRifle.CurrentAmmo, _plasmaRifle.MagazineCapacity);
                break;
            case WeaponSet.RailGun:
                break;
            default:
                break;
        }
    }

    // -----------------------------------------------------------------------------------------------------------
    // METHOD:      OverlapSphereAttack(), Animation event
    // DESCRIPTION: Casts an overlap sphere when the player performs a punch, origin of sphere being the hand bone
    //              This is preferable to using a trigger collider on the hand, because it leads to erroneuos and 
    //              unpredictable results
    // ------------------------------------------------------------------------------------------------------------
    private void OverlapSphereAttack()
    {
        _meleeAttackScript.OverlapSphereAttack();
    }

    // --------------------------------------------------------------------------------
    // METHOD:      FireWeapon()
    // DESCRIPTION: Animation event; Performs the spherecast and updates the ammo count
    //              Calls FireWeapon() in the specific weapon class.
    // --------------------------------------------------------------------------------
    private void FireWeapon()
    {
        switch (_currentWeapon)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:
                _handgun.shotCount++; // increment the shotcounter
                _handgun.UpdateAmmoCount(_handgun._ammoText, _handgun.CurrentAmmo, _handgun.MagazineCapacity); // update the ammo display
                _handgun.FireWeapon(_handgun.BulletRadius, _handgun.NonEffectiveRange, _handgun.Damage, _handgun.HeadshotDamage,
                            _handgun.shotCount); // perform the spherecast
                break;
            case WeaponSet.Shotgun:
                _shotgun.shotCount++;
                _shotgun.UpdateAmmoCount(_shotgun._ammoText, _shotgun.CurrentAmmo, _shotgun.MagazineCapacity);
                _shotgun.FireWeapon(_shotgun.BulletRadius, _shotgun.NonEffectiveRange, _shotgun.Damage, _shotgun.HeadshotDamage,
                            _shotgun.shotCount);
                break;
            case WeaponSet.PlasmaRifle:
                _plasmaRifle.shotCount++;
                _plasmaRifle.UpdateAmmoCount(_plasmaRifle._ammoText, _plasmaRifle.CurrentAmmo, _plasmaRifle.MagazineCapacity);
                _plasmaRifle.FireWeapon(_plasmaRifle.BulletRadius, _plasmaRifle.NonEffectiveRange, _plasmaRifle.Damage, _plasmaRifle.HeadshotDamage,
                            _plasmaRifle.shotCount);
                break;
            case WeaponSet.RailGun:
                break;
            default:
                break;
        }
    }

    private void ToggleSmokeTrail()
    {
        _handgun.ToggleSmokeTrail();
    }

    // -----------------------------------------------------------------------------------------------------
    // METHOD:      AuxiliaryGunEffects(), Animation Event; 
    // DESCRIPTION: Activates auxilliary effets for weapons like charging or venting steam
    // -----------------------------------------------------------------------------------------------------
    private void AuxiliaryGunEffects()
    {
        switch (_currentWeapon)
        {
            case WeaponSet.None:
                break;
            case WeaponSet.Handgun:
                _handgun.ChargeGun();
                break;
            case WeaponSet.Shotgun:
                _shotgun.VentSteam();
                break;
            case WeaponSet.PlasmaRifle:
                break;
            case WeaponSet.RailGun:
                break;
            default:
                break;
        }
    }

}
