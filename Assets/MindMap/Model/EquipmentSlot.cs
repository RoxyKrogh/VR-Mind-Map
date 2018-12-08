using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EquipmentSlot : MonoBehaviour {

    public Tool[] inventory = new Tool[1];
    private int _activeSlot = 0;

    public int ActiveSlot
    {
        get { return _activeSlot; }
        set
        {
            EquipTool(value);
        }
    }

    public Tool this[int index]
    {
        get
        {
            if (inventory.Length == 0)
                return null;
            return inventory[Mathf.Abs(index % inventory.Length)];
        }
    }

    public Tool ActiveTool
    {
        get { return this[_activeSlot]; }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void EquipTool(int slot)
    {
        if (inventory.Length == 0 || slot == _activeSlot)
            return;
        Tool active = ActiveTool;
        Tool next = this[slot];
        if (active != null)
        {
            this[_activeSlot].gameObject.SetActive(false);
        }
        _activeSlot = Mathf.Abs(slot % inventory.Length);
        if (next != null)
            this[_activeSlot].gameObject.SetActive(true);
    }

    [CustomEditor(typeof(EquipmentSlot))]
    public class EquipmentSlotEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EquipmentSlot myTarget = (EquipmentSlot)target;
            myTarget.ActiveSlot = EditorGUILayout.IntField("Active Slot", myTarget.ActiveSlot);
            string toolName = "None";
            if (myTarget.ActiveTool != null)
                toolName = myTarget.ActiveTool.name;
            EditorGUILayout.LabelField("Active Tool", toolName);
        }
    }
}
