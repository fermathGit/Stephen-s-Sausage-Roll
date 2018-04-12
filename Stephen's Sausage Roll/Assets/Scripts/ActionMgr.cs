using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ActionType
{
    pos = 1,
    rot = 2
}
public struct ActionStruct
{
    public Entity entity;
    public ActionType type;
    public float x;
    public float y;
    public float rotationY;
    public bool isMain;
}

public sealed class ActionMgr : Singleton<ActionMgr>
{
    ActionMgr() { }
    
    Stack<ActionStruct> _actionStack;

    public void Init()
    {
        if (null == _actionStack)
            _actionStack = new Stack<ActionStruct>();
    }

    public void SaveInStack(Entity instance, float posX, float posY)
    {
        var temp = new ActionStruct();
        temp.entity = instance;
        temp.type = ActionType.pos;
        temp.x = posX;
        temp.y = posY;
        PushAction(temp);
    }

    public void SaveInStack(Entity instance, float _rotationY)
    {
        var temp = new ActionStruct();
        temp.entity = instance;
        temp.type = ActionType.rot;
        temp.rotationY = _rotationY;
        PushAction(temp);
    }

    void PushAction(ActionStruct temp)
    {
        temp.isMain = temp.entity.GetType() == Type.GetType("Boat");
        _actionStack.Push(temp);
    }

    public void ResetLastStep()
    {
        if (_actionStack.Count > 0)
        {
            var temp = _actionStack.Pop();
            if (temp.type == ActionType.pos)
            {
                temp.entity.SetMyPos(temp.x, temp.y, false);
            }
            else if (temp.type == ActionType.rot)
            {
                temp.entity.SetMyRotation(temp.rotationY, false);
            }
            if (!temp.isMain) {
                ResetLastStep();
            }
        }
    }
}

