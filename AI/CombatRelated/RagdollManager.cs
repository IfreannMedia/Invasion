using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//------------------------------------------------------------------------------------------------------------------------
// CLASS:       RagdollManager
// Description: Controlls the ragdolling effect on enemies. Can perform a simple ragdoll, or ragdoll an enemy with a force
//              and random torque applied to a specific rigidbody in the heirarchy
//-------------------------------------------------------------------------------------------------------------------------
public class RagdollManager : MonoBehaviour
{

    private Rigidbody[]         _rigidbodies;
    private Animator            _anim;
    private NavMeshAgent        _navAgent;
    private StateController     _stateController;
    private CapsuleCollider     _mainCol;
    private EnemyAnimation      _enemyAnimationScript;
    private SkinnedMeshRenderer _bodyMesh;
    

    public MeshFilter           DeathMesh;
    public ParticleSystem        DeathParticles;


    //-----------------------------------------------------------------------------------
    // Method: Awake
    // Desc:   Used to cache component references in this object's heirarchy
    //-----------------------------------------------------------------------------------
    private void Awake()
    {
        _mainCol = GetComponent<CapsuleCollider>();
        _navAgent = GetComponent<NavMeshAgent>();
        _stateController = GetComponent<StateController>();
        _anim = GetComponent<Animator>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        _bodyMesh = GetComponentInChildren<SkinnedMeshRenderer>(); // Get a reference to our skinned mesh renderer
        _enemyAnimationScript = GetComponent<EnemyAnimation>();

        //Activate the capsule collider used for this enemies energypack - only relevant for flametrhower enemies
        foreach (CapsuleCollider capCol in GetComponentsInChildren<CapsuleCollider>(true))
        {
            if (capCol.CompareTag(TagsHashIDs.EnergyPack))
            {
                capCol.enabled = true;
                return;
            }
        }
    }


    //-----------------------------------------------------------------------------------
    // Method: OnEnable
    // Desc:   Couldn't figure out to get regdolled enemies to properly work with object pooling
    //         so instead of pooling enemies they are instantiaed throughout play
    //-----------------------------------------------------------------------------------
    private void OnEnable()
    {

        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    //-------------------------------------------------------------------------------------------------
    // Method: EnableRagdoll
    // Desc:   Turns off the flame particle effect and hit collider if this is the flamer enemy
    //         disables AI components like nav agent and StateController.cs
    //         turns all collider rigidbodies to be non-kinematic and disable the main capsule col
    //         Start the body disposal after 5 seconds (should be enough time for the body to lie still
    //--------------------------------------------------------------------------------------------------
    public void EnableRagdoll()
    {
        // Conditional logic to disable flame attack of flamer enemy when he is ragdolled:
        if (_enemyAnimationScript._leftHandFlamer || _enemyAnimationScript._rightHandFlamer)
        {
            _enemyAnimationScript.ToggleFlameAttackOff(2);
        }
        _anim.enabled = false;
        
        _stateController.enabled = false;
        _navAgent.enabled = false;
        _enemyAnimationScript.enabled = false;


        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = false;
        }
         _mainCol.enabled = false;
        // If this is a flamer enemy we want to disable all his particle effects upon ragdolling:
        if (GetComponent<StateControllerRanged>())
        {
            foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

        }
        Invoke("EmitParticlesFromMesh", 5.0f);
    }


    //-------------------------------------------------------------------------------------------------
    // Method: EnableRagdoll, overload I
    // Desc:   Ragdolls the enemy with a specific level of force applied in a general direction
    //--------------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables the Ragdoll while also adding a force to the main Rigidbody attached to the enemy
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public void EnableRagdoll(Vector3 direction, float force)
    {
        // Conditional logic to disable flame attack of flamer enemy when he is ragdolled:
        if (_enemyAnimationScript._leftHandFlamer || _enemyAnimationScript._rightHandFlamer)
        {
            _enemyAnimationScript.ToggleFlameAttackOff(2);
        }
        _anim.enabled = false;
        _stateController.enabled = false;
        _navAgent.enabled = false;
        _enemyAnimationScript.enabled = false;


        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = false;
            if (rb.GetComponent<BoxCollider>())
            {

                rb.AddForce(direction * force, ForceMode.Force);
            }
        }

        _mainCol.enabled = false;
        Invoke("EmitParticlesFromMesh", 5.0f);

    }


    //-------------------------------------------------------------------------------------------------
    // Method: EnableRagdoll, overload II
    // Desc:   Ragdolls the enemy with a specific level of force applied to a specific collider rigidbody
    //--------------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a ragdoll effect, and adds force to the colliders rigidbody if one is present
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    /// <param name="hitCol"></param>
    public void EnableRagdoll(Vector3 direction, float force, Collider hitCol)
    {
        // Conditional logic to disable flame attack of flamer enemy when he is ragdolled:
        if (_enemyAnimationScript._leftHandFlamer || _enemyAnimationScript._rightHandFlamer)
        {
            _enemyAnimationScript.ToggleFlameAttackOff(2);
        }

        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = false;
        }

        _anim.enabled = false;
        _stateController._aiActive = false;
        _navAgent.enabled = false;
        _enemyAnimationScript.enabled = false;


        Rigidbody rigidBod = hitCol.GetComponent<Rigidbody>();
        // apply force and random torgue to the rigidbody collider:
        if (rigidBod)
        {
            rigidBod.AddForce(direction * force, ForceMode.VelocityChange);
            Vector3 randomTorque = new Vector3(Random.Range(-360.0f, 360.0f), Random.Range(-360.0f, 360.0f), Random.Range(-360.0f, 360.0f));
            rigidBod.AddTorque(randomTorque * force, ForceMode.VelocityChange);
        }
        _mainCol.enabled = false;

        Invoke("EmitParticlesFromMesh", 5.0f); // Simply invoke after 5 seconds - testing for the velocity.magnitude of each rigidbody until
        // disposing of it is unreliable, as sometimes ragdolling leads to unpredictable effects, where the rigidbodies of a limb may shake and jitter a lot
        // Therefore their velocity.magnitude never dropping below a test threshold
    }

    //-------------------------------------------------------------------------------------------------
    // Method: EmitParticlesFromMesh()
    // Desc:   Each enemy has a cube childed to it that has it's mesh renderer turned off. When an enemy is ragdolled and 
    //         we want to dispose of the body, we take a snapshot of the skinned mesh renderer and bake it to the cube
    //         then we emit particles from the baked mesh, and fade the material to transparent. I do this because 
    //         for some reason emmitin particles from the enemies skinned mesh renderer did not work, probably has to do 
    //         with how the mesh was created in blender/imported
    //--------------------------------------------------------------------------------------------------
    private void EmitParticlesFromMesh()
    {
        // asign rotation and position
        DeathMesh.transform.position = transform.position;
        DeathMesh.transform.rotation = transform.rotation;
        _bodyMesh.BakeMesh(DeathMesh.mesh);                 // Bake the skinned mesh renderer to the death mesh object
        DeathParticles.Play();                              // Start plaing the particle effect
        // the Deathparticles are set to emit from the cube mesh, which has been baked to a snapshot of the skinned mesh

        if (!GetComponent<StateControllerRanged>())
        {
            DeathMesh.GetComponent<MeshRenderer>().enabled = true; // Activate the baked mesh rendere if it is not a ranged enemy
            _bodyMesh.enabled = false; // Deactivate the skinned mesh
            // the ranged enemies contain submeshes, and so baking these skinned meshes to static meshes lead to undesireable results
        }
        // freeze the body to prevent twitching and erratic movement after the enemy has died:
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = true;
        }

        StartCoroutine(DestroyBody());
    }

    //-------------------------------------------------------------------------------------------------
    // Method: DestroyBody()
    // Desc:   fades the material from opaque to transparent if we have a melee enemy. If we have a ranged enemy, 
    //         Simply wait and keep checking until it is not visible, then destroy it
    //--------------------------------------------------------------------------------------------------
    private IEnumerator DestroyBody()
    {
        yield return new WaitForSeconds(3.5f);

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        
        // Because of differences in enemy meshes, each one must be faded out differently:
        if (GetComponent<StateControllerRanged>())
        {
            // I tried mesh baking and fading a multitude of ways but it always failed
            // so wait until the object is not visible instead:
            bool candestroy = false;
            while (!candestroy)
            {

                yield return new WaitForEndOfFrame();
                if (!_bodyMesh.isVisible)
                {
                    candestroy = true;

                }
            }
    }
        // Otherwise we have a basic melee enemy, who only needs to have one colour value lerped,
        // And whose skinned mesh renderer does not cause issues when baked to a static mesh:
        else
        {
            Renderer meshRend = DeathMesh.GetComponent<Renderer>();
            Color bodyColour = DeathMesh.GetComponent<Renderer>().material.color;
            Color transparent = new Color(bodyColour.r, bodyColour.g, bodyColour.b, 0);
            while (bodyColour.a > 0.0f)
            {
                meshRend.material.color = Color.Lerp(meshRend.material.color, transparent, 0.1f);
                yield return new WaitForSeconds(0.05f); // fade material out
            }
        }
        DeathParticles.Stop();
        DisposeBody();
    }

    //----------------------------------
    // Method: DisposeBody()
    // Desc:   Destroys the gameobject
    //----------------------------------
    private void DisposeBody()
    {
        //_bodyMesh.BakeMesh
        Destroy(this.gameObject);
    }

}
