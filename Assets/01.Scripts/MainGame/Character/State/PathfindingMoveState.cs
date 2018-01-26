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
            sPosition curPosition;
            curPosition.x = _character.GetTileX();
            curPosition.y = _character.GetTileY();

            sPosition nextPosition;
            nextPosition.x = tileCell.GetTileX();
            nextPosition.y = tileCell.GetTileY();

            eMoveDirection direction = GetDirection(curPosition, nextPosition);
            _character.SetNextDirection(direction);

            _character.MoveStart(tileCell.GetTileX(), tileCell.GetTileY());
        }
        else
        {
            _nextState = eStateType.IDLE;
        }
    }

    eMoveDirection GetDirection(sPosition curPosition, sPosition nextPosition)
    {
        if (nextPosition.x > curPosition.x)
            return eMoveDirection.RIGHT;
        else if (curPosition.x > nextPosition.x)
            return eMoveDirection.LEFT;
        else if (curPosition.y > nextPosition.y)
            return eMoveDirection.DOWN;
        else if (nextPosition.y > curPosition.y)
            return eMoveDirection.UP;

        return eMoveDirection.UP;
    }
}
