using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic sphere collision tag for geometries within a scenenode
public class SceneNodeCollider : MonoBehaviour {

    private const float DEFAULT_RADIUS = 0.5f;

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
        Gizmos.DrawSphere(actualPos, DEFAULT_RADIUS);
    }

    // Whether the given transform falls within the spherical boundaries of this object
    public bool Collides(Transform pos)
    {
        return SqrDistFrom(pos) <= DEFAULT_RADIUS * DEFAULT_RADIUS;
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
