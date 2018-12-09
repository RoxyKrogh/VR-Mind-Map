using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SceneNode : MonoBehaviour {

    private Matrix4x4 mCombinedParentXform;
    public Matrix4x4 CombinedParentXform { get { return mCombinedParentXform; } private set { mCombinedParentXform = value; } }
    public Vector3 WorldPosition { get { return mCombinedParentXform.GetColumn(3); } }

    public Vector3 NodeOrigin = Vector3.zero;
    public List<NodePrimitive> PrimitiveList;
    public Transform GeometryList;

    public Transform AxisFrame = null;
    private Vector3 kDefaultTreeTip = new Vector3(0, 0, 0);

	// Use this for initialization
	protected void Start () {
        InitializeSceneNode();

        if (GeometryList == null)
            GeometryList = transform.Find("Geom");
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void InitializeSceneNode()
    {
        mCombinedParentXform = Matrix4x4.identity;
    }

    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform)
    {
        

        Matrix4x4 orgT = Matrix4x4.Translate(NodeOrigin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        
        mCombinedParentXform = parentXform * orgT * trs;

        // propagate to all children
        foreach (Transform child in transform)
        {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null)
            {
                cn.CompositeXform(ref mCombinedParentXform);
            }
        }

        // If we have a Geometry list attached to this, we can just use that
        NodePrimitive[] geomList = GeometryList == null ? PrimitiveList.ToArray() : GeometryList.GetComponentsInChildren<NodePrimitive>();

        // disenminate to primitives
        foreach (NodePrimitive p in geomList)
        {
            p.LoadShaderMatrix(ref mCombinedParentXform);
        }

        // Compute AxisFrame 
        if (AxisFrame != null)
        {
            AxisFrame.localPosition = mCombinedParentXform .MultiplyPoint(kDefaultTreeTip);
        }
    }
}