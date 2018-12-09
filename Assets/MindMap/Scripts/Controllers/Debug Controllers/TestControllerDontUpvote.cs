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
            ic.onSelect.AddListener(onTriggerPull);
        }

    }
	
	// Update is called once per frame
	void Update () {

	}

    private void onGrab(InputControlState ics, bool grabbed)
    {
        Debug.Log("Grab action! " + grabbed);
        if (grabbed)
            _theWorld.GrabObject(ics.HandObject.transform);
        else
            _theWorld.ReleaseObject(ics.HandObject.transform);
    }

    private void onTriggerPull(InputControlState ics, bool pulled)
    {
        Debug.Log("Trigger action!");
        if (pulled)
            _theWorld.StartDrawing(ics.HandObject.transform);
        else
            _theWorld.StopDrawing(ics.HandObject.transform);
    }

    private void onTrackpadClick(InputControlState ics, bool clicked)
    {
        if (clicked)
            _theWorld.CreateBubble(ics.HandObject.transform, true);
        else
            _theWorld.ReleaseObject(ics.HandObject.transform);
    }
}
