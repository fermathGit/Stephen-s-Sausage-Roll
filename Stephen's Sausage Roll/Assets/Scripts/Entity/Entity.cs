using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Entity : MonoBehaviour
{
    public float _x { get; private set; }
    public float _y { get; private set; }
    public float _rotationY { get; private set; }

    protected bool _isMove = false;
    protected float _moveDurotion = 0.2f;
    protected Ease _easeType = Ease.InCirc;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _easeType = Ease.InCirc;
        SetMyPos(1, 1);
        SetMyRotation(0);
    }

    // Update is called once per frame
    void Update()
    {
        DoUpdate();
    }

    void LateUpdate()
    {
        DoLateUpdate();
    }

    protected virtual void DoUpdate()
    {

    }

    protected virtual void DoLateUpdate() {

    }

    public virtual void SetMyPos(float x, float y, bool isSave = true)
    {
        if (isSave)
            ActionMgr.Instance.SaveInStack(this,_x, _y);
        transform.DOMove(new Vector3(10 * (x - (GridMgr.Side / 2) - 0.5f), 0, 10 * (y - (GridMgr.Side / 2) - 0.5f)), _moveDurotion).OnComplete(OnMoveComplete).SetEase(_easeType);
        _x = x;
        _y = y;
    }

    public virtual void Rotate90(bool isClockwise)
    {
        int e = isClockwise ? 90 : -90;
        SetMyRotation(_rotationY + e);
    }

    public virtual void SetMyRotation(float rotationY, bool isSave = true)
    {
        if (isSave)
            ActionMgr.Instance.SaveInStack(this,_rotationY);
        transform.DORotate(new Vector3(0, rotationY, 0), _moveDurotion).OnComplete(OnMoveComplete).SetEase(_easeType);
        _rotationY = rotationY;
    }

    public void OnMoveComplete()
    {
        _isMove = false;
    }

}

