using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class GamingState : Singleton<GamingState>, IFSMState<Client>
{
    Boat _myBoat;
    GridMgr _ground;
    Sausage _theSausage;

    public void Enter(Client stateEnt)
    {
        
    }

    public void Execute(Client stateEnt)
    {
        
    }

    public void Exit(Client stateEnt)
    {
        
    }
}
