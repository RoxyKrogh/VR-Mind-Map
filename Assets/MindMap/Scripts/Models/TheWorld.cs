using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private struct ReparentInfo
    {
        public Transform child;
        public float prevGapLength;
        public ConnectionRenderer connectionRenderer;
    }
    
    [System.Serializable]
    public class InteractionEvent : UnityEvent<Transform> { }

    // List of all root scenenodes that are children of the world
    public SceneNode _root;

    private delegate void TransformationMethod(Transform trs1, Transform trs2);

    private Dictionary<Transform, GrabberInfo> _movementPairs = new Dictionary<Transform, GrabberInfo>();
    private Dictionary<Transform, DoodlePenInfo> _pens = new Dictionary<Transform, DoodlePenInfo>();
    private Dictionary<Transform, ReparentInfo> _reparentPairs = new Dictionary<Transform, ReparentInfo>();
    private Dictionary<Transform, SceneNodeCollider> _selectionPairs = new Dictionary<Transform, SceneNodeCollider>();

    [SerializeField] /* show in inspector */
    private GameObject bubblePrefab; // prefab to use for bubble objects

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNodeTransforms();


        foreach (KeyValuePair<Transform, GrabberInfo> kvp in _movementPairs)
            MoveObject(kvp.Key, kvp.Value);

        foreach (KeyValuePair<Transform, DoodlePenInfo> kvp in _pens)
            UpdatePenTarget(kvp.Key, kvp.Value.pen);
    }

    void UpdateNodeTransforms()
    {
        Matrix4x4 i = transform.localToWorldMatrix;
        _root.CompositeXform(ref i);
    }

    public SceneNodeCollider GetSelectionFor(Transform hand)
    {
        if (_selectionPairs.ContainsKey(hand))
            return _selectionPairs[hand];
        else
            return null;
    }

    public T GetSelectionFor<T>(Transform hand) where T : Component
    {
        SceneNodeCollider c = GetSelectionFor(hand);
        if (c == null)
            return null;
        return c.GetComponent<T>();
    }

    public void UpdateSelection(Transform hand)
    {
        _selectionPairs[hand] = GetObjectNearestTransform<SceneNodeCollider>(hand);
    }

    public void GrabObject(Transform parentTransform)
    {
        GrabberInfo grabberInfo = new GrabberInfo();

        SceneNode target = GetSelectionFor<SceneNode>(parentTransform);

        if (target == null)
            target = _root;

        if (target)
        {
            grabberInfo.child = target.transform;
            grabberInfo.startPos = parentTransform.worldToLocalMatrix.MultiplyPoint(target.WorldPosition);
            grabberInfo.startRotForward = parentTransform.worldToLocalMatrix.MultiplyVector(target.WorldForward);
            grabberInfo.startRotUp = parentTransform.worldToLocalMatrix.MultiplyVector(target.WorldUp);

            _movementPairs[parentTransform] = grabberInfo;
        }
    }

    public void ReleaseObject(Transform parentTransform)
    {
        _movementPairs.Remove(parentTransform);
    }

    private void MoveObject(Transform parentTransform, GrabberInfo gi)
    {
        Vector3 worldTo = parentTransform.localToWorldMatrix.MultiplyPoint(gi.startPos);
        Transform giparent = gi.child.parent;
        SceneNode node = giparent.GetComponent<SceneNode>();
        Matrix4x4 worldToLocal;
        if (node != null)
            worldToLocal = node.CombinedParentXform.inverse; // use scene node transform, if it is a scene node
        else
            worldToLocal = giparent.transform.worldToLocalMatrix; // else use unity transform
        gi.child.localPosition = worldToLocal.MultiplyPoint(worldTo); // apply position, in giparent's local space

        //if (_root != null && gi.child != _root.transform) // if the root node is grabbed, do not rotate
        {
            gi.child.rotation = Quaternion.LookRotation(parentTransform.localToWorldMatrix.MultiplyVector(gi.startRotForward), parentTransform.localToWorldMatrix.MultiplyVector(gi.startRotUp));
        }
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

        SceneNodeCollider selected = GetSelectionFor(penWith);
        Bubble targetBubble = selected == null ? null : selected.GetComponentInChildren<Bubble>(); // get selected scene node

        penInfo.pen.targetBubble = targetBubble;

        _pens[penWith] = penInfo;
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

        SceneNodeCollider selected = GetSelectionFor(hand);
        pen.targetBubble = selected == null ? null : selected.GetComponentInChildren<Bubble>(); // get selected scene node
    }

    // TheWorld.InteractionEvent
    public void CreateBubble(Transform at)
    {
        CreateBubble(at, false);
    }

    // TheWorld.InteractionEvent
    public void CreateAndGrabBubble(Transform at)
    {
        CreateBubble(at, true);
    }

    // Create a new bubble at the given transform's position. It will have no parents, by default
    // If you choose to grab it afterwards, be sure to stop grabbing (ReleaseObject) later as well
    public void CreateBubble(Transform at, bool grabAfterwards)
    {
        GameObject bubbleNode = Instantiate(bubblePrefab);
        bubbleNode.transform.position = at.position;
        bubbleNode.transform.parent = _root.transform;
        UpdateNodeTransforms(); // update bubbleNode's SceneNode transform

        if (grabAfterwards)
        {
            _selectionPairs[at] = bubbleNode.GetComponent<SceneNodeCollider>(); // select the bubbleNode
            GrabObject(at); // grab the bubbleNode
        }
    }

    // Start attempting to reparent the closest object that the given transform is colliding with
    public void BeginReparent(Transform parent)
    {
        Transform child = GetSelectionFor<Transform>(parent); // get selected scene node

        if (child)
        {
            ReparentInfo repInfo = new ReparentInfo();
            repInfo.child = child;
            repInfo.connectionRenderer = child.GetComponent<ConnectionRenderer>();
            repInfo.connectionRenderer.connectedTo = parent;
            repInfo.prevGapLength = repInfo.connectionRenderer.gapLength;
            repInfo.connectionRenderer.gapLength = 0;
            _reparentPairs[parent] = repInfo;
        }
    }

    // TheWorld.InteractionEvent
    public void EndReparent(Transform parent)
    {
        EndReparent(parent, false);
    }

    // TheWorld.InteractionEvent
    public void EndReparentOrOrphan(Transform parent)
    {
        EndReparent(parent, true);
    }

    // If the bool is true, the reparented object will become a new root node. Otherwise, it remains with its current parent.
    public void EndReparent(Transform parent, bool orphanIfNoTarget)
    {
        if(_reparentPairs.ContainsKey(parent))
        {
            if (_reparentPairs[parent].child != null)
            {
                Transform target = GetSelectionFor<Transform>(parent); // get selected scene node

                if (target == null)
                {
                    if (orphanIfNoTarget)
                        _reparentPairs[parent].child.parent = transform;
                }
                else if (target != _reparentPairs[parent].child)
                {
                    // If we make this parent a child of one of its own children, we'll have one of those children remain connected to the
                    // parent's parent
                    if (target.IsChildOf(_reparentPairs[parent].child))
                    {
                        Transform immediateChild = target;

                        while (immediateChild.parent != _reparentPairs[parent].child)
                            immediateChild = immediateChild.parent;

                        immediateChild.parent = _reparentPairs[parent].child.parent;
                        _reparentPairs[parent].child.parent = target.transform;
                    }
                    else
                    {
                        _reparentPairs[parent].child.parent = target.transform;
                    }
                }
            }

            _reparentPairs[parent].connectionRenderer.gapLength = _reparentPairs[parent].prevGapLength;
            _reparentPairs[parent].connectionRenderer.connectedTo = null;
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
