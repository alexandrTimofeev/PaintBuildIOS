using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonSFXType { Standart, StandartClose, ID, SetHere }
public class ButtonSFX : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ButtonSFXType typeClick;
    [SerializeField] private SFXPlayPreset presetClick;

    [Space]
    [SerializeField] private ButtonSFXType typeNonInteract;
    [SerializeField] private SFXPlayPreset presetNonInteract;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() => ClickSFX());
    }

    private void ClickSFX()
    {
        switch (typeClick)
        {
            case ButtonSFXType.Standart:
                SoundManager.PlayInterfaceSFX(UiSfxType.ClickOpen);
                break;
            case ButtonSFXType.StandartClose:
                SoundManager.PlayInterfaceSFX(UiSfxType.ClickClose);
                break;
            case ButtonSFXType.ID:
                SoundManager.PlayInterfaceSFX(presetClick.ID);
                break;
            case ButtonSFXType.SetHere:
                SoundManager.PlayInterfaceSFX(presetClick);
                break;
            default:
                SoundManager.PlayInterfaceSFX(UiSfxType.ClickOpen);
                break;
        }
    }

    private void ClickNonInteractible()
    {
        switch (typeNonInteract)
        {
            case ButtonSFXType.Standart:
                SoundManager.PlayInterfaceSFX(UiSfxType.NonInteract);
                break;
            case ButtonSFXType.StandartClose:
                SoundManager.PlayInterfaceSFX(UiSfxType.NonInteract);
                break;
            case ButtonSFXType.ID:
                SoundManager.PlayInterfaceSFX(presetNonInteract.ID);
                break;
            case ButtonSFXType.SetHere:
                SoundManager.PlayInterfaceSFX(presetNonInteract);
                break;
            default:
                SoundManager.PlayInterfaceSFX(UiSfxType.NonInteract);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.interactable)        
            ClickNonInteractible();        
    }
}