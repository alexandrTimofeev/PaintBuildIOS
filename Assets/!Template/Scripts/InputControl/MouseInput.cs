using UnityEngine;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// ПК-ввод через мышь.
/// </summary>
public class MouseInput : MonoBehaviour, IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnMoved;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnAnyPosition;
    public event Action<float> OnScroll;

    private Vector2 _lastPosition = Vector3.zero;
    private bool _isDragging;

    void Update()
    {
        // Нажатие левой кнопки
        if (Input.GetMouseButtonDown(0))
        {
            // проверяем, не над UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _isDragging = true;
                _lastPosition = Input.mousePosition;
                OnBegan?.Invoke(_lastPosition);
            }
        }

        // Движение мыши при удержании
        if (_isDragging && Input.GetMouseButton(0))
        {
            Vector2 current = Input.mousePosition;
            Vector2 delta = current - _lastPosition;
            if (delta != Vector2.zero)
            {
                OnMoved?.Invoke(delta);
                _lastPosition = current;
            }

            OnAnyPosition?.Invoke(current);
        }

        // Отпускание кнопки
        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            Vector2 endPos = Input.mousePosition;
            OnEnded?.Invoke(endPos);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            OnScroll?.Invoke(Input.GetAxis("Mouse ScrollWheel"));
    }

    public Vector2 GetOverPosition()
    {
        return Input.mousePosition;
    }

    public bool IsPointOver()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
