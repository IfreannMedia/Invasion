using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//---------------------------------------------------------------------------------------------------------
// CLASS: PlayerMeleeAttack
// DESCRIPTION: Enables player to utilize a melee attack through the OnTriggerEnter method call
//              Deals damage and force, which could be increased if the player earned an upgrade
//              Object is on AITrigger layer because it should only collide with objects on the Enemy layer
//----------------------------------------------------------------------------------------------------------
public class PlayerMeleeAttack : MonoBehaviour {

    public float Damage = 5.0f;
    public float force = 5.0f;
    //private int _punchScore = 5;
    private ParticleSystem _impactEffect;
   // private AudioSource meleeAudio;
    private TagsHashIDs tagsHash;
    private Transform meleeEffectPool;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        _impactEffect = GetComponentInChildren<ParticleSystem>();
       // meleeAudio = _impactEffect.GetComponent<AudioSource>();
        tagsHash = GetComponentInParent<FPSRigAnimator>().TagsHashIDs;
        meleeEffectPool = ObjectPooler.CreatePool(_impactEffect.gameObject, 3, "MeleeEffects", false);
    }
    // ---------------------------------------------------------------------------
    // Method: OnTriggerEnter()
    // Desc:   Deals damage and force to enemy, disables collider on succesful hit
    // ---------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        // Because of how tags for enemies work, must use conditional logic
        // to effectively make melee attack work
        // otherwise OnTriggerEnter will return false positives (hitting level geometry or self)
        // This is despite being on a physics layer that should only detect collisions with enemies as set up in project settings
        if (other.CompareTag(TagsHashIDs.MeleeEnemy)) // We hit a melee enemy:
        {
            HitEnemy(other);
        }
        else if (other.CompareTag(TagsHashIDs.EnemyHead)) // We headshot a melee enemy:
        {
            HitEnemy(other);
        }
        else if (other.CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
        {
            HitEnemy(other);
        }
        else if (other.CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
        {
            HitEnemy(other);
        }
        else if (other.CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
        {
            HitEnemy(other);
        }
        else // No enemy was hit; we hit an object in environment:
        {
            return;
        }
        
         // ensure we can only melee attack one enemy at a time
    }


    // ---------------------------------------------------------------------------------------------------
    // Method: OverlapSphereAttack
    // Desc:   I was having buggy results using OnTriggerEnter() to handle melee attacking,
    //         This included not registering what should have been succesful hits, and registering 
    //         TriggerEnter events despite the physics collision matrix settings in the project settings
    // ---------------------------------------------------------------------------------------------------
    public void OverlapSphereAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.0f,tagsHash.EnemyLayerMask,QueryTriggerInteraction.Collide);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag(TagsHashIDs.MeleeEnemy)) // We hit a melee enemy:
            {
                HitEnemy(hitColliders[i]);
                return;
            }
            else if (hitColliders[i].CompareTag(TagsHashIDs.EnemyHead)) // We headshot a melee enemy:
            {
                HitEnemy(hitColliders[i]);
                return;
            }
            else if (hitColliders[i].CompareTag(TagsHashIDs.ArmouredEnemy)) // We hit an armoured enemy:
            {
                HitEnemy(hitColliders[i]);
                return;
            }
            else if (hitColliders[i].CompareTag(TagsHashIDs.FlamerEnemy)) // We hit a flamer enemy on his armour:
            {
                HitEnemy(hitColliders[i]);
                return;
            }
            else if (hitColliders[i].CompareTag(TagsHashIDs.EnergyPack)) // We hit a flamer enemy on his energy pack:
            {
                HitEnemy(hitColliders[i]);
                return;
            }
            else // No enemy was hit; we hit an object in environment:
            {

            }
        }
        
    }

    private void HitEnemy(Collider other)
    {
        //PlayerStats.Score += _punchScore;
        //_scoreText.text = "Score: " + PlayerStats.Score.ToString();
        //Transform particleEffect = ObjectPooler.UseObject(meleeEffectPool.transform, other.ClosestPointOnBounds(transform.position));
        //Transform particleEffect = ObjectPooler.UseObject(meleeEffectPool.transform, other.ClosestPoint(transform.position));
        Transform particleEffect = ObjectPooler.UseObject(meleeEffectPool.transform, _impactEffect.transform.position);

        particleEffect.GetComponent<ParticleSystem>().Play();
        particleEffect.GetComponent<AudioSource>().pitch = Random.Range(0.78f, 1.10f);
        particleEffect.GetComponent<AudioSource>().Play();
        EnemyLifecycle enemyLifeScript = other.GetComponentInParent<EnemyLifecycle>(); // For some reason this will not find the enemyLifecycle script
        
        if (enemyLifeScript)
        {

            if (enemyLifeScript.EnemyHealth > 0.0f)
            {
                Vector3 direction = other.transform.position = transform.position;
                enemyLifeScript.TakeDamage(Damage, direction, force, other,EnemyLifecycle.ScoreAmount.punchScore);
                //enemyLifeScript.GetComponent<Animator>().SetTrigger("KnockBack");
            }

        }
        GetComponent<Collider>().enabled = false;
    }

    private void RepoolEffect(Transform obj, Transform pool)
    {
        ObjectPooler.RePool(obj, pool);
    }
}
