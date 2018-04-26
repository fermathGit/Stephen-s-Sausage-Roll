using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBDownLoad
{
    public class DownLoadResInfo
    {
        public AssetsResource Assets;
        public string MainAbPath;
        public Queue<string> DependenceAbPath;
        public _DelegateAssetsResource onLoadComplete;
    }

    public sealed class DownLoadMgr : Singleton<DownLoadMgr>, IMgr
    {
        private static bool _isMaxiSpeed = false;
        private Queue<DownLoadResInfo>[] _downLoadQueues;
        private const int _maxThreadCount = 4;

        private static bool _specialState;
        public static bool SpecialState {
            set { _specialState = value; }
            get { return _specialState; }
        }

        DownLoadMgr()
        {
        }

        public void Init()
        {
            _isMaxiSpeed = true;
            _downLoadQueues = new Queue<DownLoadResInfo>[_maxThreadCount];
            for (int i = 0; i < _maxThreadCount; ++i)
                _downLoadQueues[i] = new Queue<DownLoadResInfo>(10);

            for (int i = 0; i < _maxThreadCount; ++i)
                CoroutineProvider.Instance.StartCoroutine(WWWDownLoad(i));
        }

        public void Release()
        {
        }

        public static void ChangeLoadMode(bool maxSpeed)
        {
            if (maxSpeed == _isMaxiSpeed)
                return;

            Application.backgroundLoadingPriority = maxSpeed ? ThreadPriority.High : ThreadPriority.Low;
            _isMaxiSpeed = maxSpeed;
        }

        public void AddToDownLoadQueue(DownLoadResInfo info)
        {
            if (_isMaxiSpeed)
                FindMin().Enqueue(info);
            else
                _downLoadQueues[0].Enqueue(info);
        }

        public bool ClearDownLoadQueue()
        {
            if (_specialState)
                return false;

            for (int i = 0; i < _maxThreadCount; ++i)
                _downLoadQueues[i].Clear();

            return true;
        }

        public AssetsResource DirectDownLoad(DownLoadResInfo info)
        {
            AssetsResource ar = info.Assets;
            AssetBundleInfo mabi = null;
            AssetBundleInfo abi = null;
            AssetBundle ab = null;

            if ((mabi = GameAssetsMgr.Instance.CheckAssetBundle(info.MainAbPath)) != null)
            {
                ar.MainAbi = mabi;
            }
            else
            {
                ab = AssetBundle.LoadFromFile(info.MainAbPath);
                abi = new AssetBundleInfo(info.MainAbPath, ab);
                ar.MainAbi = abi;
                GameAssetsMgr.Instance.AddAssetBundle(abi);
            }

            while (info.DependenceAbPath.Count > 0)
            {
                string path = info.DependenceAbPath.Dequeue();
                if ((abi = GameAssetsMgr.Instance.CheckAssetBundle(path)) != null)
                {
                    ar.DependenceAbi.Add(abi);
                    continue;
                }
                ab = AssetBundle.LoadFromFile(info.MainAbPath);
                abi = new AssetBundleInfo(info.MainAbPath, ab);
                ar.DependenceAbi.Add(abi);
                GameAssetsMgr.Instance.AddAssetBundle(abi);
            }
            return ar;
        }

        IEnumerator WWWDownLoad(int index)
        {
            bool hasError = false;
            while (true)
            {

                //				if(!_isMaxiSpeed && index > 0)
                //				{
                //					yield return null;
                //					continue;
                //				}
                if (_downLoadQueues[index].Count == 0)
                {
                    yield return null;
                    continue;
                }

                hasError = false;
                DownLoadResInfo info = _downLoadQueues[index].Dequeue();
                AssetsResource ar = info.Assets;

                AssetBundleInfo mabi = null;
                if ((mabi = GameAssetsMgr.Instance.CheckAssetBundle(info.MainAbPath)) != null)
                {
                    ar.MainAbi = mabi;
                }
                else
                {
                    WWW www = new WWW(info.MainAbPath);
                    www.threadPriority = _isMaxiSpeed ? ThreadPriority.High : ThreadPriority.Low;
                    while (!www.isDone)
                        yield return null;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        hasError = true;
                        LogManager.LogError(info.MainAbPath);
                    }
                    else
                    {
                        ar.MainAbi = new AssetBundleInfo(www.url, www.assetBundle);
                        GameAssetsMgr.Instance.AddAssetBundle(ar.MainAbi);
                    }
                    www.Dispose();
                }

                while (info.DependenceAbPath.Count > 0)
                {
                    string path = info.DependenceAbPath.Dequeue();
                    AssetBundleInfo abi = null;
                    if ((abi = GameAssetsMgr.Instance.CheckAssetBundle(path)) != null)
                    {
                        ar.DependenceAbi.Add(abi);
                        continue;
                    }
                    WWW www = new WWW(path);
                    www.threadPriority = _isMaxiSpeed ? ThreadPriority.High : ThreadPriority.Low;
                    while (!www.isDone)
                        yield return null;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        LogManager.LogError(path);
                        www.Dispose();
                        hasError = true;
                        yield return null;
                        continue;
                    }

                    abi = new AssetBundleInfo(www.url, www.assetBundle);
                    ar.DependenceAbi.Add(abi);
                    GameAssetsMgr.Instance.AddAssetBundle(abi);
                    www.Dispose();
                    yield return null;
                }

                if (hasError)
                {
                    GameAssetsMgr.Instance.RemoveAssetsResource(ar);
                }
                if (info.onLoadComplete != null)
                    info.onLoadComplete(hasError ? null : ar);

                yield return null;
            }
        }

        Queue<DownLoadResInfo> FindMin()
        {
            int index = 0;
            int min = 0;
            for (int i = 0; i < _maxThreadCount; ++i)
            {
                if (min > _downLoadQueues[i].Count)
                {
                    index = i;
                    min = _downLoadQueues[i].Count;
                }
            }
            return _downLoadQueues[index];
        }
    }
}