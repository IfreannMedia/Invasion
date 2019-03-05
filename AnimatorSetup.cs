using UnityEngine;
// -----------------------------------------------------------------------------------------------------------------------------------------
// CLASS: AnimatorSetup
// Desc:  Helps to set up enemy animation by passing vertical and angular velocity parametersto the animator controller.
//        It also applies damping to avoid erratic movement when the agent is turning.
//        The code is adopted from the unity stealth game tutorial, which is no longer on their website, but can be found
//        on their youtube channel. I chose to implemente it after a few failed iterations at properly coupling enemy navigation and animation
// --------------------------------------------------------------------------------------------------------------------------------------------
public class AnimatorSetup
{
    // Floats to control the damping of angular speed and speed, VelX and VelY params in anim controller respectively
    public float SpeedDampTime = 0.1f, AngularSpeedDampTime = 0.7f;
    public float AngleResponseTime = 0.6f; // Used to convert the turning angle into angular speed param

    private Animator _anim;
    private TagsHashIDs _hashIDs;

    // Constructor, called by other script when an instance of this class is created:
    public AnimatorSetup(Animator anim, TagsHashIDs tagsHashes)
    {
        _anim = anim;
        _hashIDs = tagsHashes;
    }

    // -----------------------------------------------------------------------------------------------------
    // METHOD:      Setup()
    // Desciption:  Called from EnemyAnimation.cs file in NavAnimSetup() method, which is called every 
    //              frame update. The speed parameter is the magnitude of the forward facing vector of the agent
    //              projected onto the local desired velocity vector. The angle is simply the angle between
    //              these two vectors. We use this method to pass these values into the animator controller,
    //              dividing the angle by the angle response time, in order to achieve a smother rotation
    // ------------------------------------------------------------------------------------------------------
    public void Setup(float speed, float angle)
    {
        float angularSpeed = angle / AngleResponseTime;
        // Pass values to animator controller VelX and VelY
        _anim.SetFloat(_hashIDs.VelY, speed, SpeedDampTime, Time.deltaTime);
        _anim.SetFloat(_hashIDs.VelX, angularSpeed, AngularSpeedDampTime, Time.deltaTime);
    }
}
