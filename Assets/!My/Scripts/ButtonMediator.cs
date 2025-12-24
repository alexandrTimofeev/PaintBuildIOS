using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonMSAction { None, Delite, CreateObjectNum, CreateObjectStr, SwitchLayerMode, SwitchInstrument, NextVisual, Save, Load, NextWeather,
HideUI, Screeshot};

public class ButtonMediator : MonoBehaviour
{
    public Action<ButtonActionInfo> OnButtonClick;

    private static List<SpecialButtonMS> listButtons = new List<SpecialButtonMS>();
    private static ButtonMediator instance;

    public ButtonMediator()
    {
        instance = this;
        listButtons.Clear();
        //listButtons.AddRange(FindObjectsByType<SpecialButtonMS>(FindObjectsInactive.Include, FindObjectsSortMode.None));
    }

    public void ClickEvent(ButtonActionInfo action)
    {
        OnButtonClick?.Invoke(action);
    }

    public static void AddButton(SpecialButtonMS specialButtonMS)
    {
        listButtons.Add(specialButtonMS);
        specialButtonMS.OnClick += instance.ClickEvent;
    }

    public static void RemoveButton(SpecialButtonMS specialButtonMS)
    {
        listButtons.Remove(specialButtonMS);
        specialButtonMS.OnClick -= instance.ClickEvent;
    }
}

[Serializable]
public class ButtonActionInfo
{
    public ButtonMSAction action;
    public int number;
    public string text;
    public Vector3 point;
}