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
        Swipe(swipe * 45.0f);
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

    void Swipe(Vector2 input)
    {
        if (selectedNode != null)
        {
            Transform t = selectedNode.transform;
            t.localRotation = Quaternion.AngleAxis(input.x, Vector3.up) * t.localRotation;
            t.localRotation = Quaternion.AngleAxis(input.y, t.localRotation * Vector3.right) * t.localRotation;
        }
    }
}
