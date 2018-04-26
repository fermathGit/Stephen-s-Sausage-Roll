using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBDownLoad;
using LBGame;

public class GameObj
{
    public GameObject Go;
    public ResPool Pool {
        get;
        private set;
    }

    public GameObj(GameObject go, ResPool pool)
    {
        Go = go;
        Pool = pool;
    }

    ~GameObj()
    {
        GameObject.Destroy(Go);
        Pool = null;
    }
}

public sealed class PoolMgr : Singleton<PoolMgr>, IMgr
{
    private Dictionary<string, ResPool> _poolDic;
    private event _DelegateVoid onUpdate;

    PoolMgr()
    {
    }

    public void Init()
    {
        _poolDic = new Dictionary<string, ResPool>(30);
    }

    public void Release()
    {
        foreach (ResPool pool in _poolDic.Values)
            pool.ReleasePool();

        _poolDic.Clear();
    }

    public bool ReleasePool(string path)
    {
        if (!_poolDic.ContainsKey(path))
            return false;

        ResPool pool = _poolDic[path];
        pool.ReleasePool();
        _poolDic.Remove(path);
        return true;
    }

    public GameObj GetGameObj(AssetsResource ar)
    {
        ResPool pool;
        if (!_poolDic.TryGetValue(ar.AssetPath, out pool))
            pool = CreatePool(ar);

        return pool.GetGameObj();
    }

    public bool RecycleGameObj(GameObj obj)
    {
        return obj.Pool.RecycleGameObj(obj);
    }

    public void RegistUpdate(ResPool pool)
    {
        onUpdate += pool.UnLoader.Update;
    }

    public void UnRegistUpdate(ResPool pool)
    {
        onUpdate -= pool.UnLoader.Update;
    }

    public void Update()
    {
        if (onUpdate != null)
            onUpdate();
    }

    ResPool CreatePool(AssetsResource ar)
    {
        string path = ar.AssetPath;
        if (!_poolDic.ContainsKey(path))
        {
            ResPool pool = new ResPool(path, ar);
            _poolDic.Add(path, pool);
            return pool;
        }
        return _poolDic[path];
    }
}
