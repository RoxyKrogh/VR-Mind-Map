using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossTest : MonoBehaviour {

    public Transform v1;
    public Transform v2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 cross = Vector3.Cross(v1.up * v1.localScale.y, v2.up * v2.localScale.y);
        transform.LookAt(transform.position + cross);
        transform.localScale = new Vector3(1,1,cross.magnitude);
	}
}
