using UnityEngine;
using UnityEngine.UI;

//-------------------------------------------------------------------------------------------------------
// Class:       PlasmaRifle : Gun(abstract)
// Description: This script is the player's PlasmaRifle  object. It calls the FireWeapon() method in the base class,
//              passing in it's own parameters, that make it different from the shotgun and handgun
//-------------------------------------------------------------------------------------------------------
public class PlasmaRifle : Gun
{

    [HideInInspector] public Text           _ammoText;
    [HideInInspector] public int            CurrentAmmo;
    [SerializeField] private ParticleSystem _gunChargeUp;
    [SerializeField] private AudioSource    _shotAudio;
    [HideInInspector] public int            MagazineCapacity = 21;

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
        BulletRadius            = 0.1f;
        EffectiveRange          = 35.0f;
        NonEffectiveRange       = 60.0f;
        Damage                  = 20f;
        HeadshotDamage          = 200f;
        StoppingPower           = 4.5f;
        MagazineCapacity        = 21;
        MaximumAmmo             = MagazineCapacity * 3;
        _spreadAmount           = 1.5f;
        UpdateAmmoCount(_ammoText, CurrentAmmo, MagazineCapacity);
        GunCrosshair.gameObject.SetActive(true);
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
}
