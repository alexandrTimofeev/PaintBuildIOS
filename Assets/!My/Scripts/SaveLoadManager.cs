using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public List<SavebleObject> SavebleObjects = new List<SavebleObject>();

    public static string CurrentMap = "SnowCity";

    public Action<SaveData> OnSave;

    public void Init()
    {
        if (string.IsNullOrEmpty(CurrentMap))
        {
            CurrentMap = "SnowCity";
            DestrotyAllSavebleObjects();
            GameEntryGameplayCCh.weatherManager.Clear();
        }
        else
            LoadCurrentMap();
    }

    public static void LoadCurrentMap()
    {
        GameEntryGameplayCCh.saveLoadManager.Load(CurrentMap);
    }

    public void Load(string mapID)
    {
        SaveData saveData = LoadSaveData(mapID);
        if(saveData == null)
        {
            ErrorSave();
            return;
        }

        DestrotyAllSavebleObjects();
        CreateObjectFromPrefs(saveData);
        LoadWeather(saveData);
    }

    private void DestrotyAllSavebleObjects()
    {
        var sobs = SavebleObjects.ToArray();
        foreach (var item in sobs)        
            Destroy(item.gameObject);
        SavebleObjects.Clear();
    }

    public SaveData LoadSaveData(string mapID)
    {
        if (!PlayerPrefs.HasKey(mapID))
            return null;

        string json = PlayerPrefs.GetString(mapID);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"{json}");

        return saveData;
    }

    public void CreateObjectFromPrefs(SaveData saveData)
    {
        foreach (var data in saveData.saveObjectDatas)
        {
            CreateSaveObjectFromData(data);
        }

        FindParrnetForAll();
    }

    public void CreateSaveObjectFromData(SaveObjectData saveObjectData)
    {
        MovebleObject prefab =
            GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs
            .FirstOrDefault(mb => mb.ID == saveObjectData.ObjectID);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {saveObjectData.ObjectID}");
            return;
        }

        var moveObj = Instantiate(prefab, saveObjectData.Position, saveObjectData.Rotation);
        var saveble = moveObj.GetComponent<SavebleObject>();

        SavebleObjects.Add(saveble);

        ApplySaveDataToObject(saveble, saveObjectData, false);
    }


    public void ApplySaveDataToObject(SavebleObject savebleObject, SaveObjectData saveObjectData, bool findParrent = true)
    {
        savebleObject.ApplySaveData(saveObjectData, findParrent);
    }

    private void FindParrnetForAll()
    {
        Dictionary<string, SavebleObject> dict =
            SavebleObjects.ToDictionary(s => s.ID, s => s);

        foreach (var obj in SavebleObjects)
        {
            string parentID = obj.Data.ParentID;

            if (string.IsNullOrEmpty(parentID))
                continue;

            if (parentID.StartsWith("notID_"))
            {
                // Родитель есть, но это не SavebleObject — ставим null
                obj.transform.SetParent(null);
                continue;
            }

            if (dict.TryGetValue(parentID, out var parent))
            {
                obj.transform.SetParent(parent.transform);
            }
            else
            {
                // Если родителя с таким ID нет — null
                obj.transform.SetParent(null);
            }
        }
    }

    public void AddInList(SavebleObject savebleObject)
    {
        if (SavebleObjects.Contains(savebleObject))
            return;

        SavebleObjects.Add(savebleObject);
    }

    public void RemoveOutList(SavebleObject savebleObject)
    {
        SavebleObjects.Remove(savebleObject);
    }

    public static void Save(string mapID = null, SavebleObject[] objectsToSave = null)
    {
        if (string.IsNullOrEmpty(mapID))        
            mapID = CurrentMap;
        if (objectsToSave == null)
            objectsToSave = GameEntryGameplayCCh.saveLoadManager.SavebleObjects.ToArray();

        if (objectsToSave.Length == 0)
        {
            Debug.Log("No objects to save.");
            return;
        }

        SaveData saveData = new SaveData
        {
            saveObjectDatas = new SaveObjectData[objectsToSave.Length]
        };

        for (int i = 0; i < objectsToSave.Length; i++)
        {
            saveData.saveObjectDatas[i] = objectsToSave[i].UpdateAndGetData();
        }

        saveData.weatherState = GameEntryGameplayCCh.weatherManager.State;
        saveData.valueLightAngle = GameEntryGameplayCCh.weatherManager.RotationValue;
        saveData.valueLightExp = GameEntryGameplayCCh.weatherManager.LightValue;

        //ToJSON
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(mapID, json);
        PlayerPrefs.Save();

        Debug.Log($"Saved {objectsToSave.Length} objects to mapID: {mapID}\n{json}");

        GameEntryGameplayCCh.saveLoadManager?.OnSave?.Invoke(saveData);
    }

    private void LoadWeather(SaveData saveData)
    {
        GameEntryGameplayCCh.weatherManager.SetWatherState(saveData.weatherState);
        GameEntryGameplayCCh.weatherManager.SetSliderLight(saveData.valueLightExp);
        GameEntryGameplayCCh.weatherManager.SetSliderRotation(saveData.valueLightAngle);
        GameEntryGameplayCCh.weatherManager.SetSlidersValue(saveData.valueLightExp, saveData.valueLightAngle);
    }

    private void ErrorSave()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class SaveData
{
    public SaveObjectData[] saveObjectDatas;
    public float valueLightAngle = 0.5f;
    public float valueLightExp = 0.5f;
    public int weatherState;
}