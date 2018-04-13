using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RttEntity
{
    public delegate void _DelegateGameObject(GameObject obj);

    public class RttData
    {
        public string Res;
        public Vector3 Pos = new Vector3(0, 0, 4);
        public Vector3 Euler;
        public Vector3 Scale = Vector3.one;
        public string Anim;
        public _DelegateGameObject PlayAnimation;
        public int Width;
        public int Height;
    }

    Camera _cam;
    GameObject _goRtt;
    int _layer;
    int _dep;

    public bool IsFree { get; private set; }
    public RenderTexture MainTexture { get; private set; }

    public RttEntity(Transform root, int dep, int layer)
    {
        GameObject go = new GameObject("RTT[" + dep + "]");

        go.transform.parent = root;
        go.transform.localPosition = new Vector3(0, 0, dep * 1000);
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;

        _layer = layer;
        _dep = dep;
        _cam = go.AddComponent<Camera>();
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.cullingMask = 1 << layer;
        _cam.depth = -1;
        _cam.enabled = false;
        IsFree = true;
        _goRtt = null;

        MainTexture = new RenderTexture(300, 300, 24);
        _cam.targetTexture = MainTexture;
    }

    public void Create(GameObject go, RttData data)
    {
        IsFree = false;
        if (MainTexture.width != data.Width || MainTexture.height != data.Height)
        {
            ResetTexture(data.Width, data.Height, 24);
        }

        if (go == null)
            return;
        _goRtt = go;
        _goRtt.transform.parent = _cam.transform;
        _goRtt.transform.localPosition = data.Pos;
        _goRtt.transform.localEulerAngles = data.Euler;
        _goRtt.transform.localScale = data.Scale;


        if (string.IsNullOrEmpty(data.Anim))
        {
            if (data.PlayAnimation != null)
            {
                data.PlayAnimation(_goRtt);
            }
        }
        else
        {
            Animation anim = _goRtt.GetComponent<Animation>();
            if (anim != null)
            {
                anim.wrapMode = WrapMode.Loop;
                anim.Play(data.Anim);
            }
        }

        SetLayerAndChildrens(_goRtt, _layer);

        _cam.enabled = true;
    }

    public void EulerChange(float angle)
    {
        if (null != _goRtt)
        {
            _goRtt.transform.Rotate(0, angle, 0);
        }
    }

    public void Clear()
    {
        if (null != _goRtt)
        {
            GameObject.Destroy(_goRtt);
        }

        _goRtt = null;
        IsFree = true;
        _cam.enabled = false;
    }
    
    #region func
    void SetLayerAndChildrens(GameObject go, int layer)
    {
        if (null == go)
        {
            return;
        }

        Transform[] children = go.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; ++i)
        {
            children[i].gameObject.layer = layer;
        }

        go.layer = layer;
    }

    void ResetTexture(int width, int height, int depth)
    {
        _cam.targetTexture = null;
        GameObject.Destroy(MainTexture);
        MainTexture = new RenderTexture(width, height, depth);
        _cam.targetTexture = MainTexture;
    }

    #endregion
}


