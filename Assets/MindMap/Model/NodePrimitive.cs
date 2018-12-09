using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This entire class was taken from the week 5 examples
 */

public class NodePrimitive: MonoBehaviour {
    public Color MyColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);
    public Vector3 Pivot;
	
    public void LoadShaderMatrix(ref Matrix4x4 nodeMatrix)
    {
        Matrix4x4 p = Matrix4x4.TRS(Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 invp = Matrix4x4.TRS(-Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        Matrix4x4 m = nodeMatrix * p * trs * invp;
        GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", m);

        // Debug.Log("New model matrix made: __ModelMatrix" + counter);
        GetComponent<Renderer>().material.SetMatrix("_ModelMatrix_IT", m.inverse.transpose);
        GetComponent<Renderer>().material.SetColor("MyColor", MyColor);
    }
}