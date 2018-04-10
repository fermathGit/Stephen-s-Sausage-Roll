using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMgr : MonoBehaviour {
    [SerializeField]
    MeshGrid _clone;

    public static float Side = 10;
    Vector3 _farAwayPos= new Vector3(99999, 0, 0);
    // Use this for initialization
    void Start()
    {
        _clone.transform.localPosition = _farAwayPos;
        for (int i = 0; i < Side; ++i)
        {
            for (int j = 0; j < Side; ++j)
            {
                CreatGrid(i - (Side / 2), j - (Side / 2));
            }
        }
    }

    void CreatGrid(float x,float y)
    {
        if (null != _clone)
        {
            var go = Instantiate(_clone, transform);
            if (null != go) {
                go.transform.localPosition = new Vector3(x, 0, y);
                go.Init();
            }
        }
        else {
            Debug.Log("MeshGrid is null");
        }
    }
}
