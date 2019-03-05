using UnityEngine;
using TMPro;
using UnityEngine.UI;
//-------------------------------------------------------------------------------------------------------
// Class:       Gun, abstract
// Description: Abstract base class for all guns. Guns can either call the FireWeapon() method contained here
//              Or override with their own version if needs be. This class fires weapons by performing a sphereast
//              And depending on what object was hit, can take damage from enemies and/0r activate the appropriate hit animation
//-------------------------------------------------------------------------------------------------------
public abstract class Gun : MonoBehaviour
{
    // Serialized Fields    
    [SerializeField] protected Image GunCrosshair = null;       // the gun's crosshair
    [SerializeField] protected GameObject MuzzleFlashEffects;   // Muzzle flash effects
    [SerializeField] protected AudioSource ShotSFX = null;      // Audio for shooting
    [SerializeField] protected GameObject EnvHitParticles;      // The particle system for when bullets hit environment
    [SerializeField] protected GameObject TargetHitFX;          // The particle system for when bullets hit targets in start scene
    [SerializeField] protected GameObject BasicEnemyHitFX;      // The particle system for when bullets hit basic enemy etc
    [SerializeField] protected GameObject ArmouredEnemyHitFX;
    [SerializeField] protected GameObject FlameEnemyHitFX;
    [SerializeField] protected GameObject EnergyPackHitFX;
    [SerializeField] protected TextMeshProUGUI _scoreText;      // The game score textMeshProUGUI
    [SerializeField] protected float _spreadAmount;             // The degree to which bullets may stray from forward vector
   
    // Protected Fields:
    protected Image             ActiveCrosshair = null;                               // The currently active crosshair
    protected ParticleSystem[]  MuzzleEffects;                                        // Array of particle effects when shooting
    protected Transform         _envHitEffectPool = null, ShootableHitFXPool = null,
                                MeleeHitFXPool = null, ArmouredHitFXPool = null,
                                FlameHitFXPool = null, EnergyPackHitFXPool = null;    // Parent objects of particle effect pools
    protected int               poolSize = 10;                                        // typical size of a pool
    protected Transform         _launchPoint = null;                                  // Origin of spherecast
    protected Ray               ShotRay;                                              // ray for spherecasting
    protected RaycastHit        ShotHit;                                              // container for what was hit
    protected int               GunEnemyLayerMask;                                    // Layer mask for shooting
    protected TagsHashIDs       TagsHashes;                                           // reference to TagsHashIDs.cs file
    protected int               _bodyShotScore = 10, _headShotScore = 20, 
                                _killScore = 50, _armouredEnemyScore = 100, 
                                _flamerEnemyScore = 150;                              // Possible score values

    // Public Fields:
    [HideInInspector] public float BulletRadius, EffectiveRange, 
                                   NonEffectiveRange, Damage, 
                                   HeadshotDamage, EnergyPackDamage = 25F,
                                   StoppingPower = 5f;                      // Parameters for shooting
    [HideInInspector] public int shotCount;                                 // Gets incremented when weapon is fired
    [HideInInspector] public int MaximumAmmo;                               // maximum possible ammo
    public AudioSource ReloadAudio;                                         // The reload audio source


    //-----------------------------------------------------
    // Method:      Awake()
    // Description: disable the currently active crosshair
    //-----------------------------------------------------
    protected void Awake()
    {
        // Added a conditional to prevent unnessecary calls of this method, it only needs to be called once
        // But depending on which weapon the player first picks up it must be called at least once
        if (_launchPoint != null)
        {
            return;
        }

        _launchPoint = Camera.main.transform;                       // get the launchpoint

        TagsHashes = FindObjectOfType<TagsHashIDs>();               // get the TagsHashIDs.cs script reference

        int EnemyLayerInt = LayerMask.NameToLayer("Enemy");         // Set up the shooting layer mask
        int DefaultLayerInt = LayerMask.NameToLayer("Default");
        int ShootableLayerInt = LayerMask.NameToLayer("Shootable");


        int EnemyLayerMask = 1 << EnemyLayerInt;
        int ShootableLayerMask = 1 << ShootableLayerInt;
        int DefaultLayerMask = 1 << DefaultLayerInt;

        GunEnemyLayerMask = EnemyLayerMask | DefaultLayerMask | ShootableLayerMask; // bit shift the layer mask

        MuzzleEffects = MuzzleFlashEffects.GetComponentsInChildren<ParticleSystem>();// Asigne the muzzle flash effects

        // Create pools of commonly used particle systems:
        _envHitEffectPool    = ObjectPooler.CreatePool(EnvHitParticles, poolSize, gameObject.name + " Hit Particles", false);
        ShootableHitFXPool   = ObjectPooler.CreatePool(TargetHitFX, 3, gameObject.name + "Target Hit FX", false);
        MeleeHitFXPool       = ObjectPooler.CreatePool(BasicEnemyHitFX, poolSize, gameObject.name + " ImpactVFX", false);
        ArmouredHitFXPool    = ObjectPooler.CreatePool(ArmouredEnemyHitFX, poolSize, gameObject.name + " ArmouredImpactVFX", false);
        FlameHitFXPool       = ObjectPooler.CreatePool(FlameEnemyHitFX, poolSize, gameObject.name + " FlamerImpactVFX", false);
        EnergyPackHitFXPool  = ObjectPooler.CreatePool(EnergyPackHitFX, poolSize, gameObject.name + " EnergyPackVFX", false);

    }

    //-----------------------------------------------------
    // Method:      OnDisable()
    // Description: disable the currently active crosshair
    //-----------------------------------------------------
    private void OnDisable()
    {

        if (GunCrosshair != null)
        {
            GunCrosshair.gameObject.SetActive(false);
        }
    }

   
    // ---------------------------------------------------------------------------------------------------
    // METHOD:      FireWeapon
    // DESCRIPTION: This method has an overload method below, which is the primary FireWeapon method.
    //              The overload method is exclusively used in the project because it allows weapons to 
    //              fire with a spread (bullets don't just fire exactly straight). I kept this method in case
    //              removing it lead to errors. No derived classes call this method
    // ----------------------------------------------------------------------------------------------------
    public virtual void FireWeapon(float BulletSize, float Range, float damage, float headshotDamage, int shotCount)
    {
        ShotSFX.pitch = Random.Range(0.85f, 1.2f);
        ShotSFX.Play();
        PlayMuzzleEffects();


        ShotRay.origin = _launchPoint.position;
        ShotRay.direction = _launchPoint.forward;

        if (Physics.SphereCast(ShotRay, BulletSize, out ShotHit, Range, GunEnemyLayerMask))
        {
            Collider hitCol = ShotHit.collider;
            if (hitCol != null)
            {
                if (Vector3.Distance(ShotRay.origin, hitCol.transform.position) <= EffectiveRange) // is our target in effective range distance:
                {
                    if (hitCol.CompareTag(TagsHashIDs.MeleeEnemy)) // We hit a melee enemy:
                    {
                        HitEnemy(hitCol, MeleeHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnemyHead)) // We headshot a melee enemy:
                    {
                        HeadshotEnemy(hitCol, MeleeHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
                    {
                        HitEnemy(hitCol, ArmouredHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
                    {
                        HitEnemy(hitCol, FlameHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
                    {
                        HitEnemy(hitCol, EnergyPackHitFXPool, true);
                    }
                    else // No enemy was hit; we hit an object in environment:
                    {
                        Transform hitEffect = ObjectPooler.UseObject(_envHitEffectPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                    }
                }
                // Now check if we hit a target outside effective range:
                else if (Vector3.Distance(ShotRay.origin, hitCol.transform.position) > EffectiveRange)
                {
                    if (hitCol.CompareTag(TagsHashIDs.MeleeEnemy)) // We hit a melee enemy:
                    {
                        HitEnemy(hitCol, MeleeHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnemyHead)) // We headshot a melee enemy:
                    {
                        HeadshotEnemy(hitCol, MeleeHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
                    {
                        HitEnemy(hitCol, ArmouredHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
                    {
                        HitEnemy(hitCol, FlameHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
                    {
                        HitEnemy(hitCol, EnergyPackHitFXPool, false);
                    }
                    else // No enemy was hit; we hit an object in environment:
                    {
                        Transform hitEffect = ObjectPooler.UseObject(_envHitEffectPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                    }
     
                }
                
            }
        }

    }
    // ----------------------------------------------------------------------------------------------------------
    // METHOD:      FireWeapon, Overload method
    // DESCRIPTION: Base virtual function for all ballistic weaponry, extension of FireWeapon() taking
    //              an extra float parameter to define the max "spread" of a weapon - how far bullets might stray
    //              from straight path. each weapon calls this method, no weapons call the above method anymore
    // -----------------------------------------------------------------------------------------------------------
    public virtual void FireWeapon(float BulletSize, float Range, float damage, float headshotDamage, int shotCount, float maxSpreadDegrees)
    {
        // Play the audio and muzzle flash effects:
        ShotSFX.pitch = Random.Range(0.85f, 1.2f);
        ShotSFX.Play();
        PlayMuzzleEffects();

        // Perform a spherecast with this guns unique parameters passed in:
        ShotRay.origin = _launchPoint.position;
        ShotRay.direction = _launchPoint.forward; // shoot straight out from camera
        float randomAngle = Random.Range(0, maxSpreadDegrees); // The range which a bullet may stray from the forward vector
        Vector3 RandomAxis = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)); // Random vector axis to rotate around
        Quaternion randomRotationShift = Quaternion.AngleAxis(randomAngle, RandomAxis); // A randomized quaternion value created using our randomAngle for degrees and randomAxis 
        ShotRay.direction = randomRotationShift * ShotRay.direction; // Multiply the Ray direction by the newly created quaternion to manufacture bullet spread

        if (Physics.SphereCast(ShotRay, BulletSize, out ShotHit, Range, GunEnemyLayerMask))
        {

            Collider hitCol = ShotHit.collider;

            if (hitCol != null)
            {
                // First check is our target in effective range distance:
                if (Vector3.Distance(ShotRay.origin, hitCol.transform.position) <= EffectiveRange) 
                {
                    //Then compare the tag to find what to do next:
                    if (hitCol.CompareTag(TagsHashIDs.MeleeEnemy)) // We hit a melee enemy:
                    {
                        HitEnemy(hitCol, MeleeHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnemyHead)) // We headshot a melee enemy:
                    {
                        HeadshotEnemy(hitCol, MeleeHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
                    {
                        HitEnemy(hitCol, ArmouredHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
                    {
                        HitFlamerArmour(hitCol, FlameHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
                    {
                        HitEnemy(hitCol, EnergyPackHitFXPool, true);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.Destructible)) // we hit a target in the start scene:
                    {
                        Transform hitEffect = ObjectPooler.UseObject(ShootableHitFXPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                        hitCol.GetComponent<Target>().GetShot();
                    }
                    else // No enemy was hit; we hit an object in environment:
                    {
                        Transform hitEffect = ObjectPooler.UseObject(_envHitEffectPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                    }
                }
                // Now check if we hit a target outside effective range:
                else if (Vector3.Distance(ShotRay.origin, hitCol.transform.position) > EffectiveRange)
                {
                    if (hitCol.CompareTag(TagsHashIDs.MeleeEnemy))      // We hit a melee enemy:
                    {
                        HitEnemy(hitCol, MeleeHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnemyHead))   // We headshot a melee enemy:
                    {
                        HeadshotEnemy(hitCol, MeleeHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
                    {
                        HitEnemy(hitCol, ArmouredHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
                    {
                        HitFlamerArmour(hitCol, FlameHitFXPool, false);
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
                    {
                        HitEnemy(hitCol, EnergyPackHitFXPool, false); // Bullets deal normal damage to energy pack
                    }
                    else if (hitCol.CompareTag(TagsHashIDs.Destructible))
                    {
                        Transform hitEffect = ObjectPooler.UseObject(ShootableHitFXPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                        hitCol.GetComponent<Target>().GetShot();
                    }
                    else // No enemy was hit; we hit an object in environment:
                    {
                        Transform hitEffect = ObjectPooler.UseObject(_envHitEffectPool, ShotHit.point, true);
                        hitEffect.rotation = Quaternion.LookRotation(hitEffect.forward, ShotHit.normal);
                    }
                }

            }
        }

    }

    //-----------------------------------------------------------------------------------------------------
    // Method: HitEnemy()
    // Desc:   Takes health away from EnemyLifeCycle.cs, triggers enemy to be killed if applicable
    //         Uses whichever particleEffect for the hit animation that we pass in. This is 
    //         decided in conditional logic of FireWeapon(). Also handles shooting outside effective range
    //-----------------------------------------------------------------------------------------------------
    protected void HitEnemy(Collider hitCol, Transform particleEffect, bool insideEffectiveRange)
    {
        EnemyLifecycle EnemyLifeScript = hitCol.GetComponentInParent<EnemyLifecycle>();

        if (insideEffectiveRange) // we have shot in the effective range:
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;

                EnemyLifeScript.TakeDamage(Damage, direction, StoppingPower, hitCol, EnemyLifecycle.ScoreAmount.bodyShotScore);

            }
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddRelativeForce(direction * StoppingPower, ForceMode.VelocityChange);
                }
            }
        }
        else // we have hit something in the non-effective range:
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                // deal one tenth of possible damage and half of the possible stopping power
                // however still get full point value for a succesful hit:
                EnemyLifeScript.TakeDamage(Damage * 0.1f, direction, StoppingPower * 0.5f, hitCol, EnemyLifecycle.ScoreAmount.bodyShotScore);
;
            }
            // If we are shooting a corpse on the ground, addRelativeForce to move it a bit:
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddRelativeForce(direction * StoppingPower, ForceMode.VelocityChange);
                }
            }
        }
            // Use the particle effect from the pool:
            Transform hitEffect = ObjectPooler.UseObject(particleEffect, ShotHit.point, true);
            hitEffect.rotation = Quaternion.LookRotation(ShotHit.normal);
    }

    //--------------------------------------------------------------------------------------------
    // Method: HeadshotEnemy()
    // Desc:   Takes health away from EnemyLifeCycle.cs, triggers enemy to be killed if applicable
    //         Uses whichever particleEffect for the hit animation that we pass in. This is 
    //         decided in conditional logic of FireWeapon(). Adds a headshot score to score counter
    //         Tester feedback said it was too easy to get headshots with the pistol from far away
    //         So halve headshot damage if in non-efffective range
    //---------------------------------------------------------------------------------------------
    protected void HeadshotEnemy(Collider hitCol, Transform particleEffect,bool insideEffectiveRange)
    {
        EnemyLifecycle EnemyLifeScript = hitCol.GetComponentInParent<EnemyLifecycle>();
        if (insideEffectiveRange)
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                EnemyLifeScript.TakeDamage(HeadshotDamage, direction, StoppingPower, hitCol, EnemyLifecycle.ScoreAmount.HeadShotScore);
            }
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddRelativeForce(direction * StoppingPower, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                EnemyLifeScript.TakeDamage(HeadshotDamage * 0.25f, direction, StoppingPower, hitCol, EnemyLifecycle.ScoreAmount.HeadShotScore);
            }
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddRelativeForce(direction * StoppingPower, ForceMode.VelocityChange);
                }
            }
        }

        Transform hitEffect = ObjectPooler.UseObject(particleEffect, ShotHit.point, true);
        hitEffect.rotation = Quaternion.LookRotation(ShotHit.normal);
    }

    //-----------------------------------------------------------------------------------------------------
    // Method: HitFlamerArmour()
    // Desc:   Same code as in HitEnemy(), but as the flamer enemy wears heavy armour, bullets may not
    //         penetrate it and so deal very little damage. Damage can only really be dealt to flamer enemy
    //         by hitting his energy pack
    //-----------------------------------------------------------------------------------------------------
    protected void HitFlamerArmour(Collider hitCol, Transform particleEffect, bool insideEffectiveRange)
    {
        EnemyLifecycle EnemyLifeScript = hitCol.GetComponentInParent<EnemyLifecycle>();
        if (insideEffectiveRange)
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                EnemyLifeScript.TakeDamage(Damage * 0.05f, direction, StoppingPower, hitCol);
            }
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(direction * StoppingPower);
                }
            }
        }
        else
        {
            if (EnemyLifeScript.EnemyHealth > 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                EnemyLifeScript.TakeDamage(Damage * 0.05f, direction, StoppingPower, hitCol);
            }
            else if (EnemyLifeScript.EnemyHealth < 0)
            {
                Vector3 direction = EnemyLifeScript.transform.position - transform.position;
                Rigidbody rb = hitCol.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(direction * StoppingPower);
                }
            }
        }

        Transform hitEffect = ObjectPooler.UseObject(particleEffect, ShotHit.point, true);
        hitEffect.rotation = Quaternion.LookRotation(ShotHit.normal);
    }

    //---------------------------------------------------
    // Method: PlayMuzzleEffects()
    // Desc:   plays muzzle flash effect particle systems
    //---------------------------------------------------
    protected void PlayMuzzleEffects()
    {
        foreach (ParticleSystem ps in MuzzleEffects)
        {
            ps.Play();
        }
    }

    //---------------------------------------------------------------------
    // Method: UpdateAmmoCount()
    // Desc:   Updates the ammo display on the gun's world-space canvas. can
    //         be called from FPSRiganimator script
    //---------------------------------------------------------------------
    public void UpdateAmmoCount(Text ammoText, int currentAmmo, int MagazineCapacity)
    {
        ammoText.text = (MagazineCapacity - shotCount).ToString() + "/" + currentAmmo.ToString();
    }

    //---------------------------------------------------------------------
    // Method: ActivateCrosshair()
    // Desc:   Activates the gun's crosshair. Called in the OnEnable()
    //         method of a derived class, because the crosshair should switch 
    //         when we switch weapons
    //---------------------------------------------------------------------
    protected void ActivateCrosshair()
    {
        // If there is no currently active crosshair:
        if (ActiveCrosshair == null)
        {
            GunCrosshair.gameObject.SetActive(true);
            ActiveCrosshair = GunCrosshair;
        }
        else if (ActiveCrosshair.gameObject == GunCrosshair.gameObject)
        {
        }
        else if (ActiveCrosshair.gameObject != GunCrosshair.gameObject)
        {
            GunCrosshair.gameObject.SetActive(true);
            ActiveCrosshair.gameObject.SetActive(false);
            ActiveCrosshair = GunCrosshair;
        }
    }
}
