using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTileLayer
{
    GROUND,
    MIDDLE,
    GAME_UI,
    MAXCOUNT,
}

public class TileCell
{
    Vector2 _position;
    List<List<MapObject>> _mapObjectMap = new List<List<MapObject>>();

    int _tileX;
    int _tileY;

    public void Init()
    {
        for(int i = 0; i < (int)eTileLayer.MAXCOUNT; i++)
        {
            List<MapObject> mapObjectlist = new List<MapObject>();
            _mapObjectMap.Add(mapObjectlist);
        }
    }

    public void SetPosition(float x, float y)
    {
        _position.x = x;
        _position.y = y;
    }

    public void SetTilePosition(int tileX, int tileY)
    {
        _tileX = tileX;
        _tileY = tileY;
    }

    public int GetTileX() { return _tileX; }
    public int GetTileY() { return _tileY; }

    public void AddObject(eTileLayer layer, MapObject mapObject)
    {
        List<MapObject> mapObjectList = _mapObjectMap[(int)layer];

        int sortingOrder = mapObjectList.Count;
        mapObject.SetSortingOrder(layer, sortingOrder);
        mapObject.SetPosition(_position);

        mapObjectList.Add(mapObject);
    }

    public void RemoveObject(MapObject mapObject)
    {
        List<MapObject> mapObjectList = _mapObjectMap[(int)mapObject.GetCurrentLayer()];
        mapObjectList.Remove(mapObject);
    }


    //Move

    public bool CanMove()
    {
        for (int layer = 0; layer < (int)eTileLayer.MAXCOUNT; layer++)
        {
            List<MapObject> mapObjectList = _mapObjectMap[layer];
            for(int i = 0; i < mapObjectList.Count; i++)
            {
                if (false == mapObjectList[i].CanMove())
                    return false;
            }
        }
        return true;
    }

    public List<MapObject> GetCollsionList()
    {
        List<MapObject> collisionList = new List<MapObject>();

        for (int layer = 0; layer < (int)eTileLayer.MAXCOUNT; layer++)
        {
            List<MapObject> mapObjectList = _mapObjectMap[layer];
            for (int i = 0; i < mapObjectList.Count; i++)
            {
                if (false == mapObjectList[i].CanMove())
                {
                    collisionList.Add(mapObjectList[i]);
                }
            }
        }
        return collisionList;
    }


    //Pathfinding

    bool _isVisit = false;
    float _distanceFromStart = 0.0f;
    float _distanceWeight = 1.0f;

    TileCell _prevTileCell = null;

    public void ResetPathfinding()
    {
        _isVisit = false;
        _distanceFromStart = 0.0f;
        _distanceWeight = 1.0f;
        _prevTileCell = null;
    }

    public bool IsVisit() { return _isVisit; }
    public void Visit() { _isVisit = true; }
    public float GetDistanceFromStart() { return _distanceFromStart; }
    public void SetDistanceFromStart(float distance) { _distanceFromStart = distance; }
    public float GetDistanceWeight() { return _distanceWeight; }

    public void SetPrevTileCell(TileCell tileCell) { _prevTileCell = tileCell; }

    public TileCell GetPrevTileCell() { return _prevTileCell; }


    public void DrawColor()
    {
        _mapObjectMap[(int)eTileLayer.GROUND][0].transform.GetComponent<SpriteRenderer>().color = Color.blue;
    }

    public void ResetPathfindingMark()
    {
        _mapObjectMap[(int)eTileLayer.GROUND][0].transform.GetComponent<SpriteRenderer>().color = Color.white;
    }

}
