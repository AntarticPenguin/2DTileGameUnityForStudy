using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
	override public void Start ()
    {
        base.Start();
        _character.SetCanMove(true);
        _character.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
    }

}
