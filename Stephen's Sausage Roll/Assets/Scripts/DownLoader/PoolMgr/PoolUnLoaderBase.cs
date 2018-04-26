using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBDownLoad;

namespace LBGame
{
    public class PoolUnLoaderBase
    {
        protected ResPool _pool;
        protected AssetsResource _res;

        public PoolUnLoaderBase(AssetsResource ar, ResPool pool)
        {
            _pool = pool;
            _res = ar;
        }

        public virtual void Reset()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Recycle()
        {
        }
    }
}