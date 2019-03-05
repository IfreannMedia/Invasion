using UnityEngine;

// ---------------------------------------------------------------------------------
// CLASS:       Pickup
// DESCRIPTION: Class allows player to pickup items - weapons, health and ammo
//              Items are controlled by tags in the unity inspector
//              If the player picks up a weapon and already has it, he gains ammo
// ---------------------------------------------------------------------------------
[RequireComponent(typeof (AudioSource))]
public class Pickup : MonoBehaviour {

    private enum _itemType { Handgun, Shotgun, PlasmaRifle, HealthPickup, AmmoPickup } // The types of pickup
    private _itemType _thisItem;            // Container for this item type
    private GameObject _pickupMesh;         // The object that will be picked up
    private WeaponManager weaponManager;    // Global variable as used by two different methods
    public bool respawn = true;             // Boolean for deciding if pickup should respawn or not
    private AudioSource _pickupAudio;

    
    // ------------------------------------------------------------------------
    // Method:      Awake
    // Description: Get the child and assign the item type
    //              Find out which item this is using tag and switch statement
    // ------------------------------------------------------------------------
    private void Awake()
    {
        _pickupMesh = transform.GetChild(0).gameObject;
        _pickupAudio = GetComponent<AudioSource>();
        switch (transform.tag)
        {
            case TagsHashIDs.HandgunPickup:
                _thisItem = _itemType.Handgun;
                break;
            case TagsHashIDs.ShotgunPickup:
                _thisItem = _itemType.Shotgun;
                break;
            case TagsHashIDs.PlasmaRiflePickup:
                _thisItem = _itemType.PlasmaRifle;
                break;
            case TagsHashIDs.HealthPickup:
                _thisItem = _itemType.HealthPickup;
                break;
            case TagsHashIDs.AmmoPickup:
                _thisItem = _itemType.AmmoPickup;
                break;
            default:
                break;
        }
    }

    // ------------------------------------------------------------------------
    // Method:      OnTriggerEnter
    // Description: Change a value in the player's script depending on which 
    //              item this script is attached to
    // ------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // TODO Add pickup noise
        if (other.CompareTag(TagsHashIDs.Player))
        {
             weaponManager = other.GetComponentInChildren<WeaponManager>();

            switch (_thisItem)
            {
                case _itemType.Handgun:
                    if (weaponManager.HasHandgun)
                    {
                        Handgun playerHandgun = weaponManager.GetComponentInChildren<Handgun>(true);
                        // Check if we have maximum ammo before updating the ammo count and disabling the pickup:
                        if (playerHandgun.CurrentAmmo < playerHandgun.MaximumAmmo)
                        {
                            for (int i = 0; i < playerHandgun.MagazineCapacity * 4; i++) // We gain 4 handgun clips from a pickup:
                            {
                                if (playerHandgun.CurrentAmmo < playerHandgun.MaximumAmmo)
                                {
                                    playerHandgun.CurrentAmmo++;
                                }
                            }

                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                            if (playerHandgun.shotCount >= playerHandgun.MagazineCapacity
                                 && weaponManager.CurrentWeapon == WeaponSet.Handgun)
                            {
                                weaponManager.GetComponent<FPSRigAnimator>().TriggerReload(WeaponSet.Handgun);
                            }
                            DisablePickup();
                        }

                    }
                    else
                    {
                        weaponManager.HasHandgun = true;
                        Handgun playerHandgun = weaponManager.GetComponentInChildren<Handgun>(true);
                        if (weaponManager.GetComponent<FPSRigAnimator>().CanFire) // Only switch to the weapon automatically if we are not in reload animation
                            weaponManager.SwitchWeapon(WeaponSet.Handgun);
                        playerHandgun.CurrentAmmo = playerHandgun.MagazineCapacity * 4;
                        if (weaponManager.CurrentWeapon == WeaponSet.Handgun)
                        {
                            // Only update the ammo display if that weapon is selected
                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                        }
                        DisablePickup();
                    }

                    break;

                case _itemType.Shotgun:
                    if (weaponManager.HasShotgun)
                    {
                        
                        Shotgun playerShotgun = weaponManager.GetComponentInChildren<Shotgun>(true);
                        if (playerShotgun.CurrentAmmo < playerShotgun.MaximumAmmo)
                        {
                            for (int i = 0; i < playerShotgun.MagazineCapacity * 4; i++)
                            {
                                if (playerShotgun.CurrentAmmo < playerShotgun.MaximumAmmo)
                                {
                                    playerShotgun.CurrentAmmo++;
                                }
                            }

                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                            if (playerShotgun.shotCount >= playerShotgun.MagazineCapacity
                                 && weaponManager.CurrentWeapon == WeaponSet.Shotgun)
                            {
                                weaponManager.GetComponent<FPSRigAnimator>().TriggerReload(WeaponSet.Shotgun);
                            }
                            DisablePickup();
                        }

                    }
                    else
                    {
                        weaponManager.HasShotgun = true;
                        Shotgun playerShotgun = weaponManager.GetComponentInChildren<Shotgun>(true);
                        if (weaponManager.GetComponent<FPSRigAnimator>().CanFire) // Only switch to the weapon automatically if we are not in reload animation
                            weaponManager.SwitchWeapon(WeaponSet.Shotgun);
                        playerShotgun.CurrentAmmo = playerShotgun.MagazineCapacity * 4;
                        if (weaponManager.CurrentWeapon == WeaponSet.Shotgun)
                        {
                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                        }
                        DisablePickup();
                    }

                    break;

                case _itemType.PlasmaRifle:

                    if (weaponManager.HasPlasmaRifle)
                    {
                        PlasmaRifle playerPlasmaRifle = weaponManager.GetComponentInChildren<PlasmaRifle>(true);
                        if (playerPlasmaRifle.CurrentAmmo < playerPlasmaRifle.MaximumAmmo)
                        {
                            for (int i = 0; i < playerPlasmaRifle.MagazineCapacity * 2; i++)
                            {
                                if (playerPlasmaRifle.CurrentAmmo < playerPlasmaRifle.MaximumAmmo)
                                {
                                    playerPlasmaRifle.CurrentAmmo++;
                                }
                            }

                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                            if (playerPlasmaRifle.shotCount >= playerPlasmaRifle.MagazineCapacity
                                && weaponManager.CurrentWeapon == WeaponSet.PlasmaRifle)
                            {
                                weaponManager.GetComponent<FPSRigAnimator>().TriggerReload(WeaponSet.PlasmaRifle);
                            }
                            DisablePickup();
                        }

                    }
                    else
                    {
                        weaponManager.HasPlasmaRifle = true;
                        PlasmaRifle playerPlasmaRifle = weaponManager.GetComponentInChildren<PlasmaRifle>(true);
                        if (weaponManager.GetComponent<FPSRigAnimator>().CanFire) // Only switch to the weapon automatically if we are not in reload animation
                            weaponManager.SwitchWeapon(WeaponSet.PlasmaRifle);
                        playerPlasmaRifle.CurrentAmmo = playerPlasmaRifle.MagazineCapacity * 2;
                        if (weaponManager.CurrentWeapon == WeaponSet.PlasmaRifle)
                        {
                            weaponManager.GetComponent<FPSRigAnimator>().UpdateAmmoDisplay();
                        }
                        DisablePickup();
                    }

                    break;

                case _itemType.HealthPickup:
                    // Check if the player can gain health
                    if (PlayerStats.PlayerHealth < 100.0f)
                    {
                        other.GetComponent<PlayerLifecycle>().GainHealth(15f);
                        DisablePickup();
                    }
                    break;

                case _itemType.AmmoPickup:
                    // Ammo pickup is handled if player picks up a weapon type but already has it
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }


    private void DisablePickup()
    {
        GetComponent<ParticleSystem>().Stop();
        GetComponent<Collider>().enabled = false;
        _pickupMesh.SetActive(false);
        _pickupAudio.PlayOneShot(_pickupAudio.clip);

        if (respawn)
        {
            float randomRepeatRate = Random.Range(0.0f, 5.0f); // Stagger the repeated invoke call
            InvokeRepeating("EnablePickup", 60.0f, randomRepeatRate); // Minimum repeat/respawn rate is 60 seconds
            // Invoke("EnablePickup", 60.0f); // respawn health pickup after 60 seconds if boolean is true
        }
        else
        {
            Destroy(this); // remove obj from game if not to respawn
        }
    }

    private void EnablePickup()
    {
        switch (_thisItem)
        {
            case _itemType.Handgun:
                Handgun playerHandgun = weaponManager.GetComponentInChildren<Handgun>(true);
                if (playerHandgun.CurrentAmmo < playerHandgun.MagazineCapacity+1) // Check if we have less than 1 clip left, if so then respawn ammo, add +1 in case user is running around with one magazine left in ammo reserve
                {
                    GetComponent<ParticleSystem>().Play();
                    GetComponent<Collider>().enabled = true;
                    _pickupMesh.SetActive(true);
                    CancelInvoke("EnablePickup"); // Make sure to cancel the invoke
                }
                break;
            case _itemType.Shotgun:
                Shotgun playerShotgun = weaponManager.GetComponentInChildren<Shotgun>(true);
                if (playerShotgun.CurrentAmmo < playerShotgun.MagazineCapacity * 2) // Check if we have less than 1 clip left, if so then respawn ammo
                {
                    GetComponent<ParticleSystem>().Play();
                    GetComponent<Collider>().enabled = true;
                    _pickupMesh.SetActive(true);
                    CancelInvoke("EnablePickup");
                }
                break;
            case _itemType.PlasmaRifle:
                PlasmaRifle playerPlasmaRifle = weaponManager.GetComponentInChildren<PlasmaRifle>(true);
                if (playerPlasmaRifle.CurrentAmmo < playerPlasmaRifle.MagazineCapacity) // Check if we have less than 1 clip left, if so then respawn ammo
                {
                    GetComponent<ParticleSystem>().Play();
                    GetComponent<Collider>().enabled = true;
                    _pickupMesh.SetActive(true);
                    CancelInvoke("EnablePickup");
                }
                break;
            case _itemType.HealthPickup:
                if (PlayerStats.PlayerHealth <=30.0f)
                {
                    GetComponent<ParticleSystem>().Play();
                    GetComponent<Collider>().enabled = true;
                    _pickupMesh.SetActive(true);
                }

                break;
            case _itemType.AmmoPickup:
                break;
            default:
                break;
        }
    }

    // Rotate the item to be picked up
    private void Update()
    {
        transform.Rotate(0.0f, 4.0f, 0.0f, Space.World);
    }
}
