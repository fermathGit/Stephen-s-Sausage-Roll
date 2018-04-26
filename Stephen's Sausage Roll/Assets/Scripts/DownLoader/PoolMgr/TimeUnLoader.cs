using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBDownLoad;

namespace LBGame
{
    public class TimeUnLoader : PoolUnLoaderBase
    {
        private readonly int _poolLife;

        private float timeduration;

        public TimeUnLoader(AssetsResource ar, ResPool pool, int life) : base(ar, pool)
        {
            _pool = pool;
            _res = ar;
            _poolLife = life;
        }

        public override void Reset()
        {
            timeduration = 0;
        }

        public override void Update()
        {
            if (timeduration >= _poolLife)
            {
                PoolMgr.Instance.ReleasePool(_pool.PoolName);
                _res.Release();
            }
            if (_pool.UsedCount() == 0)
                timeduration += Time.deltaTime;
        }
    }
}