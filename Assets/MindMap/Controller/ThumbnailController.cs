using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ThumbnailController : MonoBehaviour
{
    public InputController inputController;
    public NodePrimitive selectedNode;
    public Transform cameraGimbal;

    // Use this for initialization
    void Start()
    {
        inputController.onSwipe.AddListener(OnSwipe);
    }

    // Update is called once per frame
    void Update()
    {
        cameraGimbal.transform.position = selectedNode.WorldPosition;
        cameraGimbal.transform.rotation = selectedNode.transform.rotation; // rotation is not different for SceneNode
    }

    private void OnSwipe(InputControlState state, Vector2 swipe)
    {
        Swipe(swipe * 45.0f);
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
