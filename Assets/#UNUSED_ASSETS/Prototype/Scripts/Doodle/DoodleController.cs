using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodleController : MonoBehaviour {

    public Transform _world;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        DoodlePen[] pens = _world.GetComponentsInChildren<DoodlePen>();
        Bubble[] bubbles = _world.GetComponentsInChildren<Bubble>();
        foreach (DoodlePen pen in pens)
        {
            
            Vector3 worldPen = pen.transform.position;
            Bubble target = null;
            float closest = float.PositiveInfinity;
            foreach (Bubble bubble in bubbles)
            {
                Vector3 localPen = bubble.transform.worldToLocalMatrix.MultiplyPoint(worldPen);
                float bubbleRadius = bubble.radiusScale * 0.5f;
                if (localPen.sqrMagnitude < bubbleRadius * bubbleRadius) // only select a bubble that the pen is inside
                {
                    float distance = localPen.magnitude;
                    if (distance < closest) // select the closest bubble
                    {
                        closest = distance;
                        target = bubble;
                    }
                }
            }

            pen.targetBubble = target;
        }
	}
}
