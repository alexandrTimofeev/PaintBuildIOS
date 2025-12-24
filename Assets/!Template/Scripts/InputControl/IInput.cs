using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnMoved;
    public event Action<float> OnScroll;
    public event Action<Vector2> OnAnyPosition;

    public Vector2 GetOverPosition();
    public bool IsPointOver();
}