using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ThumbnailController : MonoBehaviour
{
    public InputController inputController;
    public NodePrimitive selectedNode;
    public Transform cameraGimbal;
    public TheWorld world;
    public NodePrimitive defaultNode;

    private bool isSwiping = false;

    // Use this for initialization
    void Start()
    {
        if (world == null)
            world = GetComponentInParent<TheWorld>();
        inputController.onSwipe.AddListener(OnSwipe);
        inputController.onMoved.AddListener(OnMoved);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputController.IsTouched)
            isSwiping = false;
        if (selectedNode != null)
        {
            cameraGimbal.transform.position = selectedNode.WorldPosition;
            cameraGimbal.transform.rotation = selectedNode.transform.rotation; // rotation is not different for SceneNode
            float depth = selectedNode.Pivot.magnitude * 2 + 0.3f;
            Camera cam = cameraGimbal.GetComponentInChildren<Camera>();
            cam.farClipPlane = depth;
            cam.orthographicSize = depth * 0.5f;
        }
    }

    private void OnSwipe(InputControlState state, Vector2 swipe)
    {
        const float SLIDE_THRESHOLD = 0.04f;
        const float EDGE_THRESHOLD = 0.7f;
        float mag = swipe.sqrMagnitude;
        if (mag > SLIDE_THRESHOLD * SLIDE_THRESHOLD)
            isSwiping = true;
        if (isSwiping)
        {
            Vector2 startV = (state.AxisPosition - state.AxisDelta);
            Vector2 endV = state.AxisPosition;
            if (startV.sqrMagnitude > EDGE_THRESHOLD * EDGE_THRESHOLD && endV.sqrMagnitude > EDGE_THRESHOLD * EDGE_THRESHOLD)
            {
                startV = startV.normalized;
                endV = endV.normalized;
                float startA = Mathf.Atan2(startV.y, startV.x) * Mathf.Rad2Deg;
                float endA = Mathf.Atan2(endV.y, endV.x) * Mathf.Rad2Deg;
                float deltaA = Mathf.DeltaAngle(startA, endA);
                Spin(deltaA);
            }
            else
                Swipe(swipe * 45.0f);
            
        }
    }

    private void OnMoved(InputControlState state)
    {
        // update selected node reference
        SceneNodeCollider selection = world.GetSelectionFor(state.HandObject);
        if (selection == null) { selectedNode = defaultNode; return; }
        Transform geom = selection.transform.Find("Geom");
        if (geom == null) { selectedNode = defaultNode; return; }
        Transform thumbnail = geom.Find("Thumbnail");
        if (thumbnail == null) { selectedNode = defaultNode; return; }
        selectedNode = thumbnail.GetComponent<NodePrimitive>();
    }

    void Swipe(Vector2 delta)
    {
        if (selectedNode != null)
        {
            Transform t = selectedNode.transform;
            t.localRotation = t.localRotation * Quaternion.AngleAxis(delta.x, Vector3.up);
            t.localRotation = t.localRotation * Quaternion.AngleAxis(delta.y, Vector3.right);
        }
    }

    void Spin(float angle)
    {
        if (selectedNode != null)
        {
            Transform t = selectedNode.transform;
            t.rotation = Quaternion.AngleAxis(angle, t.forward) * t.rotation;
        }
    }
}
