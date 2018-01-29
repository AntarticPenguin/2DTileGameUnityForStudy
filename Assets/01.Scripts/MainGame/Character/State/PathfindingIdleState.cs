using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingIdleState : State
{	
	override public void Update ()
    {
        base.Update();

        TileCell targetTileCell = _character.GetTargetTileCell();
        if (null != targetTileCell)
        {
            _nextState = eStateType.PATHFINDING;
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                MapObject mapObject = hit.collider.gameObject.GetComponent<MapObject>();
                if(null != mapObject)
                {
                    if(eMapObjectType.TILE_OBJECT == mapObject.GetObjectType())
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

                        TileCell tileCell = GameManager.Instance.GetMap().GetTileCell(mapObject.GetTileX(), mapObject.GetTileY());
                        if(true == tileCell.IsPathfindable())
                            _character.SetTargetTileCell(tileCell);
                    }
                }
            }
        }
	}
}
