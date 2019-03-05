using UnityEngine;

// ------------------------------------------------------------------------------------------------------------------------------------------
// CLASS:       WeaponManager
// DESCRIPTION: This class is responsible for enabling players to change their selected weapon. It is basically an admistrative/manager class.
//              It defines which weapons are available, responds to user input, and triggers a method call in the FPSRigAnimator.cs class.
//              Previously this was handled exclusively in the FPSRigAnimator.cs file, but I opted to relocate and clean up that code 
//              to make it more readable
// ------------------------------------------------------------------------------------------------------------------------------------------

public enum WeaponSet { None, Handgun, Shotgun, PlasmaRifle, RailGun } // enum defines possible weapons
public class WeaponManager : MonoBehaviour {

    // public fields
    [HideInInspector] public WeaponSet CurrentWeapon; // the currently selected weapon
    [HideInInspector] public bool HasHandgun = false, HasShotgun = false, HasPlasmaRifle = false; // booleans for if the player has picked up weapons
    
    // Private fiels
    private FPSRigAnimator _fpsAnimator;

    // ------------------------------------------------------------------------------
    // METHOD:      Awake
    // DESCRIPTION: Gets all necessary components for managing weapons
    //              Detects if the player has any weapons and switches to that weapon
    // ------------------------------------------------------------------------------
    private void Start()
    {
        _fpsAnimator = GetComponent<FPSRigAnimator>(); // get the animator script to call public methods
        // Check if we have any weapons by default
        if (GetComponentInChildren<Handgun>(true).gameObject.activeInHierarchy)
        {
            HasHandgun = true;
            CurrentWeapon = WeaponSet.Handgun;
            _fpsAnimator.SwitchWeapon(WeaponSet.Handgun);
        }
        else if (GetComponentInChildren<Shotgun>(true).gameObject.activeInHierarchy)
        {
            HasShotgun = true;
            CurrentWeapon = WeaponSet.Shotgun;
            _fpsAnimator.SwitchWeapon(WeaponSet.Shotgun);
        }
        else if (GetComponentInChildren<PlasmaRifle>(true).gameObject.activeInHierarchy)
        {
            HasPlasmaRifle = true;
            CurrentWeapon = WeaponSet.PlasmaRifle;
            _fpsAnimator.SwitchWeapon(WeaponSet.PlasmaRifle);
        }
        else
        {
            CurrentWeapon = WeaponSet.None;
            _fpsAnimator.SwitchWeapon(WeaponSet.None);
        }
    }

    // -------------------------------------------------------------------
    // METHOD:      Update
    // DESCRIPTION: Listens for user input
    // -------------------------------------------------------------------
    private void Update()
    {
        if (Input.GetButtonDown("Switch Weapon") && _fpsAnimator.CanFire && PlayerStats.PlayerHealth > 0)
        { SwitchWeapon(); }
    }
    // --------------------------------------------------------------------------------------------
    // METHOD:      SwitchWeapon
    // DESCRIPTION: Cycles through available weapons, and calls SwitchWeapon from FPSRigAnimator.cs
    //              Sets the CurrentWeapon variable to the correct value
    // ---------------------------------------------------------------------------------------------
    public void SwitchWeapon()
    {
        switch (CurrentWeapon)
        {
            case WeaponSet.None:
                if (HasHandgun)
                {
                    _fpsAnimator.SwitchWeapon(WeaponSet.Handgun);
                    CurrentWeapon = WeaponSet.Handgun;
                }

                break;
            case WeaponSet.Handgun:

                if (HasShotgun)
                {

                    _fpsAnimator.SwitchWeapon(WeaponSet.Shotgun);
                    CurrentWeapon = WeaponSet.Shotgun;
                }
                else if (!HasShotgun && HasPlasmaRifle)
                {

                    _fpsAnimator.SwitchWeapon(WeaponSet.PlasmaRifle);
                    CurrentWeapon = WeaponSet.PlasmaRifle;
                }
                break;

            case WeaponSet.Shotgun:

                if (HasPlasmaRifle)
                {

                    _fpsAnimator.SwitchWeapon(WeaponSet.PlasmaRifle);
                    CurrentWeapon = WeaponSet.PlasmaRifle;
                }
                else if (!HasPlasmaRifle && HasHandgun)
                {

                    _fpsAnimator.SwitchWeapon(WeaponSet.Handgun);
                    CurrentWeapon = WeaponSet.Handgun;
                }
                break;

            case WeaponSet.PlasmaRifle:

                if (HasHandgun)
                {
                    _fpsAnimator.SwitchWeapon(WeaponSet.Handgun);
                    CurrentWeapon = WeaponSet.Handgun;
                }
                break;


            default:
                break;
        }
    }

    // -------------------------------------------------------------------
    // METHOD:      Overload; SwitchWeapon(weaponToSwitchTo)
    // DESCRIPTION: Switches from current weapon to a specific weapon.
    //              Called from Pickup.cs when user picks up a new weapon
    // -------------------------------------------------------------------
    public void SwitchWeapon(WeaponSet weaponToSwitchTo)
    {
        switch (weaponToSwitchTo)
        {
            case WeaponSet.Handgun:
                _fpsAnimator.SwitchWeapon(WeaponSet.Handgun);
                CurrentWeapon = WeaponSet.Handgun;
                break;
            case WeaponSet.Shotgun:
                _fpsAnimator.SwitchWeapon(WeaponSet.Shotgun);
                CurrentWeapon = WeaponSet.Shotgun;
                break;
            case WeaponSet.PlasmaRifle:
                _fpsAnimator.SwitchWeapon(WeaponSet.PlasmaRifle);
                CurrentWeapon = WeaponSet.PlasmaRifle;
                break;
            case WeaponSet.RailGun:

                break;
            default:
                break;
        }
    }
}
