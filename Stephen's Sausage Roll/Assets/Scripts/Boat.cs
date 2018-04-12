﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Boat : Entity
{
    protected override void Init()
    {
        ActionMgr.Instance.Init();

        base.Init();
        _easeType = Ease.InCirc;
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();

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
        ActionMgr.Instance.ResetLastStep();
    }

}
