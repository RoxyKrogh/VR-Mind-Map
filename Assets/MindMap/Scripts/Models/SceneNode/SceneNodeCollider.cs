using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic sphere collision tag for geometries within a scenenode
[RequireComponent(typeof(SceneNode))]
public class SceneNodeCollider : MonoBehaviour {

    public float mRadius = 0.5f;

    public SceneNode sceneNode { get { return GetComponent<SceneNode>(); } }

    private Vector3 actualPos { get { return sceneNode.WorldPosition; } }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(actualPos, mRadius);
    }

    // Whether the given transform falls within the spherical boundaries of this object
    public bool Collides(Transform pos)
    {
        return SqrDistFrom(pos) <= mRadius * mRadius;
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
