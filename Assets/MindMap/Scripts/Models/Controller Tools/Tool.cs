using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Tool : MonoBehaviour {

    public Renderer[] coloredGeometry = new Renderer[1];
    public TheWorld.InteractionEvent onInteraction = new TheWorld.InteractionEvent();
    public TheWorld.InteractionEvent offInteraction = new TheWorld.InteractionEvent();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private Material ColoredMaterial(Renderer r)
    {
        if (r == null)
            return null;
        if (Application.isPlaying)
            return r.material;
        else
            return r.sharedMaterial;
    }

    public InputController Controller {
        get { return GetComponentInParent<InputController>(); }
    }

    public Color ToolColor
    {
        get
        {
            foreach (Renderer r in coloredGeometry)
                if (r != null)
                {
                    Material m = ColoredMaterial(r);
                    if (m != null)
                        return m.color;
                }
            return Color.white;
        }
        set
        {
            foreach (Renderer r in coloredGeometry)
                if (r != null)
                {
                    Material m = ColoredMaterial(r);
                    if (m != null)
                        m.color = value;
                }
        }
    }

    public Transform Tip
    {
        get { return transform.Find("Tip"); }
    }

    [CustomEditor(typeof(Tool))]
    public class ToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Tool myTarget = (Tool)target;
            myTarget.ToolColor = EditorGUILayout.ColorField("Tool Color", myTarget.ToolColor);
        }
    }
}
