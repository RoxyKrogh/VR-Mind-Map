using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic sphere collision tag for geometries within a scenenode
public class SceneNodeCollider : MonoBehaviour {

    private Vector3 actualPos
    {
        get
        {
            return transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, 0));
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(actualPos, 0.5f);
    }

    // Whether the given transform falls within the spherical boundaries of this object
    public bool Collides(Transform pos)
    {
        return SqrDistFrom(pos) <= 0.25;
    }

    public float SqrDistFrom(Transform pos)
    {
        return (pos.position - actualPos).sqrMagnitude;
    }

    public float DistFrom(Transform pos)
    {
        return Mathf.Sqrt(SqrDistFrom(pos));
    }
}
