using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MovebleObjectInput
{
    private LayerMask firstSelectLayer;
    private LayerMask selectLayer;
    private LayerMask moveLayer;
    private LayerMask wallLayer;

    private IInput input;
    public bool IsEnable { get; private set; }

    private SelectorPoint selectorPoint;
    private CinemachineCamera virtualCamera;
    private Camera cameraUI;
    private CinemachineOrbitalFollow orbitalFollow;

    private bool isMoveCameraCurrent;

    public MovebleObjectInput(IInput input, LayerMask selectLayer, LayerMask moveLayer, LayerMask firstSelectLayer, LayerMask wallLayer)
    {
        this.input = input;
        this.selectLayer = selectLayer;
        this.moveLayer = moveLayer;
        this.firstSelectLayer = firstSelectLayer;
        this.wallLayer = wallLayer;

        virtualCamera = UnityEngine.GameObject.Find("Virtual Camera").GetComponent<CinemachineCamera>();
        cameraUI = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();

        input.OnBegan += BeganWork;
        //input.OnEnded += EndedWork;
        input.OnMoved += MovedWork;
        input.OnScroll += ScrollWork;
    }

    public void SetEnable (bool enable)
    {
        IsEnable = enable;
    }

    private void BeganWork(Vector2 vector)
    {
        if (!IsEnable)
            return;

        switch (GameEntryGameplayCCh.instrumentManager.Instrument)
        {
            case InstrumentType.None:
                BeganNone();
                break;
            case InstrumentType.Create:
                BeganCreate();
                break;
            case InstrumentType.HideWall:
                BeganHideView();
                break;
            default:
                break;
        }

        isMoveCameraCurrent = input.GetOverPosition().y > Screen.height / 3f;
    }

    private void BeganNone()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(input.GetOverPosition()), out RaycastHit hitInfo, Mathf.Infinity, firstSelectLayer) ||
                    Physics.Raycast(Camera.main.ScreenPointToRay(input.GetOverPosition()), out hitInfo, Mathf.Infinity, selectLayer))
        {
            if (hitInfo.transform.TryGetComponent(out MovebleObject movebleObject))
            {
                MoveObjectSystem.Select(movebleObject);
                return;
            }

            if (hitInfo.transform.TryGetComponent(out SelectorPoint selectorPoint))
            {
                this.selectorPoint = selectorPoint;
                return;
            }
        }

        //Debug.Log($"IsPointOver {GameEntryGameplayCCh.input.IsPointOver()}");
        if (GameEntryGameplayCCh.input.IsPointOver() == false)
        {
            selectorPoint = null;
            MoveObjectSystem.Unselect();
        }
    }

    private void BeganCreate()
    {
        selectorPoint = null;
        MoveObjectSystem.Unselect();

        if (MoveObjectBank.CurrentCreateObject != null && GameEntryGameplayCCh.instrumentManager.Instrument == InstrumentType.Create)
        {
            if (MoveObjectBank.CurrentCreateObject.isWallObject &&
                Physics.SphereCast(Camera.main.ScreenPointToRay(input.GetOverPosition()), 0.5f, out RaycastHit hitInfo, Mathf.Infinity, wallLayer))
            {
                MoveObjectBank.CreateCurrentObject(hitInfo.point, hitInfo.normal, hitInfo.transform);
                return;
            } else if (MoveObjectBank.CurrentCreateObject.isFloorObject &&
                Physics.SphereCast(Camera.main.ScreenPointToRay(input.GetOverPosition()), 0.5f, out hitInfo, Mathf.Infinity, moveLayer))
            {
                MoveObjectBank.CreateCurrentObject(hitInfo.point, hitInfo.normal, hitInfo.transform);
                return;
            }
        }
    }

    private void MovedWork(Vector2 vector)
    {
        if (!IsEnable)
            return;

        if (MoveObjectSystem.CurrentObject)
        {
            RaycastHit[] raycastHitsMove = Physics.RaycastAll(Camera.main.ScreenPointToRay(input.GetOverPosition()), Mathf.Infinity, moveLayer);
            RaycastHit[] raycastHitsWall = Physics.RaycastAll(Camera.main.ScreenPointToRay(input.GetOverPosition()), Mathf.Infinity, wallLayer);

            List<RaycastHit> hitInfoList = new List<RaycastHit>();
            if (MoveObjectSystem.CurrentObject.isFloorObject)
                hitInfoList.AddRange(raycastHitsMove);
            if (MoveObjectSystem.CurrentObject.isWallObject)
                hitInfoList.AddRange(raycastHitsWall);

            if (hitInfoList.Count > 0)
            {
                RaycastHit hit = new RaycastHit();

                foreach (var hitInfo in hitInfoList)
                {
                    if (hitInfo.transform == MoveObjectSystem.CurrentObject.transform)
                        continue;

                    if (selectorPoint != null)
                    {
                        if (hit.transform == null || hit.distance > hitInfo.distance)
                            hit = hitInfo;
                    }
                }

                if (hit.transform != null)
                {
                    selectorPoint.MovePoint(hit.point + (Vector3.ProjectOnPlane(-Camera.main.transform.forward, Vector3.up).normalized * 0.2f));
                    MoveObjectSystem.CurrentObject.transform.parent = hit.transform;
                    return;
                }
            }
            else
            {
                if (selectorPoint != null && selectorPoint.UseOnlyOnPlane == false)
                {
                    Ray ray = Camera.main.ScreenPointToRay(GameEntryGameplayCCh.input.GetOverPosition());
                    if (selectorPoint.GetPlane().Raycast(ray, out float enter))
                    {
                        Vector3 point = ray.GetPoint(enter);
                        selectorPoint.MovePoint(point);
                    }
                    return;
                }
            }
        }

        CameraMove(vector, isMoveCameraCurrent);
    }

    private void CameraMove(Vector2 vector, bool isMove)
    {
        if (isMove)
        {
            virtualCamera.Target.TrackingTarget.position += virtualCamera.transform.TransformDirection(-vector) * 0.3f * Time.deltaTime;
        }
        else
        {
            orbitalFollow.HorizontalAxis.Value += vector.x * 3f * Time.deltaTime;
            orbitalFollow.VerticalAxis.Value += -vector.y * 3f * Time.deltaTime;
        }
    }

    private void ScrollWork(float scroll)
    {
        virtualCamera.Lens.FieldOfView -= scroll * Time.deltaTime * 10f;
        cameraUI.fieldOfView = virtualCamera.Lens.FieldOfView;
    }

    private void BeganHideView()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(input.GetOverPosition()), out RaycastHit hitInfo, Mathf.Infinity, firstSelectLayer))
        {
            if (hitInfo.transform.TryGetComponent(out Tap3DButton tap3DButton))
            {
                if (tap3DButton.ID == "HideWall") 
                    tap3DButton.Click();
            }

            /*if (hitInfo.transform.TryGetComponent(out HideViewObject hideView))
            {
                hideView.SwitchState();
            }*/
        }

    }
}
