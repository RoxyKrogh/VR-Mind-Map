using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(XformProvider))]
public class Activatable : XformMonoBehaviour
{

    private bool _isActive = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void ActivateActivatable(Grabber grabber)
    {
        Debug.Log("Activated " + name);
        _isActive = true;
    }

    public void DeactivateActivatable(Grabber grabber)
    {
        Debug.Log("Deactivated " + name);
        _isActive = false;
    }

    public bool isActive
    {
        get { return _isActive; }
    }
}
