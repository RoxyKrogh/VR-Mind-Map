using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * This entire class was taken from the week 5 examples
 */
[DisallowMultipleComponent]
public class NodePrimitive: MonoBehaviour {
    public Color MyColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);
    public Vector3 Pivot;
    public Vector3 WorldPosition { get; private set; }
	
    public void LoadShaderMatrix(ref Matrix4x4 nodeMatrix)
    {
        Matrix4x4 p = Matrix4x4.TRS(Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 invp = Matrix4x4.TRS(-Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        Matrix4x4 m = nodeMatrix * p * trs * invp;
        WorldPosition = m.GetColumn(3);
        ApplyMatrix(ref m);
    }
    
    private void ApplyMatrix(ref Matrix4x4 modelMatrix)
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null)
            return;
        r.material.SetMatrix("_ModelMatrix", modelMatrix);

        // Debug.Log("New model matrix made: __ModelMatrix" + counter);
        r.material.SetMatrix("_ModelMatrix_IT", modelMatrix.inverse.transpose);
        r.material.SetColor("MyColor", MyColor);
    }
}