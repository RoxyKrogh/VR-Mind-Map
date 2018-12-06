using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWorld : MonoBehaviour {

    public SceneNode _root;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_root != null) {
            Matrix4x4 i = Matrix4x4.identity;
            _root.CompositeXform(ref i);
        }
	}
}
