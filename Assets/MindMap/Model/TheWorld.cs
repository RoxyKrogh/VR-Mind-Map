using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWorld : MonoBehaviour {

    public InputController testControllerDontUpvote;

    // Contains all information necessary to grab an object from any point an move it relative to its starting position,
    // without just snapping it
    private struct GrabberInfo
    {
        public Transform child;
        public Vector3 startPos;
        public Vector3 startRotUp;
        public Vector3 startRotForward;
    }

    public SceneNode _root;

    private delegate void TransformationMethod(Transform trs1, Transform trs2);

    private Dictionary<Transform, GrabberInfo> movementPairs;

	// Use this for initialization
	void Start () {
        movementPairs = new Dictionary<Transform, GrabberInfo>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_root != null) {
            Matrix4x4 i = transform.localToWorldMatrix;
            _root.CompositeXform(ref i);
        }

        foreach (KeyValuePair<Transform, GrabberInfo> kvp in movementPairs)
            MoveObject(kvp.Key, kvp.Value);

        SceneNodeCollider snc = _root.gameObject.GetComponentInChildren<SceneNodeCollider>();

        if (snc)
        {
            Debug.Log(snc.SqrDistFrom(testControllerDontUpvote.transform) + ", " + snc.DistFrom(testControllerDontUpvote.transform));
        }
	}

    public void GrabObject(Transform parentTransform)
    {
        SceneNodeCollider target = null;
        GrabberInfo grabberInfo = new GrabberInfo();

        // Used to determine what the closest object is
        float dist = -1;

        foreach(SceneNodeCollider npr in _root.gameObject.GetComponentsInChildren<SceneNodeCollider>())
        {
            bool collides = npr.Collides(parentTransform);

            if (collides)
            {
                float newDist = npr.SqrDistFrom(parentTransform);

                if (dist < 0 || newDist < dist)
                {
                    target = npr;
                    dist = newDist;
                }
            }
        }

        if (target != null)
        {
            grabberInfo.child = target.transform;
            grabberInfo.startPos = parentTransform.worldToLocalMatrix.MultiplyPoint(target.transform.position);
            grabberInfo.startRotForward = parentTransform.worldToLocalMatrix.MultiplyVector(target.transform.forward);
            grabberInfo.startRotUp = parentTransform.worldToLocalMatrix.MultiplyVector(target.transform.up);

            movementPairs.Add(parentTransform, grabberInfo);
        }
    }

    public void ReleaseObject(Transform parentTransform)
    {
        movementPairs.Remove(parentTransform);
    }

    private void MoveObject(Transform parentTransform, GrabberInfo gi)
    {
        gi.child.position = parentTransform.localToWorldMatrix.MultiplyPoint(gi.startPos);
        gi.child.rotation = Quaternion.LookRotation(parentTransform.localToWorldMatrix.MultiplyVector(gi.startRotForward), parentTransform.localToWorldMatrix.MultiplyVector(gi.startRotUp));
    }
}
