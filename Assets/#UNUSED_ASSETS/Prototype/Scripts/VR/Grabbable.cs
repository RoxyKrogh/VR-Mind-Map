using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(XformProvider))]
public class Grabbable : XformMonoBehaviour {

    private bool savedPhysicsState;
    private bool _isGrabbed = false;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GrabGrabbable(Grabber grabber)
    {
        Debug.Log("Grabbed " + name);
        Rigidbody physicsbody = GetComponent<Rigidbody>();
        if (physicsbody != null)
        {
            savedPhysicsState = physicsbody.isKinematic;
            physicsbody.isKinematic = false;
        }
        _isGrabbed = true;
    }

    public void ReleaseGrabbable(Grabber grabber)
    {
        Debug.Log("Released " + name);
        Rigidbody physicsbody = GetComponent<Rigidbody>();
        if (physicsbody != null)
        {
            physicsbody.isKinematic = savedPhysicsState;
            physicsbody.velocity = Vector3.zero;
        }
        _isGrabbed = false;
    }

    public bool isGrabbed
    {
        get { return _isGrabbed; }
    }
}
