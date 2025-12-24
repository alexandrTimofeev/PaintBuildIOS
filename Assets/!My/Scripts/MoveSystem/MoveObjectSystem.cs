using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public static class MoveObjectSystem
{
    private static SelectorMS selector;

    private static MovebleObject currentObject;

    public static MovebleObject CurrentObject  => currentObject;

    public static void Init()
    {
        selector = Object.FindFirstObjectByType<SelectorMS>(FindObjectsInactive.Include);

        selector.OnMovePosition += SelectorMovePositionWork;
        selector.OnMoveRotation += SelectorMoveRotationWork;
        selector.OnMoveScale += SelectorMoveScaleWork;
    }

    private static void SelectorMoveScaleWork(float scale)
    {
        selector.SetScaleView(scale);

        SetObjectScale(scale);
    }
    private static void SelectorMoveRotationWork(Vector3 euler)
    {
        selector.SetRotationView(Quaternion.Euler(euler));

        SetObjectRotation(euler);
    }
    private static void SelectorMovePositionWork(Vector3 point)
    {
        selector.SetPositionView(point);

        SetObjectPosition(point);
    }

    public static void SetObjectScale(float scale)
    {
        if (currentObject == null)
            return;

        currentObject.SetScale(scale);
    }
    public static void SetObjectRotation(Vector3 euler)
    {
        if (currentObject == null)
            return;

        currentObject.SetRotation(euler);
    }
    public static void SetObjectPosition(Vector3 point)
    {
        if (currentObject == null)
            return;

        currentObject.SetPosition(point);
    }

    public static void Select(MovebleObject movebleObject)
    {
        if (currentObject != null)
            currentObject.Select(false);

        movebleObject.Select(true);

        currentObject = movebleObject;
        selector.Select(movebleObject);

        GameEntryGameplayCCh.uIManagerMono.ChoiceStateUI($"Select");
        GameEntryGameplayCCh.uIManagerMono.SelectObject(movebleObject);
    }

    public static void Unselect()
    {
        if (currentObject == null)
            return;

        currentObject.Select(false);
        currentObject = null;

        selector.Hide();

        GameEntryGameplayCCh.uIManagerMono.ChoiceStateUI($"Unselect");
    }

    public static void DeliteCurrentObject()
    {
        Debug.Log($"DeliteCurrentObject {currentObject.name}");
        if (currentObject == null)
            return;

        GameObject go = currentObject.gameObject;
        Unselect();

        GameObject vfx = GameObject.Instantiate(GameEntryGameplayCCh.DataContainer.DeliteVFX,
            go.transform.position, go.transform.rotation);
        vfx.transform.localScale = Vector3.one;

        Object.Destroy(go);
        GameObject.Destroy(vfx, 10f);
    }
}
