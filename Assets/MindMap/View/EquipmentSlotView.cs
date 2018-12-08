using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlotView : MonoBehaviour {

    public Tool targetTool;
    public Material material;

    public bool isHidden = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (targetTool != null && !isHidden)
        {
            Matrix4x4 toolToWorld = transform.localToWorldMatrix * targetTool.transform.worldToLocalMatrix;
            Renderer[] renderers = targetTool.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                MeshFilter mf = r.GetComponent<MeshFilter>();
                if (mf == null || mf.mesh == null)
                    continue;
                Matrix4x4 localToWorld = toolToWorld * r.transform.localToWorldMatrix;
                Material mat = material == null ? r.material : material;
                Graphics.DrawMesh(mf.mesh, localToWorld, mat, r.sortingLayerID);
            }
        }
	}
}
