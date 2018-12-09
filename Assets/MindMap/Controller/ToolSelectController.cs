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
            if (axis.x < -0.4f)
                model.ActiveSlot -= 1;
            else if (axis.x > 0.4f)
                model.ActiveSlot += 1;
            UpdateView();
        }
    }

    void UpdateView()
    {
        previousView.targetTool = model[model.ActiveSlot - 1];
        nextView.targetTool = model[model.ActiveSlot + 1];
        inputController.handObject = model.ActiveTool.Tip;
    }
}
