using System;
using UnityEngine;

public class OrientationDetectorObject : MonoBehaviour
{
    public enum OrientationReaction
    {
        Ignore,
        ActivateOnTrue,
        DeactivateOnTrue
    }

    [Header("Checks")]
    public OrientationReaction portraitCheckReaction = OrientationReaction.ActivateOnTrue;
    public OrientationReaction landscapeCheckReaction = OrientationReaction.DeactivateOnTrue;

    [Header("Portrait Events")]
    public OrientationReaction anyPortraitReaction;
    public OrientationReaction portraitUpReaction;
    public OrientationReaction portraitUpsideDownReaction;

    [Header("Landscape Events")]
    public OrientationReaction anyLandscapeReaction;
    public OrientationReaction landscapeLeftReaction;
    public OrientationReaction landscapeRightReaction;

    public Action<bool> onPortraitCheck;
    public Action<bool> onLandscapeCheck;
    public Action onAnyPortrait;
    public Action onPortraitUp;
    public Action onPortraitUpsideDown;
    public Action onLandscape;
    public Action onLandscapeLeft;
    public Action onLandscapeRight;

    private static OrientationDetector detector;

    private void Awake()
    {
        if (detector == null)
            detector = FindFirstObjectByType<OrientationDetector>(FindObjectsInactive.Include);

        if (detector != null)
            detector.Register(this);
        else
            Debug.LogError("OrientationDetector not found!");
    }

    private void OnDestroy()
    {
        detector?.Unregister(this);
    }

    public void OnOrientationChanged(ScreenOrientation orientation)
    {
        // Определяем текущую ориентацию
        bool isPortrait = orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown;

        // Основные проверки
        ApplyReaction(portraitCheckReaction, isPortrait);
        onPortraitCheck?.Invoke(isPortrait);

        ApplyReaction(landscapeCheckReaction, !isPortrait);
        onLandscapeCheck?.Invoke(!isPortrait);

        // Специфичные события
        switch (orientation)
        {
            case ScreenOrientation.Portrait:
                ApplyReaction(portraitUpReaction, true);
                ApplyReaction(anyPortraitReaction, true);
                onPortraitUp?.Invoke();
                onAnyPortrait?.Invoke();
                break;
            case ScreenOrientation.PortraitUpsideDown:
                ApplyReaction(portraitUpsideDownReaction, true);
                ApplyReaction(anyPortraitReaction, true);
                onPortraitUpsideDown?.Invoke();
                onAnyPortrait?.Invoke();
                break;
            case ScreenOrientation.LandscapeLeft:
                ApplyReaction(landscapeLeftReaction, true);
                ApplyReaction(anyLandscapeReaction, true);
                onLandscapeLeft?.Invoke();
                onLandscape?.Invoke();
                break;
            case ScreenOrientation.LandscapeRight:
                ApplyReaction(landscapeRightReaction, true);
                ApplyReaction(anyLandscapeReaction, true);
                onLandscapeRight?.Invoke();
                onLandscape?.Invoke();
                break;
        }
    }

    private void ApplyReaction(OrientationReaction reaction, bool condition)
    {
        switch (reaction)
        {
            case OrientationReaction.Ignore: return;
            case OrientationReaction.ActivateOnTrue:
                if (condition) gameObject.SetActive(true);
                break;
            case OrientationReaction.DeactivateOnTrue:
                if (condition) gameObject.SetActive(false);
                break;
        }
    }
}