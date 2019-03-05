using UnityEngine;
 
// ---------------------------------------------------------------------------------
// CLASS:       EnemyGun
// DESCRIPTION: This class is responsible for performing a spherecast from enemies gun to the player
//              As well as all associated SFX and VFX
// ---------------------------------------------------------------------------------
public class EnemyGun : MonoBehaviour
{


    [SerializeField] private LayerMask _LayerMask;

    private Ray _shotRay = new Ray();
    private RaycastHit _shotHit;
    public float _shotDamage = 25f;
    private float _range = 15f; // This is the radius of the projectile range trigger collider attached to the player game object
    private Transform _playerTransform;
    private ParticleSystem[] _shotParticles = new ParticleSystem[3];
    private AudioSource _gunshotAudio;
    private TagsHashIDs _tagsHashes;
    private int _layerMask;
    public GameObject OrganicImpactEffect; // Assigned in inspector
    public GameObject NonOrganicImpactEffect; // The enemies rate of fire is generally slow, so object pooling is not necessary
    public Transform EnemyHead; //  I previously would fire the gun from the end of the barral, but this lead to unwanted behaviour
    // Because in a fire animation, the enemy can raise his arm beside a piece of level geometry like a pillar, and his arm is long enough so
    // that the gun barrel pokes out the other side - meaning an enemy can shoot the player through a wall or pillar in some edge cases
    // I cast the spherecast from the enemies head to avoid this glitch

    private EnemySight _enemySightScript; // I access the sight script to the controller -- if the enemy shoots and hits the player,
    // but cannot see the player, it means that the barrel of the gun is poking through some level geometry while the enemy is behind it
    // So we cannot reasonably take damage from the player in that edge case

    //-----------------------------------------------------------------------------------
    // Method: Awake
    // Desc:   Used to cache component references in this object's heirarchy
    //-----------------------------------------------------------------------------------
    private void Awake()
    {
        _shotParticles = GetComponentsInChildren<ParticleSystem>(true);
        _gunshotAudio = GetComponent<AudioSource>();
        _enemySightScript = GetComponentInParent<StateController>().GetComponentInChildren<EnemySight>();    
    }

    //-----------------------------------------------------------------------------------
    // Method: Start
    // Desc:   Create the layer mask. It is possible for ranged enemies to fire a spherecast
    //         and hit other enemies, which takes damage from them. Also cache comps not 
    //         in this object's heirarchy
    //-----------------------------------------------------------------------------------
    private void Start()
    {
        StateController sc = GetComponentInParent<StateController>();
        _playerTransform = sc.Player.transform;
        _tagsHashes = sc.TagsHash;
        _layerMask = _tagsHashes.PlayerLayerMask | _tagsHashes.EnemyLayerMask | _tagsHashes.DefaultLayerMask | _tagsHashes.ShootableLayerMask;
    }

    //------------------------------------------------------------------------------------------
    // Method: FireGun
    // Desc:   Called from a method in the EnemyAnimation.cs script, which is an animation event 
    //------------------------------------------------------------------------------------------
    public void FireGun()
    {
        foreach (ParticleSystem ps in _shotParticles)
        {
            ps.Play();
        }
        _gunshotAudio.pitch = Random.Range(0.85f, 1.20f);
        _gunshotAudio.Play();
        _shotRay.origin = transform.position;
        _shotRay.direction = _playerTransform.position - _shotRay.origin;
       
        if (Physics.SphereCast(_shotRay, 0.15f, out _shotHit, _range, _layerMask))
        {
            Collider hitCol = _shotHit.collider;
            //I verify that there is a hit before the next line to avoid a null reference exception
            if (hitCol != null)
            {

                if (hitCol.CompareTag(TagsHashIDs.Player))
                {
                    if (_enemySightScript.PlayerInSight) // If the player is not in sight we do not consider it a hit, this is to prevent the edge case that armoured enemies can shoot through walls
                    {
                        hitCol.GetComponent<PlayerLifecycle>().TakeDamage(_shotDamage);
                        OrganicImpactEffect.transform.SetParent(null);
                        OrganicImpactEffect.transform.position = _shotHit.point;
                        OrganicImpactEffect.SetActive(true);
                    }

                }
                else if (hitCol.CompareTag(TagsHashIDs.Enemy)) // enemies can shoot other enemies and take damage
                {
                    hitCol.GetComponent<EnemyLifecycle>().TakeDamage(_shotDamage);
                    OrganicImpactEffect.transform.SetParent(null);
                    OrganicImpactEffect.transform.position = _shotHit.point;
                    OrganicImpactEffect.SetActive(true);
                }
                else
                {
                    NonOrganicImpactEffect.transform.SetParent(null);
                    NonOrganicImpactEffect.transform.position = _shotHit.point;
                    NonOrganicImpactEffect.SetActive(true);
                }
            }
        }
    }

    public void ChargeGun()
    {

    }

}
