using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(SteamVR_Behaviour_Pose))]
public class Grabber : XformMonoBehaviour
{
    private const float grabThreshold = 0.9f;

    public float handSize = 0.2f;

    private SteamVR_Input_Sources inputSource; // right or left hand

    private Grabbable grabbing;
    private Activatable activating;

    private Vector3 relativePosition;
    private Vector3 relativeUp;
    private Vector3 relativeForward;

    public SteamVR_Action_Boolean grabAction; //Grab Pinch is the trigger, select from inspecter
    public SteamVR_Action_Boolean activateAction; //Grab Pinch is the trigger, select from inspecter

    private SteamVR_Action_Pose currentPose;

    // Use this for initialization
    void Start () {
        SteamVR_Behaviour_Pose pose = GetComponent<SteamVR_Behaviour_Pose>();
        pose.onTransformChanged.AddListener(UpdatePose);
        pose.onTransformUpdated.AddListener(UpdatePose);
        inputSource = pose.inputSource;
	}

    void UpdatePose(SteamVR_Action_Pose pose)
    {
        currentPose = pose;
    }

    void OnEnable()
    {
        if (grabAction != null)
        {
            grabAction.AddOnChangeListener(OnGrabActionPressedOrReleased, inputSource);
        }
        if (activateAction != null)
        {
            activateAction.AddOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
        }
    }


    private void OnDisable()
    {
        if (grabAction != null)
        {
            grabAction.RemoveOnChangeListener(OnGrabActionPressedOrReleased, inputSource);
        }
        if (activateAction != null)
        {
            activateAction.RemoveOnChangeListener(OnActivateActionPressedOrReleased, inputSource);
        }
    }


    private void OnGrabActionPressedOrReleased(SteamVR_Action_In action_In)
    {
        if (grabAction.GetStateDown(inputSource))
            Grab();
        if (grabAction.GetStateUp(inputSource))
            Release();
    }

    private void OnActivateActionPressedOrReleased(SteamVR_Action_In action_In)
    {
        if (activateAction.GetStateDown(inputSource))
            Activate();
        if (activateAction.GetStateUp(inputSource))
            Deactivate();
    }

    // Update is called once per frame
    void Update () {
        Drag(currentPose);
    }

    private void OnPreRender()
    {
        Drag(currentPose);
    }

    private float grabRadius
    {
        get {
            float radius = handSize;
            {
                Vector3 scale = transform.lossyScale;
                radius *= Mathf.Min(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            }
            return radius;
        }
    }

    private void Grab()
    {
        if (grabbing != null)
            return;

        Collider[] targets = new Collider[4];
        
        targets = Physics.OverlapSphere(xform.position, grabRadius);
        Collider c = targets.FirstOrDefault(t => t.GetComponent<Grabbable>() != null);
        if (c == null)
            return; // nothing to grab
        Grabbable grab = c.GetComponent<Grabbable>(); // grab this
        Release();
        Vector3 relP = xform.worldToLocalMatrix.MultiplyPoint(grab.xform.position);
        Vector3 relU = xform.worldToLocalMatrix.MultiplyVector(grab.xform.localToWorldMatrix.GetColumn(1));
        Vector3 relF = xform.worldToLocalMatrix.MultiplyVector(grab.xform.localToWorldMatrix.GetColumn(2));
        grabbing = grab;
        relativePosition = relP;
        relativeUp = relU;
        relativeForward = relF;
        Deactivate(); // deactivate while grabbing
        grabbing.GrabGrabbable(this);
    }

    private void Release()
    {
        if (grabbing == null)
            return;
        grabbing.ReleaseGrabbable(this);
        grabbing = null;
    }

    private void Activate()
    {
        if (grabbing != null)
            return;

        Collider[] targets = new Collider[4];

        targets = Physics.OverlapSphere(xform.position, grabRadius);
        Collider c = targets.FirstOrDefault(t => t.GetComponent<Activatable>() != null);
        if (c == null)
            return; // nothing to grab
        Activatable trigger = c.GetComponent<Activatable>(); // grab this
        Deactivate();
        Vector3 relP = xform.worldToLocalMatrix.MultiplyPoint(trigger.xform.position); // trigger position vector to local space
        Vector3 relU = xform.worldToLocalMatrix.MultiplyVector(trigger.xform.localToWorldMatrix.GetColumn(1)); // trigger up vector to local space
        Vector3 relF = xform.worldToLocalMatrix.MultiplyVector(trigger.xform.localToWorldMatrix.GetColumn(2)); // trigger forward vector to local space
        activating = trigger;
        relativePosition = relP;
        relativeUp = relU;
        relativeForward = relF;
        activating.ActivateActivatable(this);
    }

    private void Deactivate()
    {
        if (activating == null)
            return;
        activating.DeactivateActivatable(this);
        activating = null;
    }

    public void Drag(SteamVR_Action_Pose pose)
    {
        if (grabbing == null)
            return;

        // apply the position and rotation of grabbed relative to grabber, from when it was first grabbed
        Vector3 gp = xform.localToWorldMatrix.MultiplyPoint(relativePosition); // global position of grabbed, relative to grabbber
        Quaternion gr = Quaternion.LookRotation(xform.localToWorldMatrix.MultiplyVector(relativeForward), xform.localToWorldMatrix.MultiplyVector(relativeUp)); // global rotation of grabbed, relative to grabbber

        grabbing.xform.position = gp;
        grabbing.xform.rotation = gr;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = grabbing == null ? Color.red : Color.green;
        Gizmos.DrawWireSphere(xform.position, grabRadius);
    }
}
