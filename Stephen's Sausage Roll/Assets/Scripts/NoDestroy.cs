using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoDestory : MonoBehaviour
{
    public List<GameObject> objList = new List<GameObject>();

    void Start()
    {
    }

    void Awake()
    {
        MonoBehaviour.DontDestroyOnLoad(gameObject);

        foreach (GameObject obj in objList)
        {
            MonoBehaviour.DontDestroyOnLoad(obj);
        }
    }

    public void AddNoDestroyObject(GameObject obj)
    {
        objList.Add(obj);
        MonoBehaviour.DontDestroyOnLoad(obj);
    }

    public void RemoveNoDestroyObject(GameObject obj)
    {
        objList.Remove(obj);
    }
}
