using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectorMS : MonoBehaviour
{
    [SerializeField] private Transform lookTr;
    [SerializeField] private Transform rotateTr;

    [SerializeField] private SelectorPoint pointPos;
    [SerializeField] private SelectorPoint pointLook;
    [SerializeField] private SelectorPoint pointRotate;

    [Space]
    [SerializeField] private float rotationCoef = 1f;

    public Action<Vector3> OnMovePosition;
    public Action<Vector3> OnMoveRotation;
    public Action<float> OnMoveScale;

    private void Awake()
    {
        pointPos.OnMovePoint += MovePosition;
        pointLook.OnMovePoint += MoveLookPoint;
        pointRotate.OnMovePoint += MoveRotatePoint;
    }

    public void Select(MovebleObject movebleObject)
    {
        gameObject.SetActive(true);

        SetPositionView(movebleObject.GetPosition());
        SetRotationView(movebleObject.GetRotation());
        SetScaleView(movebleObject.GetScaleValue());
    }

    public void SetRotationView(Quaternion quaternion)
    {
        lookTr.rotation = quaternion;
        SetRotateView(quaternion.eulerAngles.z);
    }

    public void SetScaleView(float value)
    {
        SetScaleView(new Vector3(lookTr.localScale.x, lookTr.localScale.y, value));
    }
    public void SetScaleView(Vector3 vector3)
    {
        lookTr.localScale = vector3;
    }

    public void SetRotateView(float value)
    {
        SetRotateView(new Vector3(rotateTr.localScale.x, rotateTr.localScale.y, Mathf.Clamp(value / rotationCoef, 0.5f, 10f)));
    }
    public void SetRotateView(Vector3 vector3)
    {
        rotateTr.localScale = vector3;
    }

    public void SetPositionView(Vector3 position)
    {
        transform.position = position;
    }

    public void MovePosition (Vector3 position)
    {
        OnMovePosition?.Invoke(position);
        SoundManager.PlaySFXNotOverlap(GameEntryGameplayCCh.DataContainer.ChangeObPositionSFX, 0.15f);
    }

    public void MoveRotation(Vector3 rotation)
    {
        OnMoveRotation?.Invoke(rotation);
        SoundManager.PlaySFXNotOverlap(GameEntryGameplayCCh.DataContainer.ChangeObSizeSFX, 0.2f);
    }
    public void MoveScale(float scale)
    {
        OnMoveScale?.Invoke(scale);
    }

    public void MoveLookPoint(Vector3 position)
    {
        Vector3 vector = position - transform.position;

        Vector3 lookEuler = Quaternion.LookRotation(vector).eulerAngles;
        lookEuler = new Vector3(lookEuler.x, lookEuler.y, GetZRotation());
        float scale = vector.magnitude / 2f;

        MoveRotation(lookEuler);
        MoveScale(scale);
    }

    private float GetZRotation()
    {
        return ((pointRotate.transform.position - transform.position).magnitude / 2f * rotationCoef) - 5f;
    }

    private void MoveRotatePoint(Vector3 position)
    {
        float scale = ((position - transform.position).magnitude - 0.5f) / 2f;
        Vector3 vector = Quaternion.LookRotation(pointLook.transform.position - transform.position).eulerAngles;
        vector = new Vector3(vector.x, vector.y, scale * rotationCoef);

        MoveRotation(vector);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
