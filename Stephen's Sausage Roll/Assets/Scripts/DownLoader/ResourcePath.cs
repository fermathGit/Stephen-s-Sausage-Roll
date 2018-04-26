using UnityEngine;
using System.Collections;

// 资源类型
public enum EResourceType
{
    entity,
    player,
    monster,
    settings,
    effect,
    ui,
    model,
    audio,
    scene,
    config,
    max,
}

public static class ResourcePath
{
    // 获取资源路径
    public static string GetFullPath(EResourceType type)
    {
        string path = GetPath(type);
        return path;
    }

    // 获取资源路径
    public static string GetPath(EResourceType type)
    {
        switch (type)
        {
            case EResourceType.settings:
                return "Settings/";
            case EResourceType.scene:
                return GameSetting.Instance.GameAssetPath.Path + "/scene/";
            case EResourceType.player:
                return GameSetting.Instance.GameAssetPath.Path + "/model/";
            case EResourceType.monster:
                return GameSetting.Instance.GameAssetPath.Path + "/model/";
            case EResourceType.effect:
                return GameSetting.Instance.GameAssetPath.Path + "/effect/";
            case EResourceType.audio:
                return GameSetting.Instance.GameAssetPath.Path + "/audio/";
            case EResourceType.ui:
                return GameSetting.Instance.GameAssetPath.Path + "/ui/";
            case EResourceType.model:
                return GameSetting.Instance.GameAssetPath.Path + "/model/";
            case EResourceType.config:
                return GameSetting.Instance.GameAssetPath.Path + "/config/";
            default:
                return "file://" + Application.dataPath;
        }
    }

    public static string GetPath(EResourceType type, string path)
    {
        return GetPath(type) + path;
    }



}
