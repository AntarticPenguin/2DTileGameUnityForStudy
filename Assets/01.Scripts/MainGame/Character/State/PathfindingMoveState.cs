using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingMoveState : State
{

    override public void Start()
    {
        base.Start();
        if (null != _character.GetTargetTileCell())
        {
            Debug.Log("TARGETTILECELL SETTING SUCCESS");
        }
    }
}
