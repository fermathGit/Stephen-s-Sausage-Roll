using UnityEngine;

/// <summary>
/// Coroutine provider.
/// </summary>
public class CoroutineProvider : MonoBehaviour
{
    private static CoroutineProvider _instance = null;

    public static CoroutineProvider Instance {
        get {
            if (null == _instance)
            {
                GameObject go = new GameObject("CoroutineProvider");
                _instance = go.AddComponent<CoroutineProvider>();
                Client.Instance.AddNoDestroyObject(go);
            }
            return _instance;
        }
    }
}