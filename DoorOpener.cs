using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CLASS: DoorOpener
// Desc:  This script lerps the vector3 position of a gameobject in a certain direction,
//        And for the third lerp parameter passes in a point on an animation curve
//        In order to prevent erroneous method calls the script instance destroys itself once the ienumerator is called

public class DoorOpener : MonoBehaviour {


    public AnimationCurve ObjectCurve = new AnimationCurve();
    public ParticleSystem[] _steamParticles = new ParticleSystem[2];
    [SerializeField] float _distance = 4f;
    [SerializeField] float _duration = 0.5f;

    private Transform _transform;
    private Vector3 _startPos, _endPos;
    private AudioSource _doorAudio;

    void Start () {
        _transform = GetComponentInChildren<Transform>();
        _startPos = _transform.position;
        _endPos = _startPos + (-_transform.up * _distance);
        _doorAudio = GetComponent<AudioSource>();
    }
	
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHashIDs.Player))
        {
            _doorAudio.PlayOneShot(_doorAudio.clip);
            foreach (ParticleSystem ps in _steamParticles)
            {
                ps.Play();
            }
            StartCoroutine(MoveDoor());
        }
    }

    IEnumerator MoveDoor()
    {
       // _currentState = ObjectState.Animating;
        float timer = 0.0f;

        // is our goal the start state? then our start position is the end position
        //Vector3 startPos = (goalState == ObjectState.Start) ? _endPos : _startPos;
        //Vector3 endPos = (goalState == ObjectState.Start) ? _startPos : _endPos;

        while (timer <= _duration)
        {
            // Lerp to the end position, each lerp dictated by the playhead position on the curve, dictated by normalized float t
            float t = timer / _duration;
            _transform.position = Vector3.Lerp(_startPos, _endPos, ObjectCurve.Evaluate(t));

            timer += Time.deltaTime;
            yield return null;
        }
        // Snap the object ot it's end pos just in case, and set the current object state to the passed in value
        _transform.position = _endPos;
        Destroy(this);
    }
}
