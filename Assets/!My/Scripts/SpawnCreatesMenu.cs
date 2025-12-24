using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnCreatesMenu : MonoBehaviour
{
    [SerializeField] private SpawnCreatesButton buttonPref;
    [SerializeField] private RectTransform content1;
    [SerializeField] private RectTransform content2;
    [SerializeField] private RectTransform contentGlobal;
    [SerializeField] private TextMeshProUGUI txtPref;

    private List<SpawnCreatesButton> buttons = new List<SpawnCreatesButton>();

    void Start()
    {
        InstanceButtons();
    }

    /*private void OnEnable()
    {
        foreach (var createsButton in buttons)
        {
            createsButton.SetSelect(false);
        }
    }*/

    private void InstanceButtons()
    {
        List<MovebleObject> mobs = new List<MovebleObject>();
        List<MovebleObject> mobs2 = new List<MovebleObject>();
        for (int i = 0; i < GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs.Count; i++)
        {
            MovebleObject item = GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs[i];
            if (item.isWallObject)
                mobs2.Add(item);
            else
                mobs.Add(item);
        }

        CreateText("Ground", content1);

        for (int i = 0; i < mobs.Count; i++)
        {
            MovebleObject item = mobs[i];
            CreateButton(item, content1);
        }

        CreateText("Wall", content2);

        for (int i = 0; i < mobs2.Count; i++)
        {
            MovebleObject item = mobs2[i];
            CreateButton(item, content2);
        }
    }

    private void CreateText(string text, Transform content)
    {
        TextMeshProUGUI textMesh = Instantiate(txtPref, content.position, Quaternion.identity, content);
        textMesh.transform.localPosition += (Vector3.down * 20f);
        textMesh.text = text;
    }

    private void CreateButton(MovebleObject item, Transform content)
    {
        if (item == null || buttonPref == null)
            return;

        SpawnCreatesButton buttonSpawn = Instantiate(buttonPref, content);
        buttonSpawn.Init(item);
        buttonSpawn.OnClick += ClickButtonWork;

        buttons.Add(buttonSpawn);
    }

    private void ClickButtonWork(ButtonActionInfo info)
    {
        foreach (var createsButton in buttons)
        {
            createsButton.SetSelect(info.text == createsButton.MovebleObject.ID);
        }
    }
}
