using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public interface InputControlState
{
    bool IsGrabbing { get; }
    bool IsActivating { get; }
    bool IsSelecting { get; }
    Vector2 AxisPosition { get; }
    Vector2 AxisDelta { get; }
    Transform HandObject { get; }
    T GetComponent<T>();
}

// Attach this component to a SteamVR controller/hand object
[RequireComponent(typeof(SteamVR_Behaviour_Pose))]
public class InputController : MonoBehaviour, InputControlState
{
    /** An event carrying the transform of a hand, and whether this action is being entered or exited (true/false). */
    [System.Serializable]
    public class InputToggleAction : UnityEvent<InputControlState, bool> { };
    /** An event carrying a 2D vector. */
    [System.Serializable]
    public class Input2DAction : UnityEvent<InputControlState, Vector2> { };
    
    [System.Serializable]
    public class InputMoveAction : UnityEvent<InputControlState> { };

    [Tooltip("Invoked when the touchpad is swiped.")]
    public Input2DAction onSwipe = new Input2DAction();

    [Tooltip("Invoked when the select button is pressed or released.")]
    public InputToggleAction onSelect = new InputToggleAction();

    [Tooltip("Invoked when the grab button is pressed or released.")]
    public InputToggleAction onGrab = new InputToggleAction();

    [Tooltip("Invoked when the activate button is pressed or released.")]
    public InputToggleAction onActivate = new InputToggleAction();

    [Tooltip("Invoked when the hand is moved.")]
    public InputMoveAction onMoved = new InputMoveAction();

    [Tooltip("This object is passed with events as the position of this hand.")]
    public Transform handObject = null;

    /*  SteamVR event bindings */
    // Trackpad events
    [Tooltip("Bind to touchpad position")]
    public SteamVR_Action_Vector2 swipeAction; // swipe the trackpad

    [Tooltip("Bind to touchpad touch")]
    public SteamVR_Action_Boolean touchAction; // touch the trackpad
    
    // Button events
    [Tooltip("Bind to touchpad press")]
    public SteamVR_Action_Boolean selectAction; //Grab Grip

    [Tooltip("Bind to grip pull")]
    public SteamVR_Action_Boolean grabAction; //Grab Grip

    [Tooltip("Bind to trigger pull")]
    public SteamVR_Action_Boolean activateAction; //Grab Pinch (trigger pull)
    

    private SteamVR_Input_Sources inputSource; // right or left hand

    private bool _boundEvents = false;

    public bool IsTouched
    {
        get
        {
            return touchAction.GetState(inputSource);
        }
    }

    public bool IsGrabbing
    {
        get
        {
            return grabAction.GetState(inputSource);
        }
    }

    public bool IsActivating
    {
        get
        {
            return activateAction.GetState(inputSource);
        }
    }

    public bool IsSelecting
    {
        get
        {
            return selectAction.GetState(inputSource);
        }
    }

    public Transform HandObject
    {
        get
        {
            return handObject == null ? transform : handObject;
        }
    }

    public Vector2 AxisPosition
    {
        get
        {
            return swipeAction.GetAxis(inputSource);
        }
    }

    public Vector2 AxisDelta
    {
        get
        {
            return swipeAction.GetAxisDelta(inputSource);
        }
    }

    // Use this for initialization
    void Start () {
        SteamVR_Behaviour_Pose pose = GetComponent<SteamVR_Behaviour_Pose>();
        inputSource = pose.inputSource;
        pose.onConnectedChanged.AddListener(OnConnectedChanged);
    }

    void OnEnable()
    {
        // bind events
        SteamVR_Behaviour_Pose pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (!_boundEvents)
        {
            _boundEvents = true;
            Debug.Log("Binding controller events for " + inputSource);
            pose.onTransformChanged.AddListener(OnHandMoved);
            grabAction.AddOnChangeListener(OnGrabActionPressedOrReleased, inputSource);
            activateAction.AddOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
            selectAction.AddOnChangeListener(OnSelectActionPressedOrReleased, inputSource);
            touchAction.AddOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
            swipeAction.AddOnChangeListener(OnSwipeAction, inputSource);
        }
    }


    void OnDisable()
    {
        if (_boundEvents)
        {
            _boundEvents = false;
            Debug.Log("Unbinding controller events for " + inputSource);
            // unbind events
            SteamVR_Behaviour_Pose pose = GetComponent<SteamVR_Behaviour_Pose>();
            pose.onTransformChanged.RemoveListener(OnHandMoved);
            grabAction.RemoveOnChangeListener(OnGrabActionPressedOrReleased, inputSource);
            activateAction.RemoveOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
            selectAction.RemoveOnChangeListener(OnSelectActionPressedOrReleased, inputSource);
            touchAction.RemoveOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
            swipeAction.RemoveOnChangeListener(OnSwipeAction, inputSource);
        }
    }

    void OnConnectedChanged(SteamVR_Action_Pose pose)
    {
        bool wasConnected = pose.GetLastDeviceIsConnected(inputSource);
        bool isConnected = pose.GetDeviceIsConnected(inputSource);
        if (isConnected && !wasConnected)
            OnEnable();
        else if (wasConnected && !isConnected)
            OnDisable();
    }

    // Update is called once per frame
    void Update () {

    }

    private void OnHandMoved(SteamVR_Action_Pose pose)
    {
        onMoved.Invoke(this);
    }

    private void OnSwipeAction(SteamVR_Action_In action_In)
    {
        // if this is not the first event since the trackpad was touched (do not swipe from default position {0,0}).
        if (touchAction.GetLastState(inputSource) && IsTouched) 
            onSwipe.Invoke(this, swipeAction.GetAxisDelta(inputSource));
    }

    private void OnGrabActionPressedOrReleased(SteamVR_Action_In action_In)
    {
        onGrab.Invoke(this, grabAction.GetState(inputSource)); // grab state changed
    }

    private void OnActivateActionPressedOrReleased(SteamVR_Action_In action_In)
    {
        onActivate.Invoke(this, activateAction.GetState(inputSource)); // activate state changed
    }

    private void OnSelectActionPressedOrReleased(SteamVR_Action_In action_In)
    {
        onSelect.Invoke(this, selectAction.GetState(inputSource)); // select state changed
    }
}
