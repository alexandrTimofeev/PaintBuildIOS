using UnityEngine;

public class LayerObject : MonoBehaviour
{
    public int layer;

    public static int LastAllLayer;

    public void SeeLayerUpdate (int layer)
    {
        gameObject.SetActive(this.layer >= layer);
    }

    public static void SeeLayerUpdateAll (int layer)
    {
        LayerObject[] layerObjects = Object.FindObjectsByType<LayerObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var layerObject in layerObjects)
        {
            layerObject.SeeLayerUpdate(layer);
        }

        Debug.Log($"layerStart {layer}");
        LastAllLayer = layer;
    }

    public static void SeeLayerUpdateAllSwitch()
    {

        if (LastAllLayer > 2)
            SeeLayerUpdateAll(0);
        else
            SeeLayerUpdateAll(LastAllLayer + 1);
    }
}
