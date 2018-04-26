using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBDownLoad;

namespace LBGame
{
    public class ResPool
    {
        private LinkedList<GameObj> _usedList = new LinkedList<GameObj>();
        private Queue<GameObj> _unUsedQueue = new Queue<GameObj>();

        private AssetsResource _res;

        private PoolUnLoaderBase _unLoader;
        public PoolUnLoaderBase UnLoader {
            get { return _unLoader; }
        }

        private string _poolName;
        public string PoolName {
            get { return _poolName; }
        }

        private int _totalCount = 1;
        private int _usedCount = 0;

        public ResPool(string name, AssetsResource ar)
        {
            _poolName = name;
            _res = ar;
            _res.AddRef();
            CreateUnLoader();
        }

        public bool ReleasePool()
        {
            PoolMgr.Instance.UnRegistUpdate(this);
            while (_usedList.Count > 0)
            {
                _usedList.RemoveLast();
            }
            while (_unUsedQueue.Count > 0)
            {
                _unUsedQueue.Dequeue();
            }

            _usedList = null;
            _unUsedQueue = null;
            return true;
        }

        public GameObj GetGameObj()
        {
            if (_unUsedQueue.Count == 0)
                return Swapn();

            GameObj item = _unUsedQueue.Dequeue();
            _usedList.AddLast(item);
            _unLoader.Reset();

            _usedCount++;

            return item;
        }

        public bool RecycleGameObj(GameObj item)
        {
            if (!item.Pool.Equals(this))
                return false;

            _usedList.Remove(item);
            _unUsedQueue.Enqueue(item);
            _usedCount--;
            item.Go.SetActive(false);
            _unLoader.Recycle();
            return true;
        }

        public int UsedCount()
        {
            return _usedCount;
        }

        GameObj Swapn()
        {
            Object obj = _res.OriginObj;
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            GameObj item = new GameObj(go, this);
            _usedList.AddLast(item);

            _usedCount++;
            _totalCount++;

            return item;
        }

        void CreateUnLoader()
        {
            switch (_res.ResType)
            {
                case EResourceType.effect:
                    _unLoader = new TimeUnLoader(_res, this, 120);
                    break;

                default:
                    _unLoader = new TimeUnLoader(_res, this, 60);
                    break;
            }
        }
    }
}