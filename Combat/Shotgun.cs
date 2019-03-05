using UnityEngine;
using UnityEngine.UI;

//-------------------------------------------------------------------------------------------------------
// Class:       Shotgun : Gun(abstract)
// Description: This script is the player's Shotgun  object. It calls the FireWeapon() method in the base class,
//              passing in it's own parameters, that make it different from the plasma rifle and handgun
//-------------------------------------------------------------------------------------------------------
public class Shotgun : Gun
{

    public Transform launhPoint; // Origin of bullets, the FPS cam
    public float shotDamage = 5f;
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public Text _ammoText;
    public int MagazineCapacity = 2;
    [SerializeField] ParticleSystem _muzzleFlashAlpha, _muzzleFlashAdditive,
                                    _leftSteam, _rightSteam;
    AudioSource shotAudio;

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
        BulletRadius = 0.2f;
        EffectiveRange = 8f;
        NonEffectiveRange = 40f;
        Damage = 20f;
        HeadshotDamage = 150f;
        StoppingPower = 6.5f; // Stopping Power of weapon
        MagazineCapacity = 2;
        MaximumAmmo = MagazineCapacity * 10;
        _spreadAmount = 8.0f;
        GunCrosshair.gameObject.SetActive(true);
        UpdateAmmoCount(_ammoText, CurrentAmmo, MagazineCapacity);
    }


    // ------------------------------------------------------------------------------------------------------------
    // Method:      FireWeapon
    // Description: Called from the FPSRiganimator.cs file when user resses Fire button.
    //              increments the shotcount, updates the ammo count, and calls on the base class
    //              method to perform the spherecast and take damage from enemy etc, passing in the 
    //              parameters we set in this weapon's OnEnable() method
    //              The shotgun likes to perform four FireWeapon calls at once, to siulate firing buckshot
    //              The shotgun also has a wider range/spread than other weapons
    // -------------------------------------------------------------------------------------------------------------
    public override void FireWeapon(float bulletSize, float range, float damage, float headshotDamage, int shotCount)
    {
        shotCount++;
        UpdateAmmoCount(_ammoText, CurrentAmmo, MagazineCapacity);
        base.FireWeapon(bulletSize, range, damage, headshotDamage, shotCount, _spreadAmount);
        base.FireWeapon(bulletSize, range, damage, headshotDamage, shotCount, _spreadAmount);
        base.FireWeapon(bulletSize, range, damage, headshotDamage, shotCount, _spreadAmount);
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

    // -------------------------------------------------------------------------
    // Method:      VentSteam
    // Description: Vents steam from sized of shotgun after firing
    // -------------------------------------------------------------------------
    public void VentSteam()
    {
        _leftSteam.Play();
        _rightSteam.Play();
    }
}
