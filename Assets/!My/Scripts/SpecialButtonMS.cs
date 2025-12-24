using System;
using UnityEngine;
using UnityEngine.UI;

public class SpecialButtonMS : MonoBehaviour
{
    public ButtonActionInfo action;
    public Action<ButtonActionInfo> OnClick;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Click());
        ButtonMediator.AddButton(this);
    }

    private void Click()
    {
        OnClick?.Invoke(action);
    }

    private void OnDestroy()
    {
        ButtonMediator.RemoveButton(this);
    }
}
