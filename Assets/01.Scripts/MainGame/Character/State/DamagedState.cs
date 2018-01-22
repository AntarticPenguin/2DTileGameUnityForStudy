using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : State
{
	override public void Start ()
    {
        base.Start();

        int damagedPoint = _character.GetDamagedPoint();
        _character.DecreaseHP(damagedPoint);
        
        if(false == _character.IsLive())
        {
            _nextState = eStateType.DEAD;
        }
        else
        {
            _nextState = eStateType.IDLE;
        }
    }
}
