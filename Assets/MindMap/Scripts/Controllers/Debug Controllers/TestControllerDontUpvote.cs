using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControllerDontUpvote : MonoBehaviour {

    public InputController[] _controllers;
    public TheWorld _theWorld;

	// Use this for initialization
	void Start () {
        foreach (InputController ic in _controllers)
        {
            ic.onGrab.AddListener(onGrab);
        }

    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void onGrab(InputControlState ics, bool grabbed)
    {
        if (grabbed)
            _theWorld.GrabObject(ics.HandObject.transform);
        else
            _theWorld.ReleaseObject(ics.HandObject.transform);
    }
}
