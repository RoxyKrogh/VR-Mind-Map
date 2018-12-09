using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWorld : MonoBehaviour
{
    public MeshFilter _doodleTarget;

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

    // List of all root scenenodes that are children of the world
    public List<SceneNode> _roots;

    private delegate void TransformationMethod(Transform trs1, Transform trs2);

    private Dictionary<Transform, GrabberInfo> _movementPairs;

    private Dictionary<Transform, DoodlePenInfo> _pens;

    private Dictionary<Transform, Transform> _reparentPairs;

    // Use this for initialization
    void Start()
    {
        foreach (SceneNode child in transform)
            _roots.Add(child);

        _movementPairs = new Dictionary<Transform, GrabberInfo>();
        _pens = new Dictionary<Transform, DoodlePenInfo>();
        _reparentPairs = new Dictionary<Transform, Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SceneNode root in _roots)
        {
            Matrix4x4 i = transform.localToWorldMatrix;
            root.CompositeXform(ref i);
        }

        foreach (KeyValuePair<Transform, GrabberInfo> kvp in _movementPairs)
            MoveObject(kvp.Key, kvp.Value);

        foreach (KeyValuePair<Transform, DoodlePenInfo> kvp in _pens)
            UpdatePenTarget(kvp.Key, kvp.Value.pen);
    }

    public void GrabObject(Transform parentTransform)
    {
        GrabberInfo grabberInfo = new GrabberInfo();

        Transform target = GetObjectNearestTransform(parentTransform);

        if (target)
        {
            grabberInfo.child = target;
            grabberInfo.startPos = parentTransform.worldToLocalMatrix.MultiplyPoint(target.position);
            grabberInfo.startRotForward = parentTransform.worldToLocalMatrix.MultiplyVector(target.forward);
            grabberInfo.startRotUp = parentTransform.worldToLocalMatrix.MultiplyVector(target.up);

            _movementPairs.Add(parentTransform, grabberInfo);
        }
    }

    public void ReleaseObject(Transform parentTransform)
    {
        _movementPairs.Remove(parentTransform);
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
        penInfo.pen.doodleTarget = _doodleTarget;

        Bubble targetBubble = GetObjectNearestTransform<Bubble>(penWith);

        if (targetBubble)
        {
            penInfo.pen.targetBubble = targetBubble;
        }

        _pens.Add(penWith, penInfo);
    }

    public void StopDrawing(Transform penWith)
    {
        if (_pens.ContainsKey(penWith))
        {
            _pens[penWith].pen.isPenDown = false;

            // Remove the doodle pen if our object didn't start with it
            if (!_pens[penWith].startedWithDoodlePen)
                Destroy(penWith.GetComponent<DoodlePen>());

            _pens.Remove(penWith);
        }
    }

    // Updates the target bubble of the pen corresponding to the given transform
    private void UpdatePenTarget(Transform hand, DoodlePen pen)
    {
        if(pen.isDrawing)
            AddDoodleToSceneNode(pen);
        pen.targetBubble = GetObjectNearestTransform<Bubble>(hand);
    }

    // Create a new bubble at the given transform's position. It will have no parents, by default
    // If you choose to grab it afterwards, be sure to stop grabbing (ReleaseObject) later as well
    public void CreateBubble(Transform at, bool grabAfterwards = false)
    {
        GameObject BubbleNode = Resources.Load<GameObject>("Assets/MindMap/Prefabs/BubbleNode.prefab");
        BubbleNode.transform.parent = transform;
        BubbleNode.transform.position = at.position;
        _roots.Add(BubbleNode.GetComponent<SceneNode>());

        if (grabAfterwards)
            GrabObject(at);
    }

    // Start attempting to reparent the closest object that the given transform is colliding with
    public void BeginReparent(Transform parent)
    {
        Transform child = GetObjectNearestTransform(parent);

        if (child)
        {
            _reparentPairs.Add(parent, child);
        }
    }

    // If the bool is true, the reparented object will become a new root node. Otherwise, it remains with its current parent.
    public void EndReparent(Transform parent, bool orphanIfNoTarget = false)
    {
        if(_reparentPairs.ContainsKey(parent))
        {
            if (_reparentPairs[parent] != null)
            {
                Transform target = GetObjectNearestTransform<Transform>(parent);

                if (target == null)
                {
                    if (orphanIfNoTarget)
                        _reparentPairs[parent].parent = transform;
                }
                else if (target != _reparentPairs[parent])
                {
                    // If we make this parent a child of one of its own children, we'll have one of those children remain connected to the
                    // parent's parent
                    if (target.IsChildOf(_reparentPairs[parent]))
                    {
                        Transform immediateChild = target;

                        while (immediateChild.parent != _reparentPairs[parent])
                            immediateChild = immediateChild.parent;

                        immediateChild.parent = _reparentPairs[parent].parent;
                        _reparentPairs[parent].parent = target.transform;
                    }
                    else
                    {
                        _reparentPairs[parent].parent = target.transform;
                    }
                }
            }


            _reparentPairs.Remove(parent);
        }
    }


    // Add the given pen's current doodle to the scenenode geometry of the bubble it's attached to
    private void AddDoodleToSceneNode(DoodlePen pen)
    {
         GameObject doodle = pen.doodleTarget.gameObject;

        if (!doodle.GetComponent<NodePrimitive>())
        {
            doodle.AddComponent<NodePrimitive>();
            doodle.transform.parent = pen.targetBubble.transform.parent;
        }
    }

    // Find the transform of a scenenode object in the world nearest to the given transform that's colliding with it
    private Transform GetObjectNearestTransform(Transform parentTransform)
    {
        return GetObjectNearestTransform<Transform>(parentTransform);
    }

    // Find a component T attached to the nearest colliding scenenode object to the given transform
    private T GetObjectNearestTransform<T>(Transform parentTransform) where T : Component
    {
        SceneNodeCollider target = null;
        T retVal = null;
        float dist = float.PositiveInfinity;

        foreach (SceneNode root in _roots)
        {
            foreach (SceneNodeCollider snc in root.gameObject.GetComponentsInChildren<SceneNodeCollider>())
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
