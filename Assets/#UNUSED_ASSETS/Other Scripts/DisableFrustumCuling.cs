using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DisableFrustumCuling : MonoBehaviour {

    private static float MAX_VALUE = float.MaxValue;

    private Camera cam;

    void Start()
    {
        cam = this.GetComponent<Camera>();
    //}

    //void OnPreCull()
    //{
        cam.cullingMatrix = Matrix4x4.Ortho(-MAX_VALUE, MAX_VALUE, -MAX_VALUE, MAX_VALUE, 0.001f, MAX_VALUE) *
                            Matrix4x4.Translate(Vector3.forward * -MAX_VALUE / 2f) *
                            cam.worldToCameraMatrix;
    }

    void OnDisable()
    {
        cam.ResetCullingMatrix();
    }
}
