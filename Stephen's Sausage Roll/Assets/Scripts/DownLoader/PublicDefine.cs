using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PublicComDef
{
    public enum EPlatform
    {
        windows,
        webPlayer,
        iphone,
        android,
        editor,
        unknown
    }


    public enum EModuleType
    {
        invalid = -1,
        login,
        gaming,
        selectRole,
        createRole,
    }


    public enum EGameState
    {
        invalid = -1,
        safe,
        fight,
    }
}