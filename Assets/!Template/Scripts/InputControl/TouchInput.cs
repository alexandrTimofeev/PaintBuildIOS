using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Мобильный ввод через сенсорный экран.
/// </summary>
public class TouchInput : MonoBehaviour, IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnMoved;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnAnyPosition;
    public event Action<float> OnScroll;

    private Vector3 lastPos = Vector3.zero;

    float lastPinchDistance;
    bool isPinching;

    private bool isPointOverLast;

    void Update()
    {
        isPointOverLast = IsPointOver();

        // === PINCH (2 пальца) ===
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // если любой палец над UI — игнорируем
            if (TestTouch(t0) || TestTouch(t1))
                return;

            float currentDistance = Vector2.Distance(t0.position, t1.position);

            if (!isPinching)
            {
                // начало pinch
                lastPinchDistance = currentDistance;
                isPinching = true;
                return;
            }

            float delta = currentDistance - lastPinchDistance;
            lastPinchDistance = currentDistance;

            if (Mathf.Abs(delta) > 0.01f)
                OnScroll?.Invoke(delta);

            return; // ВАЖНО: остальные эвенты не вызываются
        }

        // если пальцев стало меньше 2 — сбрасываем pinch
        isPinching = false;

        // === ОДИН ПАЛЕЦ ===
        foreach (Touch touch in Input.touches)
        {
            if (TestTouch(touch))
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnBegan?.Invoke(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (touch.deltaPosition != Vector2.zero)
                        OnMoved?.Invoke(touch.deltaPosition);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnEnded?.Invoke(touch.position);
                    break;
            }

            OnAnyPosition?.Invoke(touch.position);
        }
    }

    public Vector2 GetOverPosition() => Input.touchCount > 0 ? Input.GetTouch(0).position : lastPos;

    public bool IsPointOver()
    {
        if (Input.touches.Length == 0)
            return isPointOverLast;

        foreach (Touch touch in Input.touches)
        {
            if(TestTouch(touch))
                return true;
        }
        return false;
    }

    public bool TestTouch (Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
    }
}