using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingMoveState : State
{

    override public void Start()
    {
        base.Start();

        _character.PopPathfindingTileCell();
    }

    override public void Stop()
    {
        base.Stop();
        _character.ClearPathfindingTileCell();
    }

    public override void Update()
    {
        base.Update();

        if(false == _character.IsEmptyPathfindingTileCell())
        {
            TileCell tileCell = _character.PopPathfindingTileCell();
            _character.MoveStart(tileCell.GetTileX(), tileCell.GetTileY());
        }
        else
        {
            _nextState = eStateType.IDLE;
        }
    }
}
