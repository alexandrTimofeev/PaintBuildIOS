using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Tap3DButton : MonoBehaviour
{
    public string ID;

    public UnityEvent OnClick;

    public void Click()
    {
        OnClick?.Invoke();
    }

    public static void ViewButtonID (string id, bool view)
    {
        Tap3DButton[] buttons = FindObjectsByType<Tap3DButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log($"tap {id} {view}");
        foreach (var tapButton in buttons)
        {
            if(tapButton.ID == id)
                tapButton.gameObject.SetActive(view);
        }
    }
}
