using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWorld : MonoBehaviour
{
    public MeshFilter doodleTarget;

    // Contains all information necessary to grab an object from any point an move it relative to its starting position,
    // without just snapping it
    private struct GrabberInfo
    {
        public Transform child;
        public Vector3 startPos;
        public Vector3 startRotUp;
        public Vector3 startRotForward;
    }

    private struct DoodlePenInfo
    {
        public bool startedWithDoodlePen;
        public DoodlePen pen;
    }

    public SceneNode _root;

    private delegate void TransformationMethod(Transform trs1, Transform trs2);

    private Dictionary<Transform, GrabberInfo> movementPairs;

    private Dictionary<Transform, DoodlePenInfo> pens;

    // Use this for initialization
    void Start()
    {
        movementPairs = new Dictionary<Transform, GrabberInfo>();
        pens = new Dictionary<Transform, DoodlePenInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_root != null)
        {
            Matrix4x4 i = transform.localToWorldMatrix;
            _root.CompositeXform(ref i);
        }

        foreach (KeyValuePair<Transform, GrabberInfo> kvp in movementPairs)
            MoveObject(kvp.Key, kvp.Value);

        foreach (KeyValuePair<Transform, DoodlePenInfo> kvp in pens)
        {
            UpdatePenTarget(kvp.Key, kvp.Value.pen);
        }
    }

    public void GrabObject(Transform parentTransform)
    {
        // SceneNodeCollider target = null;
        GrabberInfo grabberInfo = new GrabberInfo();

        Transform target = GetObjectNearestTransform(parentTransform);

        if (target)
        {
            grabberInfo.child = target;
            grabberInfo.startPos = parentTransform.worldToLocalMatrix.MultiplyPoint(target.position);
            grabberInfo.startRotForward = parentTransform.worldToLocalMatrix.MultiplyVector(target.forward);
            grabberInfo.startRotUp = parentTransform.worldToLocalMatrix.MultiplyVector(target.up);

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

    public void StartDrawing(Transform penWith)
    {
        DoodlePenInfo penInfo = new DoodlePenInfo();

        penInfo.startedWithDoodlePen = penInfo.pen = penWith.gameObject.GetComponent<DoodlePen>();

        if (!penInfo.startedWithDoodlePen)
        {
            penInfo.pen = penWith.gameObject.AddComponent<DoodlePen>();
        }

        penInfo.pen.isPenDown = true;
        penInfo.pen.doodleTarget = doodleTarget;

        Bubble targetBubble = GetObjectNearestTransform<Bubble>(penWith);

        if (targetBubble)
        {
            penInfo.pen.targetBubble = targetBubble;
        }

        pens.Add(penWith, penInfo);
    }

    public void StopDrawing(Transform penWith)
    {
        if (pens.ContainsKey(penWith))
        {
            pens[penWith].pen.isPenDown = false;

            // Remove the doodle pen if our object didn't start with it
            if (!pens[penWith].startedWithDoodlePen)
                Destroy(penWith.GetComponent<DoodlePen>());

            pens.Remove(penWith);
        }
    }

    // Updates the target bubble of the pen corresponding to the given transform
    private void UpdatePenTarget(Transform hand, DoodlePen pen)
    {
        if(pen.isDrawing)
            AddDoodleToSceneNode(pen);
        pen.targetBubble = GetObjectNearestTransform<Bubble>(hand);
    }

    // Assuming that the bubble is stored in a scenenode's Geom child, this will add its doodles to the scenenode hierarchy so they
    // will be rendered
    /*private void AddDoodlesToSceneNode(Bubble bubble)
    {
        foreach (MeshFilter mf in bubble.doodles)
        {
            GameObject doodle = mf.gameObject;
            NodePrimitive doodlePrimitive = doodle.AddComponent<NodePrimitive>();
            // We don't need to do any crazy bullshit like adding this to the SceneNode's primitive list,
            // as we've redefined the scenenode to try to use a list of geometries as a child "Geom",
            // which this parent is
            doodle.transform.parent = bubble.transform.parent; 
        }
    }*/

    // Add the given pen's current doodle to the scenenode geometry of the bubble it's attached to
    private void AddDoodleToSceneNode(DoodlePen pen)
    {
            GameObject doodle = pen.doodleTarget.gameObject;
            NodePrimitive doodlePrimitive = doodle.AddComponent<NodePrimitive>();
            doodle.transform.parent = pen.targetBubble.transform.parent;
    }

    // Find the transform of a scenenode object in the world nearest to the given transform that's colliding with it
    private Transform GetObjectNearestTransform(Transform parentTransform)
    {
        return GetObjectNearestTransform<Transform>(parentTransform);
    }

    // Find a component T attached to the nearest colliding object to the given transform
    private T GetObjectNearestTransform<T>(Transform parentTransform) where T : Component
    {
        SceneNodeCollider target = null;
        T retVal = null;
        float dist = float.PositiveInfinity;

        foreach (SceneNodeCollider snc in _root.gameObject.GetComponentsInChildren<SceneNodeCollider>())
        {
            if (snc.gameObject.GetComponent<T>() || snc.transform.Find("Geom").GetComponentInChildren<T>())
            {
                bool collides = snc.Collides(parentTransform);

                if (collides)
                {
                    float newDist = snc.SqrDistFrom(parentTransform);

                    if (newDist < dist)
                    {
                        target = snc;
                        dist = newDist;
                    }
                }
            }
        }

        if (target)
        {
            if (target.GetComponent<T>())
                retVal = target.GetComponent<T>();
            else
                retVal = target.transform.Find("Geom").GetComponentInChildren<T>();
        }

        return retVal;
    }
}
