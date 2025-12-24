using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnCreatesButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI tmp;

    [Space]
    [SerializeField] private Sprite spriteSelect;
    [SerializeField] private Sprite spriteUnselect;

    private MovebleObject movebleObject;
    public MovebleObject MovebleObject => movebleObject;

    public Action<ButtonActionInfo> OnClick;

    public void Init (MovebleObject movebleObject)
    {
        this.movebleObject = movebleObject;

        GetComponent<SpecialButtonMS>().action = new ButtonActionInfo()
        {
            action = ButtonMSAction.CreateObjectStr,
            text = movebleObject.ID
        };
        GetComponent<SpecialButtonMS>().OnClick += ClickWork;
        SetSelect(false);

        tmp.text = string.IsNullOrEmpty(movebleObject.Title) ? movebleObject.ID : movebleObject.Title;
    }

    private void ClickWork(ButtonActionInfo info)
    {
        OnClick?.Invoke(info);
    }

    public void SetSelect (bool select)
    {
        image.sprite = select ? spriteSelect : spriteUnselect;
    }
}