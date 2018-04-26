using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBDownLoad
{
    public class AssetsResource : SmartReference
    {
        public AssetBundleInfo MainAbi;
        public List<AssetBundleInfo> DependenceAbi = new List<AssetBundleInfo>();

        public Object OriginObj { get; private set; }
        public string AssetName { get; private set; }
        public string AssetPath { get; private set; }

        private bool _loadOk = false;
        public bool LoadOk {
            get { return _loadOk; }
        }

        private EResourceType _resType;
        public EResourceType ResType {
            get { return _resType; }
        }

        private List<_DelegateGameObj> _getObjList;
        private List<_DelegateObject> _getOriginObjList;

        public void SetNameAndPath(string name, string path, EResourceType type)
        {
            AssetName = name;
            AssetPath = path;
            _resType = type;
        }

        public void LoadObject()
        {
            _loadOk = true;
            OriginObj = MainAbi.Ab.LoadAsset(AssetName);

            if (_getObjList != null)
            {
                for (int i = 0, imax = _getObjList.Count; i < imax; ++i)
                    _getObjList[i](PoolMgr.Instance.GetGameObj(this));

                _getObjList.Clear();
                _getObjList = null;
            }

            if (_getOriginObjList != null)
            {
                for (int j = 0, jmax = _getOriginObjList.Count; j < jmax; ++j)
                    _getOriginObjList[j](OriginObj);

                _getOriginObjList.Clear();
                _getObjList = null;
            }

            SetAssetBundleInfos();
        }

        public override void Destroy()
        {
            if (DependenceAbi != null)
                DependenceAbi = null;

            if (OriginObj != null)
                Resources.UnloadAsset(OriginObj);

            GameAssetsMgr.Instance.RemoveAssetsResource(this);
            Dispose();
        }

        public void AddToWaitList(_DelegateGameObj onloadok)
        {
            if (_getObjList == null)
                _getObjList = new List<_DelegateGameObj>();

            _getObjList.Add(onloadok);
        }

        public void AddToWaitList(_DelegateObject onloadok)
        {
            if (_getOriginObjList == null)
                _getOriginObjList = new List<_DelegateObject>();

            _getOriginObjList.Add(onloadok);
        }

        void SetAssetBundleInfos()
        {
            if (_resType == EResourceType.ui)
            {
                for (int i = 0, imax = DependenceAbi.Count; i < imax; ++i)
                    DependenceAbi[i].UnloadType = AssetUnloadType.Never;
            }
            //ReleaseBundles ();
        }

        //		void ReleaseBundles()
        //		{
        //			MainAbi.Release ();
        //
        //			for (int i = 0, imax = DependenceAbi.Count; i < imax; ++i)
        //				DependenceAbi [i].Release ();
        //		}
    }
}
