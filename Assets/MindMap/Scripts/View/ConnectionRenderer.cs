using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConnectionRenderer : MonoBehaviour {

    public Transform connectedTo;
    public Material lineMaterial;
    public float gapLength = 0.5f;

    private Mesh mesh;

	// Use this for initialization
	void Start () {
        mesh = new Mesh();
	}
	
	// Update is called once per frame
	void Update () {
        if (lineMaterial != null)
        {
            if (mesh == null)
                mesh = new Mesh();
            Mesh m = mesh;

            SceneNode thisNode = GetComponent<SceneNode>();
            Vector3 thisPos = (!Application.isPlaying || thisNode == null) ? transform.position : thisNode.WorldPosition;

            Vector3 thatPos = Vector3.zero;
            Transform that = connectedTo == null ? transform.parent : connectedTo;
            if (that != null)
            {
                SceneNode thatNode = that.GetComponent<SceneNode>();
                thatPos = (!Application.isPlaying || thatNode == null) ? that.position : thatNode.WorldPosition;
            }

            Vector3 thisToThat = (thatPos - thisPos).normalized;

            m.SetVertices(new List<Vector3>() { thisPos + thisToThat * gapLength, thatPos - thisToThat * gapLength });
            m.SetColors(new List<Color>() { Color.red, Color.blue });
            m.SetIndices(new int[] { 0, 1, 0 }, MeshTopology.LineStrip, 0);
            Graphics.DrawMesh(m, Matrix4x4.identity, lineMaterial, 0);
        }
    }
}
