using UnityEngine;
using UnityEngine.UI;
//-------------------------------------------------------------------------------------------------------
// Class:       Handgun : Gun(abstract)
// Description: This script is the player's Handgun object. It calls the FireWeapon() method in the base class,
//              passing in it's own parameters, that make it different from the shotgun and plasma rifle
//-------------------------------------------------------------------------------------------------------
public class Handgun : Gun
{

    [HideInInspector] public Text _ammoText;            // Reference to the ammo text on world space ammo canvas, displays ammo to user
    [HideInInspector] public int CurrentAmmo = 16;      // Initialize at 16, if we begin with the pistol
    [HideInInspector] public int MagazineCapacity = 8;  // Have to keep the mag capacity as an instance variable for each derived gun, otherwise the pickup.cs code leads to runtime errors

    [SerializeField] private ParticleSystem _gunChargeUp; // Instance particle system of the handung, charge up particle systems

    //-----------------------------------------------------------
    // Method:      Awake()
    // Description: Calls the base awake method and gets a ref
    //              to the ammo display text
    //-----------------------------------------------------------
    private new void Awake()
    {
        base.Awake();
        _ammoText = GetComponentInChildren<Text>(true);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Method:      OnEnable()
    // Description: Here we update the firing parameters of this gun, for instance the size of the bullet, the weapon's 
    //              effective and non effective range, headshot damage etc
    // -----------------------------------------------------------------------------------------------------------------
    private void OnEnable()
    {

        BulletRadius = 0.15f;                                       // size of bullet
        EffectiveRange = 25f;                                       // effective range
        NonEffectiveRange = 40.0f;                                  // non-effective range
        Damage = 10f;                                               // typical damage
        HeadshotDamage = 200f;                                      // typical headshot damage
        MagazineCapacity = 8;                                       // how many bullets a clip can hold
        MaximumAmmo = MagazineCapacity * 6;
        StoppingPower = 3.25f;                                      // How much force we can apply to a ragdolled enemy
        _spreadAmount = 2.0f;                                       // degree to which bullets may spread
        GunCrosshair.gameObject.SetActive(true);                    // Activate this weapons gun crosshair when we enable it
        UpdateAmmoCount(_ammoText,CurrentAmmo, MagazineCapacity);   // Update the ammo when we enable the gun
    }

    // -----------------------------------------------------------
    // Method:      ChargeGun
    // Description: Plays charge up particle effects
    // -----------------------------------------------------------
    public void ChargeGun()
    {
        _gunChargeUp.Play();
    }

    // ------------------------------------------------------------------------------------------------------------
    // Method:      FireWeapon
    // Description: Called from the FPSRiganimator.cs file when user resses Fire button.
    //              increments the shotcount, updates the ammo count, and calls on the base class
    //              method to perform the spherecast and take damage from enemy etc, passing in the 
    //              parameters we set in this weapon's OnEnable() method
    // -------------------------------------------------------------------------------------------------------------
    public override void FireWeapon(float bulletSize, float range, float damage, float headshotDamage, int shotCount)
    {
         shotCount++;
        UpdateAmmoCount(_ammoText, CurrentAmmo, MagazineCapacity);
        base.FireWeapon(bulletSize, range, damage, headshotDamage, shotCount, _spreadAmount);
    }

    // --------------------------------------------
    // Method:      Reload
    // Description: manages ammo while reloading.
    // --------------------------------------------
    public void Reload()
    {
        if (CurrentAmmo >= shotCount) // If we have more than one magazine worth of ammo in reserve:
        {
            CurrentAmmo -= shotCount;
            shotCount = 0;
            return;
        }
        else
        {
            for (int i = 0; i < shotCount; i++)
            {
                if (CurrentAmmo <= 0)
                {
                    CurrentAmmo = 0;  // return if we have no ammo in reserve, make sure ammo display does notdisplay a negative integer
                    return;
                }
                else if (shotCount <= 0)
                {
                    return; // Our ammo clip is full
                }
                else if (CurrentAmmo >= shotCount)
                {
                    CurrentAmmo -= shotCount;
                    shotCount = 0;
                    return;
                }
                shotCount--;
                CurrentAmmo--;
            }
        }
    }

    public void ToggleSmokeTrail()
    {
        
    }
}
