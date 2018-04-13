using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public sealed class LoadResourceMgr : MonoBehaviour
{
    
    public GameObject LoadResource(string resource) {
        var go = Instantiate( Resources.Load(resource) as GameObject);
        return go;
    }
}

