using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{ 
    //Unity Functions

    void Start()
    {
        _type = eMapObjectType.MONSTER;
    }

    override public void InitState()
    {
        base.InitState();
        {
            State state = new MonsterIdleState();
            state.Init(this);
            _stateMap[eStateType.IDLE] = state;
        }
        _state = _stateMap[eStateType.IDLE];
    }
}
