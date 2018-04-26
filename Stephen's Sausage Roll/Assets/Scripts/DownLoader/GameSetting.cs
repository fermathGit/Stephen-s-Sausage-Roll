using UnityEngine;
using System.Collections;
using PublicComDef;

public class GameSetting : MonoBehaviour
{
    public struct AssetPath
    {
        public string Path;
        public bool NeedCache;
    }

    public struct LoginInfo
    {
        public string UserName;
        public string UserID;
        public string token;
    }

    private static GameSetting _instance;
    [SerializeField]
    private string _assetUrl;
    [SerializeField]
    private EPlatform _platform;
    [SerializeField]
    private string _assetCachePath;
    [SerializeField]
    private string _loginUrl;

    public AssetPath GameAssetPath;

    private string _serverIPAddress;
    private int _port;
    private LoginInfo _loginInfo;

    public static int Version { get; private set; }

    public static int Quality { get; private set; }

    public static string ServerIP { get { return _instance._serverIPAddress; } }

    public static int ServerPort { get { return _instance._port; } }

    public static string AssetUrl { get { return _instance._assetUrl; } }

    public static EPlatform Platform { get { return _instance._platform; } }

    public static string LoginUrl { get { return _instance._loginUrl; } }

    public static GameSetting Instance {
        get {
            if (null == _instance)
            {
                _instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameSetting>();
            }

            return _instance;
        }
    }

    public static void Initialize()
    {
        if (null == _instance)
        {
            _instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameSetting>();
        }

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
                break;

            case SystemLanguage.Turkish:
                break;

            case SystemLanguage.Korean:
                break;

            case SystemLanguage.Japanese:
                break;

            case SystemLanguage.English:
                break;
        }

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    _instance.GameAssetPath = new AssetPath();
                    _instance._assetCachePath = Application.persistentDataPath;
                    _instance.GameAssetPath.Path = "jar:file://" + Application.dataPath + "!/assets";
                    _instance.GameAssetPath.NeedCache = false;
                    _instance._platform = EPlatform.android;
                }
                break;

            case RuntimePlatform.IPhonePlayer:
                {
                    _instance.GameAssetPath = new AssetPath();
                    _instance.GameAssetPath.Path = "file://" + Application.dataPath + "/Raw";
                    _instance.GameAssetPath.NeedCache = false;
                    _instance._platform = EPlatform.iphone;
                }
                break;

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
                {

                    _instance.GameAssetPath = new AssetPath();
                    _instance.GameAssetPath.Path = "file:///" + Application.streamingAssetsPath;
                    _instance.GameAssetPath.NeedCache = false;

                    _instance._platform = EPlatform.editor;
                }
                break;
            case RuntimePlatform.WindowsPlayer:
                {
                    _instance.GameAssetPath = new AssetPath();
                    _instance.GameAssetPath.Path = "file://" + System.IO.Directory.GetCurrentDirectory() + "/";
                    _instance.GameAssetPath.Path = _instance.GameAssetPath.Path.Replace("\\", "/");
                    _instance.GameAssetPath.NeedCache = false;

                    _instance._platform = EPlatform.windows;
                }
                break;

            default:
                _instance._platform = EPlatform.unknown;
                break;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public void SetLoginInfo(LoginInfo loginInfo)
    {
        _loginInfo = loginInfo;
    }

    public void SetServer(string ipAddress, int port)
    {
        _serverIPAddress = ipAddress;
        _port = port;
    }
}
