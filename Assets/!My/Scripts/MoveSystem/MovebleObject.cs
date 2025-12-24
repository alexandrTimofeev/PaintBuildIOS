using System;
using UnityEngine;

public class MovebleObject : MonoBehaviour
{
    public string ID;
    public string Title;
    public bool isFloorObject = true;
    public bool isWallObject = false;
    public bool isUseNormal = true;

    [Space]
    [SerializeField] private GameObject[] visuals;
    private int currentVisual = 0;

    private int layerStart;

    public int CurrentVisual => currentVisual;
    public int MaxVisual => visuals.Length;

    private void Awake()
    {
        layerStart = gameObject.layer;
    }

    public void Select(bool isSelect)
    {
        gameObject.layer = isSelect ? LayerMask.NameToLayer("Select") : layerStart;
    }

    public Vector3 GetPosition()
    {
        return transform.position;  
    }
    public Quaternion GetRotation()
    {
        return Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }
    public float GetScaleValue()
    {
        //return Mathf.Clamp(transform.localScale.x * (transform.parent ? transform.parent.localScale.x : 1f), 0.5f, 99f);
        return transform.localScale.x * (transform.parent ? transform.parent.lossyScale.x : 1f);
    }

    public void SetPosition(Vector3 point)
    {
        transform.position = point;
    }
    public void SetRotation(Vector3 euler)
    {
        transform.rotation = Quaternion.Euler(euler);
    }
    public void SetScale(float scale)
    {
        Vector3 parentScale = transform.parent ? transform.parent.lossyScale : Vector3.one;
        transform.localScale = new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z) * scale;
    }

    public void NextVisual()
    {
        currentVisual++;
        if(currentVisual >= visuals.Length)
            currentVisual = 0;

        SetVisual(currentVisual);
    }

    public void SetVisual (int visual)
    {
        if(visuals.Length == 0) 
            return;

        currentVisual = visual;
        for (int i = 0; i < visuals.Length; i++)
            visuals[i].SetActive(currentVisual == i);

        GameObject vfx = GameObject.Instantiate(GameEntryGameplayCCh.DataContainer.VisualVFX,
            transform.position, transform.rotation);
        GameObject.Destroy(vfx, 10f);
    }
}
