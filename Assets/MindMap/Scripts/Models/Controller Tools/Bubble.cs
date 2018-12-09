using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public float radiusScale = 1.0f;

    public Quaternion thumbnailAngle = Quaternion.identity;

    public void AddDoodle(MeshFilter doodle)
    {
        doodle.transform.SetParent(transform);
        doodle.gameObject.tag = "Doodle";
    }

    public IEnumerable<MeshFilter> doodles
    {
        get { return GetComponentsInChildren<MeshFilter>().Where(mf => mf.gameObject.tag == "Doodle"); }
    }
}
