using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrientationDetector : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<bool> onPortraitCheck;
    public UnityEvent<bool> onLandscapeCheck;

    [Header("Portrait")]
    public UnityEvent onAnyPortrait;
    public UnityEvent onPortraitUp;
    public UnityEvent onPortraitUpsideDown;

    [Header("Landscape")]
    public UnityEvent onLandscape;
    public UnityEvent onLandscapeLeft;
    public UnityEvent onLandscapeRight;

    private ScreenOrientation _lastOrientation;

    private List<OrientationDetectorObject> orientationObjects = new();

    private void Start()
    {
        _lastOrientation = Screen.orientation;
        FindAllOrientationObjects();
        InvokeOrientationEvent(_lastOrientation);
    }

    private void Update()
    {
        if (Screen.orientation != _lastOrientation)
        {
            _lastOrientation = Screen.orientation;
            InvokeOrientationEvent(_lastOrientation);
        }
    }

    private void InvokeOrientationEvent(ScreenOrientation orientation)
    {
        bool isPortrait = orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown;

        // Стандартные UnityEvent
        switch (orientation)
        {
            case ScreenOrientation.Portrait:
                onPortraitUp?.Invoke();
                onAnyPortrait?.Invoke();
                break;
            case ScreenOrientation.PortraitUpsideDown:
                onPortraitUpsideDown?.Invoke();
                onAnyPortrait?.Invoke();
                break;
            case ScreenOrientation.LandscapeLeft:
                onLandscape?.Invoke();
                onLandscapeLeft?.Invoke();
                break;
            case ScreenOrientation.LandscapeRight:
                onLandscape?.Invoke();
                onLandscapeRight?.Invoke();
                break;
        }

        onPortraitCheck?.Invoke(isPortrait);
        onLandscapeCheck?.Invoke(!isPortrait);

        // Вызов всех объектов, даже если они выключены
        foreach (var obj in orientationObjects)
        {
            obj.OnOrientationChanged(orientation);
        }
    }

    public void Register(OrientationDetectorObject obj)
    {
        if (!orientationObjects.Contains(obj))
            orientationObjects.Add(obj);
    }

    public void Unregister(OrientationDetectorObject obj)
    {
        if (orientationObjects.Contains(obj))
            orientationObjects.Remove(obj);
    }

    private void FindAllOrientationObjects()
    {
        var allObjects = FindObjectsByType<OrientationDetectorObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in allObjects)
            Register(obj);
    }
}