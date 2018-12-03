using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRTouchOrbit : MonoBehaviour {

    public SteamVR_Behaviour_Pose pose;
    private SteamVR_Input_Sources inputSource; // right or left hand
    private SteamVR_Action_Pose currentPose;

    public SteamVR_Action_Vector2 swipeAction; // swipe the trackpad
    public SteamVR_Action_Boolean touchAction; // swipe the trackpad

    private Vector2 lastSwipeState;
    private bool lastTouchState;

    // Use this for initialization
    void Start ()
    {
        if (pose == null)
            pose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        pose.onTransformChanged.AddListener(UpdatePose);
        pose.onTransformUpdated.AddListener(UpdatePose);
        inputSource = pose.inputSource;

    }

    void UpdatePose(SteamVR_Action_Pose pose)
    {
        currentPose = pose;
    }

    // Update is called once per frame
    void Update () {
        /*Vector2 padPos = swipeAction.GetAxis(inputSource);
        if (padPos.sqrMagnitude > 0.05f)
            Swipe(swipeAction.GetAxisDelta(inputSource) * 45.0f);*/
        //bool touchState = touchAction.GetState(inputSource);
        //Vector2 swipeState = swipeAction.GetAxis(inputSource);
        //if (touchState)
        //    Debug.Log(swipeState);
        //if (touchState && lastTouchState)
        //{
        //    Debug.Log("proper swipe" + swipeState);
        //    Swipe(swipeState - lastSwipeState);
        //}
        //lastSwipeState = swipeState;
        //lastTouchState = touchState;
    }

    void OnEnable()
    {
        if (swipeAction != null)
        {
            swipeAction.AddOnChangeListener(OnSwipeAction, inputSource);
        }
        if (touchAction != null)
        {
            touchAction.AddOnChangeListener(OnTouchAction, inputSource);
        }
    }

    private void OnDisable()
    {
        if (swipeAction != null)
        {
            swipeAction.RemoveOnChangeListener(OnSwipeAction, inputSource);
        }
        if (touchAction != null)
        {
            touchAction.RemoveOnChangeListener(OnTouchAction, inputSource);
        }
    }


    private void OnTouchAction(SteamVR_Action_In action_In)
    {
        Debug.Log("touch");
    }

    private void OnSwipeAction(SteamVR_Action_In action_In)
    {
        if (touchAction.GetState(inputSource) && touchAction.GetLastState(inputSource)) // if the trackpad is already touched, since last update
        {
            Swipe(swipeAction.GetAxisDelta(inputSource)  * 45.0f);
        }
    }

    void Swipe(Vector2 input)
    {
        Debug.Log("swipe" + input);
        transform.localRotation = Quaternion.AngleAxis(input.x, Vector3.up) * transform.localRotation;
        transform.localRotation = Quaternion.AngleAxis(input.y, transform.localRotation * Vector3.right) * transform.localRotation;
    }
}
