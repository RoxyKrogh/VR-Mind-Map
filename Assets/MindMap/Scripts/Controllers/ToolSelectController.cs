using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelectController : MonoBehaviour {

    public EquipmentSlot model;
    public EquipmentSlotView nextView;
    public EquipmentSlotView previousView;
    public GameObject controlsPrompt;

    public InputController inputController;

    // Use this for initialization
    void Start () {
        inputController.onSelect.AddListener(OnSelectButton);
        inputController.onActivate.AddListener(OnInteract);
        UpdateView();
    }
	
	// Update is called once per frame
	void Update () {
        bool isTouched = inputController.IsTouched;
        previousView.isHidden = !isTouched;
        nextView.isHidden = !isTouched;
        controlsPrompt.SetActive(isTouched);
    }

    void OnSelectButton(InputControlState state, bool isPress)
    {
        if (isPress)
        {
            Vector2 axis = state.AxisPosition;
            const float THRESHOLD = 0.4f;
            if (Mathf.Abs(axis.x) > THRESHOLD)
                Interact(false); // stop interaction when changing tools
            if (axis.x < -THRESHOLD) // previous tool
                model.ActiveSlot -= 1;
            else if (axis.x > THRESHOLD) // next tool
                model.ActiveSlot += 1;
            UpdateView();
        }
    }

    void OnInteract(InputControlState state, bool isPress)
    {
        Interact(isPress);
    }

    void Interact(bool isOn)
    {
        if (model.ActiveTool == null)
            return;
        if (isOn)
            model.ActiveTool.onInteraction.Invoke(model.ActiveTool.Tip);
        else
            model.ActiveTool.offInteraction.Invoke(model.ActiveTool.Tip);
    }

    void UpdateView()
    {
        previousView.targetTool = model[model.ActiveSlot - 1];
        nextView.targetTool = model[model.ActiveSlot + 1];
        inputController.handObject = model.ActiveTool.Tip;
    }
}
