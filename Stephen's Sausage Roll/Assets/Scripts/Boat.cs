using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Boat : MonoBehaviour
{
    enum ActionType
    {
        pos = 1,
        rot = 2
    }
    struct ActionStruct
    {
        public ActionType type;
        public float x;
        public float y;
        public float rotationY;
    }

    public float _x { get; private set; }
    public float _y { get; private set; }
    public float _rotationY { get; private set; }

    bool _isMove = false;
    float _moveDurotion = 0.2f;
    Ease _easeType = Ease.InCirc;

    Stack<ActionStruct> _actionStack;

    // Use this for initialization
    void Start()
    {
        _actionStack = new Stack<ActionStruct>();
        SetMyPos(1, 1);
        SetMyRotation(0);
    }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        if (!_isMove && (h != 0 || v != 0))
        {
            _isMove = true;

            //一次只响应一个键位
            if (h != 0 && v != 0)
            {
                h = 0;
            }

            if (v == 0 && (int)_rotationY % 180 != 0)
            {
                Rotate90((int)_rotationY % 360 == 90 ^ h > 0);
            }
            else if (h == 0 && (int)_rotationY % 180 == 0)
            {
                Rotate90((int)_rotationY % 360 == 0 ^ v > 0);
            }
            else
            {
                SetMyPos(_x + h, _y + v);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PressOnZZZZZ();
        }
    }

    void PressOnZZZZZ()
    {
        if (_actionStack.Count > 0)
        {
            var temp = _actionStack.Pop();
            if (temp.type == ActionType.pos)
            {
                SetMyPos(temp.x, temp.y, false);
            }
            else if (temp.type == ActionType.rot)
            {
                SetMyRotation(temp.rotationY, false);
            }
        }
    }

    public void SetMyPos(float x, float y, bool isSave = true)
    {
        if (isSave)
            SaveInStack(_x, _y);
        transform.DOMove(new Vector3(10 * (x - (GridMgr.Side / 2) - 0.5f), 0, 10 * (y - (GridMgr.Side / 2) - 0.5f)), _moveDurotion).OnComplete(OnMoveComplete).SetEase(_easeType);
        _x = x;
        _y = y;
    }

    public void Rotate90(bool isClockwise)
    {
        int e = isClockwise ? 90 : -90;
        SetMyRotation(_rotationY + e);
    }

    void SetMyRotation(float rotationY,bool isSave=true)
    {
        if (isSave)
            SaveInStack(_rotationY);
        transform.DORotate(new Vector3(0, rotationY, 0), _moveDurotion).OnComplete(OnMoveComplete).SetEase(_easeType);
        _rotationY = rotationY;
    }

    public void OnMoveComplete()
    {
        _isMove = false;
    }

    void SaveInStack(float posX, float posY)
    {
        var temp = new ActionStruct();
        temp.type = ActionType.pos;
        temp.x = posX;
        temp.y = posY;
        _actionStack.Push(temp);
    }

    void SaveInStack(float _rotationY)
    {
        var temp = new ActionStruct();
        temp.type = ActionType.rot;
        temp.rotationY = _rotationY;
        _actionStack.Push(temp);
    }

}
