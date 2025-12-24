using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIManagerMono : MonoBehaviour
{
    [SerializeField] private GameObject[] allUIButton;
    [SerializeField] private List<StateUI> stateUIs = new List<StateUI>()
    {
        new StateUI("Default"), new StateUI("OnlyDownPanel")
    };

    [Space]
    [SerializeField] private Button nextVisualButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Sprite backSprite;

    [Space]
    [SerializeField] private Image createImage;
    [SerializeField] private Sprite createSprite;
    [SerializeField] private TextMeshProUGUI createButtonText;

    private int stateHide;
    private StateUI lastSateUI;

    private void Start()
    {
        GameEntryGameplayCCh.input.OnBegan += BeganWork;
    }

    public void ChoiceStateUI (string id)
    {
        StateUI stateUI = stateUIs.FirstOrDefault((sui) => sui.ID == id);
        if (stateUI == null)
            return;
        ChoiceStateUI(stateUI);
    }

    private void ChoiceStateUI(StateUI stateUI)
    {
        foreach (var buttonGO in allUIButton)
        {
            buttonGO.SetActive(stateUI.needToAct.Contains(buttonGO));
        }

        lastSateUI = stateUI;
    }

    public void DoScreen()
    {
        StartCoroutine(TakeScreenshot($"{SaveLoadManager.CurrentMap}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png"));
        SoundManager.PlayInterfaceSFX(GameEntryGameplayCCh.DataContainer.ScreenshootSFX);
    }

    public IEnumerator TakeScreenshot(string fileName)
    {
        HideAll();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        byte[] png = tex.EncodeToPNG();
        Object.Destroy(tex);

        string path = Application.persistentDataPath + "/" + fileName;
        System.IO.File.WriteAllBytes(path, png);

        //Debug.Log("Screenshot saved: " + path);

        NativeGallery.SaveImageToGallery(png, "Draw A Snow City", fileName, (sucssec, path) => Debug.Log($"file save {sucssec}:\n {path}"));

        yield return new WaitForEndOfFrame();

        ReturnLast();
    }

    public void SwichUIState()
    {
        stateHide++;
        if (stateHide >= 3)
            stateHide = 0;

        switch (stateHide)
        {
            case 0:
                ChoiceStateUI("Default");
                break;
            case 1:
                ChoiceStateUI("OnlyDownPanel");
                break;
            case 2:
                HideAll();
                break;

            default:
                break;
        }
    }

    public void HideAll()
    {
        foreach (var item in allUIButton)
            item.SetActive(false);
    }

    public void ReturnLast()
    {
        ChoiceStateUI(lastSateUI);
    }

    private void BeganWork(Vector2 vector)
    {
        if (stateHide == 2)
            SwichUIState();
    }

    public void SelectObject(MovebleObject movebleObject)
    {
        nextVisualButton.interactable = movebleObject.MaxVisual > 0;
    }

    public void CreateState(bool isCreate)
    {
        createButtonText.text = isCreate ? "Back" : "Create";
        createImage.sprite = isCreate ? backSprite : createSprite;
    }
}

[Serializable]
public class StateUI
{
    public string ID;
    public GameObject[] needToAct;

    public StateUI(string iD)
    {
        ID = iD;
    }
}
