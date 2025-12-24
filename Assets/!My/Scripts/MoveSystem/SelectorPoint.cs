using System;
using System.Collections;
using UnityEngine;

public class SelectorPoint : MonoBehaviour
{
    public Action<Vector3> OnMovePoint;

    [SerializeField] private bool onlyY;
    public bool UseOnlyOnPlane = true;
    [SerializeField] private Transform rootToDirection;

    public void MovePoint (Vector3 point)
    {
       //if (onlyY)
       //     point = new Vector3(transform.position.x, point.y, transform.position.z);

        OnMovePoint?.Invoke(point);
    }

    public Vector3 GetDirection()
    {
        if(rootToDirection!= null)
            return (rootToDirection.position - transform.position).normalized;
        else
            return Vector3.zero;
    }

    public Plane GetPlane()
    {
        if (rootToDirection)
            return new Plane(Vector3.up, rootToDirection.position);
        return new Plane(Vector3.up, transform.position);
    }
}