using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SavebleObject : MonoBehaviour
{
    public string ID;
    public MovebleObject movebleObject;
    private SaveObjectData data;

    public SaveObjectData Data  => data;

    private void Start()
    {
        if (Data == null)
            InitFirst();

        GameEntryGameplayCCh.saveLoadManager.AddInList(this);
    }

    public void InitFirst()
    {
        ID = System.Guid.NewGuid().ToString();
        if (movebleObject == null)
            movebleObject = GetComponent<MovebleObject>();

        UpdateAndGetData();
    }

    public void ApplySaveData (SaveObjectData data, bool findParrent = true)
    {
        this.data = data;

        ID = data.ID;

        if (movebleObject == null)
            movebleObject = GetComponent<MovebleObject>();

        Transform t = movebleObject.transform;
        t.SetParent(null);

        t.position = data.Position;
        t.rotation = data.Rotation;
        t.localScale = data.Scale;

        movebleObject.SetVisual(data.VisualStep);

        if (findParrent == false)
            return;

        // Восстановление родителя
        if (string.IsNullOrEmpty(data.ParentID) == false)
        {
            SavebleObject[] all = GameEntryGameplayCCh.saveLoadManager.SavebleObjects.ToArray();
            foreach (var s in all)
            {
                if (s.ID == data.ParentID)
                {
                    t.SetParent(s.transform);
                    break;
                }
            }
        }
    }

    public SaveObjectData UpdateAndGetData()
    {
        if (movebleObject == null)
            movebleObject = GetComponent<MovebleObject>();

        if (string.IsNullOrEmpty(ID))
            ID = System.Guid.NewGuid().ToString();

        Transform t = movebleObject.transform;

        string parentID = string.Empty;
        if (t.parent != null)
        {
            if (t.parent.TryGetComponent(out SavebleObject parentSaveble))
            {
                parentID = parentSaveble.ID;
            }
            else
            {
                // Сохраняем "notID_" + имя объекта, чтобы знать, что есть родитель, но он не SavebleObject
                parentID = $"notID_{t.parent.name}";
            }
        }

        data = new SaveObjectData(
            ID,
            t.position,
            t.rotation,
            t.lossyScale,
            movebleObject.CurrentVisual,
            parentID,
            movebleObject.ID
        );

        return data;
    }

    private void OnDestroy()
    {
        GameEntryGameplayCCh.saveLoadManager.RemoveOutList(this);
    }
}

[Serializable]
public class SaveObjectData
{
    public string ID;
    public string ObjectID;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public int VisualStep;
    public string ParentID;

    public SaveObjectData()  { }

    public SaveObjectData(string iD, Vector3 position, Quaternion rotation, Vector3 scale, int visualStep, string parentID, string objectID)
    {
        ID = iD;
        Position = position;
        Rotation = rotation;
        Scale = scale;
        VisualStep = visualStep;
        ParentID = parentID;
        ObjectID = objectID;
    }
}