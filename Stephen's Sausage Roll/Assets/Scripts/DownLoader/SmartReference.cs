using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源卸载类型
/// </summary>
public enum AssetUnloadType
{
    Unuse = 1,      // 计数删除  计数为0时自动删除
    Overflow = 2,   // 暂时不使用 这里是溢出删除   这里只有做世界大地图才会使用   所以我还没有实现 用不上
    Never = 3,      // 从不删除
    Destroy = 4,    // 直接删除
    None = 0
}

public class SmartReference:BaseObject
{
    public AssetUnloadType UnloadType = AssetUnloadType.Unuse;
    private int _refCount = 0;

    public SmartReference() {

    }

    public int AddRef() {
        return ++_refCount;
    }

    public int GetRef() {
        return _refCount;
    }

    public virtual void Destroy() {

    }

    public int Release() {
        --_refCount;

        switch (UnloadType) {
            case AssetUnloadType.Unuse:
                if (_refCount <= 0) {
                    Destroy();
                }
                break;
            case AssetUnloadType.Destroy:
                Destroy();
                break;
        }
        return _refCount;
    }
}

