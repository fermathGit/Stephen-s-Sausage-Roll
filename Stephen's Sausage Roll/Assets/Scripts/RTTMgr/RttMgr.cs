using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class RttMgr : Singleton<RttMgr>
{
    Transform _root;
    List<RttEntity> _rttList;

    RttMgr()
    {
        _rttList = new List<RttEntity>();

        GameObject root = new GameObject("RTT");

        Client.Instance.AddNoDestroyObject(root);

        _root = root.transform;
    }

    public void Init()
    {
    }

    RttEntity GetRTT()
    {
        RttEntity rtt = null;
        for (int i = 0, imax = _rttList.Count; i < imax; ++i)
        {
            if (_rttList[i].IsFree)
            {
                rtt = _rttList[i];
                break;
            }
        }
        if (null == rtt)
        {
            rtt = new RttEntity(_root, _rttList.Count + 1, LayerMask.NameToLayer("RTT"));
            _rttList.Add(rtt);
        }
        return rtt;
    }

    public void ReleaseAll()
    {
        for (int i = 0; i < _rttList.Count; ++i)
        {
            _rttList[i].Clear();
        }
    }
    
    public void GreateRTT(UITexture texture, RttEntity.RttData data)
    {
        string name = (data.Res.Split('/')[data.Res.Split('/').Length - 1]).Replace(".unity3d", "");
        string path = data.Res;
        LBDownLoad.GameAssetsMgr.Instance.GetGameObjAsync(name, path, EResourceType.audio, delegate (GameObj obj)
        {
            CallBack(obj, data, texture);
        });
    }
    
    private void CallBack(GameObj go, RttEntity.RttData data, UITexture texture)
    {
        RttEntity rtt = GetRTT();
        if (go != null) {
            rtt.Create(go.Go, data);
        }
        texture.mainTexture = rtt.MainTexture;
    }
}

