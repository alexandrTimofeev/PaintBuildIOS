using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideViewObject : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();
    [SerializeField] private Tap3DButton hideViewButton;
    private List<Material> materials = new List<Material>();
    private List<Material> materialsAlpha = new List<Material>();

    private int state;

    private void Start()
    {
        if (renderers.Count == 0)
            renderers.Add(GetComponent<Renderer>());

        foreach (var render in renderers)
        {
            materials.Add(render.material);
            materialsAlpha.Add(new Material(render.material) { color = render.material.color - new Color(0, 0, 0, 0.5f) });
        }

        if (hideViewButton == null)
            hideViewButton = Instantiate(Resources.Load<Tap3DButton>("Prefabs/Tap3DButton"), transform.position, transform.rotation);
        hideViewButton.OnClick.AddListener(() => SwitchState());
        hideViewButton.ID = "HideWall";
        hideViewButton.gameObject.SetActive(false); 
    }

    public void SetState (int state)
    {
        this.state = state;
        Debug.Log($"state {state}");

        switch (state)
        {
            case 0:
                    for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].enabled = true;
                    renderers[i].material = materials[i];
                }

                ForChildRenderers((r) =>
                {
                    r.gameObject.SetActive(true);
                });
                break;

            case 1:
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].enabled = false;
                }

                ForChildRenderers((r) =>
                {
                    r.gameObject.SetActive(false);
                });
                break;

            default:
                break;
        }
    }

    private void ForChildRenderers(Action<Renderer> actionForChild)
    {
        foreach(Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.transform.childCount; i++)
            {
                if (renderer.transform.GetChild(i).TryGetComponent(out Renderer rendererChild) && renderers.Contains(rendererChild) == false)
                {
                    actionForChild?.Invoke(rendererChild); 
                }
            }
        }
    }

    public void SwitchState()
    {
        if (state == 1)
            state = -1;

        SetState(state + 1);
    }
}