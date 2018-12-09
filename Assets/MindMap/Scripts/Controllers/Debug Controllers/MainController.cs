using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void OnControllerGrab(InputControlState hand, bool state)
    {
        Debug.Log("GrabAction_" + hand.HandObject.name + "_" + state);
    }

    public void OnControllerActivate(InputControlState hand, bool state)
    {
        Debug.Log("ActivateAction_" + hand.HandObject.name + "_" + state);
    }

    public void OnControllerSelect(InputControlState hand, bool state)
    {
        Debug.Log("SelectAction_" + hand.HandObject.name + "_" + state);
    }

    public void OnControllerSwipe(InputControlState hand, Vector2 swipe)
    {
        Debug.Log("SwipeAction_" + hand.HandObject.name + "_" + swipe);
    }

    public void OnControllerMove(InputControlState hand)
    {
        if (hand.IsGrabbing)
            Debug.Log("MoveAction_" + hand.HandObject.name);
    }
}
