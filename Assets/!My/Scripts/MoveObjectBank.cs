using System;
using System.Collections;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;

public static class MoveObjectBank
{
    public static MovebleObject CurrentCreateObject;

    public static void CreateObject(int n, Vector3 point)
    {
        CreateObject(GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs[n], point);
    }

    public static void CreateObject(string id, Vector3 point)
    {
        CreateObject(GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs.First((mb) => mb.ID == id), point);
    }

    public static void CreateObject(MovebleObject movebleObject, Vector3 point, Vector3? normal = null, Transform parent = null)
    {
        Vector3 realFwd = Vector3.forward;
        Vector3 realNormal = Vector3.up;
        if (movebleObject.isUseNormal && normal != null)
        {
            if (movebleObject.isWallObject)
            {
                realFwd = normal.Value;
                realNormal = Vector3.up;
            }
            else
            {
                realFwd = Vector3.Cross(normal.Value, Vector3.right);
                realNormal = normal.Value;
            }
        }

        var moveOb = UnityEngine.Object.Instantiate(movebleObject, point,
            movebleObject.isUseNormal ? Quaternion.LookRotation(realFwd, realNormal) : Quaternion.identity);
        if (parent != null)
            moveOb.transform.parent = parent.transform;
        //MoveObjectSystem.Select(moveOb);

        GameObject ngo = GameObject.Instantiate(GameEntryGameplayCCh.DataContainer.CreateVFX,
            moveOb.transform.position, moveOb.transform.rotation, moveOb.transform);
        ngo.transform.localScale = Vector3.one;

        GameObject.Destroy(ngo, 10f);
    }

    public static void CreateCurrentObject(Vector3 point, Vector3 normal, Transform parent = null)
    {
        CreateObject(CurrentCreateObject, point, normal, parent);
    }

    public static void ChoiceCreateObject(int n)
    {
        SetCreateObject(GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs[n]);
    }

    public static void ChoiceCreateObject(string id)
    {
        SetCreateObject(GameEntryGameplayCCh.DataContainer.movebleObjectsPrefs.First((mb) => mb.ID == id));
    }

    public static void ClearCreateObject()
    {
        SetCreateObject(null);
    }

    public static void SetCreateObject(MovebleObject movebleObject)
    {
        CurrentCreateObject = movebleObject;
    }
}
