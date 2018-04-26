using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBGame;

namespace LBDownLoad
{
    public class AssetBundleInfo : SmartReference
    {
        public string AbName {
            get;
            private set;
        }

        public AssetBundle Ab {
            get;
            private set;
        }

        public AssetBundleInfo(string name, AssetBundle ab)
        {
            LogManager.LogError("load  " + name);
            AbName = name;
            Ab = ab;
            AddRef();
        }

        public override void Destroy()
        {
            LogManager.LogError("destroy   " + AbName);
            GameAssetsMgr.Instance.RemoveAssetBundle(this);
            Ab.Unload(false);
            Dispose();
        }

    }

    public sealed class GameAssetsMgr : Singleton<GameAssetsMgr>, IMgr
    {
        private Dictionary<string, AssetsResource> _originObjDic;
        private Dictionary<string, AssetBundleInfo> _allAssetBundleDic;

        private AssetBundleManifest _manifest;
        private const string _bundleEx = ".unity3d";

        GameAssetsMgr()
        {
        }

        public void Init()
        {
            _originObjDic = new Dictionary<string, AssetsResource>(50);
            _allAssetBundleDic = new Dictionary<string, AssetBundleInfo>(100);
            AssetBundle ab = null;// AssetBundle.LoadFromFile(Application.streamingAssetsPath + "");
            if (ab != null)
            {
                _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                ab.Unload(false);
            }
        }

        public void Release()
        {
        }

        public bool RemoveAssetBundle(AssetBundleInfo abi)
        {
            if (!_allAssetBundleDic.ContainsValue(abi))
                return false;

            _allAssetBundleDic.Remove(abi.AbName);
            return true;
        }

        public bool AddAssetBundle(AssetBundleInfo abi)
        {
            if (_allAssetBundleDic.ContainsValue(abi))
                return false;

            _allAssetBundleDic.Add(abi.AbName, abi);
            return true;
        }

        public AssetBundleInfo CheckAssetBundle(string path)
        {
            AssetBundleInfo abi = null;
            _allAssetBundleDic.TryGetValue(path, out abi);
            if (abi != null)
                abi.AddRef();

            return abi;
        }

        public bool RemoveAssetsResource(AssetsResource ar)
        {
            if (!_originObjDic.ContainsValue(ar))
                return false;

            _originObjDic.Remove(ar.AssetPath);
            return true;
        }

        public bool AddAssetsResource(AssetsResource ar)
        {
            if (_originObjDic.ContainsValue(ar))
                return false;

            _originObjDic.Add(ar.AssetPath, ar);
            return true;
        }

        public bool ReleaseObject(Object obj)
        {
            AssetsResource res = null;
            foreach (AssetsResource ar in _originObjDic.Values)
            {
                if (obj.Equals(ar.OriginObj))
                {
                    res = ar;
                    break;
                }
            }
            if (res != null)
                res.Release();

            return res != null;
        }

        #region 异步加载资源
        public void GetGameObjAsync(string name, string path, EResourceType type, _DelegateGameObj onLoadOk)
        {
            //string path = ResourcePath.GetPath (type) + name + _bundleEx;

            AssetsResource ar;
            if (_originObjDic.TryGetValue(path, out ar))
            {
                if (!ar.LoadOk)
                {
                    ar.AddToWaitList(onLoadOk);
                    return;
                }
                onLoadOk(PoolMgr.Instance.GetGameObj(ar));
            }
            else
            {
                DownLoadResource(name, path, onLoadOk, type);
            }
        }

        public void GetOriginObjAsync(string name, string path, EResourceType type, _DelegateObject onLoadOk)
        {
            //string path = ResourcePath.GetPath (type) + name + _bundleEx;

            AssetsResource ar;
            if (_originObjDic.TryGetValue(path, out ar))
            {
                if (!ar.LoadOk)
                {
                    ar.AddToWaitList(onLoadOk);
                    return;
                }
                ar.AddRef();
                onLoadOk(ar.OriginObj);
            }
            else
            {
                DownLoadOriginResource(name, path, onLoadOk, type);
            }
        }

        public void LoadSceneAsync(string name, string path, EResourceType type, _DelegateAssetsResource onLoadOk)
        {
            DownLoadScene(name, path, onLoadOk, type);
        }

        #endregion

        #region 同步加载资源
        public GameObj GetGameObjImmediately(string name, string path, EResourceType type)
        {
            //string path = ResourcePath.GetPath (type) + name + _bundleEx;
            AssetsResource ar;
            if (_originObjDic.TryGetValue(path, out ar))
            {
                return PoolMgr.Instance.GetGameObj(ar);
            }
            else
            {
                string[] deps = _manifest.GetAllDependencies(path);
                Queue<string> queue = new Queue<string>(deps.Length);
                for (int i = 0, imax = deps.Length; i < imax; ++i)
                    queue.Enqueue(deps[i]);

                DownLoadResInfo info = new DownLoadResInfo();
                info.Assets = new AssetsResource();
                info.Assets.SetNameAndPath(name, path, type);
                info.MainAbPath = path;
                info.DependenceAbPath = queue;
                ar = DownLoadMgr.Instance.DirectDownLoad(info);
                ar.LoadObject();
                AddAssetsResource(ar);
                return PoolMgr.Instance.GetGameObj(ar);
            }
        }

        public Object GetOriginObjImmediately(string name, string path, EResourceType type)
        {
            //string path = ResourcePath.GetPath (type) + name + _bundleEx;
            AssetsResource ar;
            if (_originObjDic.TryGetValue(path, out ar))
            {
                ar.AddRef();
                return ar.OriginObj;
            }
            else
            {
                string[] deps = _manifest.GetAllDependencies(path);
                Queue<string> queue = new Queue<string>(deps.Length);
                for (int i = 0, imax = deps.Length; i < imax; ++i)
                    queue.Enqueue(deps[i]);

                DownLoadResInfo info = new DownLoadResInfo();
                info.Assets = new AssetsResource();
                info.Assets.SetNameAndPath(name, path, type);
                info.MainAbPath = path;
                info.DependenceAbPath = queue;
                ar = DownLoadMgr.Instance.DirectDownLoad(info);
                ar.LoadObject();
                ar.AddRef();
                AddAssetsResource(ar);
                return ar.OriginObj;
            }
        }
        #endregion

        public bool RecycleGameObj(GameObj obj)
        {
            return PoolMgr.Instance.RecycleGameObj(obj);
        }

        void DownLoadResource(string name, string path, _DelegateGameObj onLoadOk, EResourceType type)
        {
            string[] deps = _manifest != null ? _manifest.GetAllDependencies(path) : new string[0];
            Queue<string> queue = new Queue<string>(deps.Length);
            for (int i = 0, imax = deps.Length; i < imax; ++i)
                queue.Enqueue(deps[i]);

            DownLoadResInfo info = new DownLoadResInfo();
            info.Assets = new AssetsResource();
            info.Assets.SetNameAndPath(name, path, type);
            info.MainAbPath = path;
            info.DependenceAbPath = queue;
            AddAssetsResource(info.Assets);
            info.onLoadComplete = delegate (AssetsResource ar)
            {
                if (ar != null)
                {
                    ar.LoadObject();
                    onLoadOk(PoolMgr.Instance.GetGameObj(ar));
                }
                else
                    onLoadOk(null);
            };

            DownLoadMgr.Instance.AddToDownLoadQueue(info);
        }

        void DownLoadOriginResource(string name, string path, _DelegateObject onLoadOk, EResourceType type)
        {
            string[] deps = _manifest.GetAllDependencies(path);
            Queue<string> queue = new Queue<string>(deps.Length);
            for (int i = 0, imax = deps.Length; i < imax; ++i)
                queue.Enqueue(deps[i]);

            DownLoadResInfo info = new DownLoadResInfo();
            info.Assets = new AssetsResource();
            info.Assets.SetNameAndPath(name, path, type);
            info.MainAbPath = path;
            info.DependenceAbPath = queue;
            AddAssetsResource(info.Assets);
            info.onLoadComplete = delegate (AssetsResource ar)
            {
                if (ar != null)
                {
                    ar.LoadObject();
                    ar.AddRef();
                    onLoadOk(ar.OriginObj);
                }
                else
                    onLoadOk(null);
            };

            DownLoadMgr.Instance.AddToDownLoadQueue(info);
        }

        void DownLoadScene(string name, string path, _DelegateAssetsResource onLoadOK, EResourceType type)
        {
            string[] deps = _manifest.GetAllDependencies(path);
            Queue<string> queue = new Queue<string>(deps.Length);
            for (int i = 0, imax = deps.Length; i < imax; ++i)
                queue.Enqueue(deps[i]);

            DownLoadResInfo info = new DownLoadResInfo();
            info.Assets = new AssetsResource();
            info.Assets.SetNameAndPath(name, path, type);
            info.MainAbPath = path;
            info.DependenceAbPath = queue;
            info.onLoadComplete = delegate (AssetsResource ar)
            {
                if (ar != null)
                {
                    ar.AddRef();
                    onLoadOK(ar);
                }
                else
                    onLoadOK(null);
            };

            DownLoadMgr.Instance.AddToDownLoadQueue(info);
        }
    }
}