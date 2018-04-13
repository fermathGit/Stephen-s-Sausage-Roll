using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Client : MonoBehaviour
{
    [SerializeField]
    UITexture _texture;

    public LoadResourceMgr LoadResMgr;

    static Client _instance;

    FSM<Client> _fsm;

    NoDestory _noDestory;

    public static Client Instance {
        get {
            if (null == _instance)
            {
                _instance = GameObject.Find("Client").GetComponent<Client>();
            }
            return _instance;
        }
    }
    // Use this for initialization
    void Start()
    {
        _instance = this;
        _noDestory = gameObject.AddComponent<NoDestory>();
        _noDestory.AddNoDestroyObject(gameObject);

        _fsm = new FSM<Client>(this);
        Initialize();
    }

    private void Initialize()
    {
        //Instantiate(Resources.Load("Prefabs/Player"), transform);
        //Instantiate(Resources.Load("Prefabs/Ground"), transform);
        //Instantiate(Resources.Load("Prefabs/Sausage"), transform);

        RttMgr.Instance.Init();

        RttEntity.RttData data = new RttEntity.RttData();
        data.Width = 500;
        data.Height = 500;
        data.Res = "Prefabs/ruoshui";
        data.Pos = new Vector3(0.15f, -1.66f, 2.24f);
        data.Euler = new Vector3(0, 180, 0);
        data.Scale = Vector3.one;
        data.Anim = "running";
        data.PlayAnimation = null;
        RttMgr.Instance.GreateRTT(_texture, data);
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFsmState(IFSMState<Client> state)
    {
        _fsm.ChangeState(state);
    }

    public void AddNoDestroyObject(GameObject go)
    {
        _noDestory.AddNoDestroyObject(go);
    }
}
