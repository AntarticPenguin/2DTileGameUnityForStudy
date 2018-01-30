﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{

    //Unity Functions
	
	void Start ()
    {
        
	}
	
	void Update ()
    {
		
	}


    //SpriteList

    Sprite[] _spriteArray;

    public void Init()
    {
        _spriteArray = Resources.LoadAll<Sprite>("Sprites/MapSprite");
        //CreateTiles();
        CreateRandomMaze();
    }


    //Tile

    public GameObject TileObjectPrefabs;

    int _width;
    int _height;

    TileCell[,] _tileCellList;

    void CreateTiles()
    {
        float tileSize = 32.0f;

        //1층
        {
            TextAsset scriptAsset = Resources.Load<TextAsset>("Data/MapData_layer1");
            string[] records = scriptAsset.text.Split('\n');

            {
                string[] token = records[0].Split(',');
                _width = int.Parse(token[1]);
                _height = int.Parse(token[2]);
            }
            _tileCellList = new TileCell[_height, _width];

            for (int y = 0; y < _height; y++)
            {
                int line = y + 2;
                string[] token = records[line].Split(',');
                for (int x = 0; x < _width; x++)
                {
                    int spriteIndex = int.Parse(token[x]);

                    GameObject tileGameObject = GameObject.Instantiate(TileObjectPrefabs);
                    tileGameObject.transform.SetParent(transform);
                    tileGameObject.transform.localScale = Vector3.one;
                    tileGameObject.transform.localPosition = Vector3.zero;

                    TileObject tileObject = tileGameObject.GetComponent<TileObject>();
                    tileObject.Init(_spriteArray[spriteIndex]);
                    tileObject.SetTilePosition(x, y);

                    _tileCellList[y, x] = new TileCell();
                    GetTileCell(x, y).Init();
                    GetTileCell(x, y).SetPosition(x * tileSize / 100.0f, y * tileSize / 100.0f);
                    GetTileCell(x, y).SetTilePosition(x, y);
                    GetTileCell(x, y).AddObject(eTileLayer.GROUND, tileObject);
                }
            }
        }

        //2층
        {
            TextAsset scriptAsset = Resources.Load<TextAsset>("Data/MapData_layer2");
            string[] records = scriptAsset.text.Split('\n');
            for (int y = 0; y < _height; y++)
            {
                int line = y + 2;
                string[] token = records[line].Split(',');
                for (int x = 0; x < _width; x++)
                {
                    int spriteIndex = int.Parse(token[x]);
                    if(0<= spriteIndex)
                    {
                        GameObject tileGameObject = GameObject.Instantiate(TileObjectPrefabs);
                        tileGameObject.transform.SetParent(transform);
                        tileGameObject.transform.localScale = Vector3.one;
                        tileGameObject.transform.localPosition = Vector3.zero;

                        TileObject tileObject = tileGameObject.GetComponent<TileObject>();
                        tileObject.Init(_spriteArray[spriteIndex]);
                        tileObject.SetCanMove(false);

                        GetTileCell(x, y).AddObject(eTileLayer.GROUND, tileObject);
                    }
                }
            }
        }
    }

    void CreateRandomMaze()
    {
        float tileSize = 32.0f;

        //1층
        {
            TextAsset scriptAsset = Resources.Load<TextAsset>("Data/MapData_layer1");
            string[] records = scriptAsset.text.Split('\n');

            {
                string[] token = records[0].Split(',');
                _width = int.Parse(token[1]);
                _height = int.Parse(token[2]);
            }
            _tileCellList = new TileCell[_height, _width];

            for (int y = 0; y < _height; y++)
            {
                int line = y + 2;
                string[] token = records[line].Split(',');
                for (int x = 0; x < _width; x++)
                {
                    int spriteIndex = int.Parse(token[x]);

                    GameObject tileGameObject = GameObject.Instantiate(TileObjectPrefabs);
                    tileGameObject.transform.SetParent(transform);
                    tileGameObject.transform.localScale = Vector3.one;
                    tileGameObject.transform.localPosition = Vector3.zero;

                    TileObject tileObject = tileGameObject.GetComponent<TileObject>();
                    tileObject.Init(_spriteArray[spriteIndex]);
                    tileObject.SetTilePosition(x, y);

                    _tileCellList[y, x] = new TileCell();
                    GetTileCell(x, y).Init();
                    GetTileCell(x, y).SetPosition(x * tileSize / 100.0f, y * tileSize / 100.0f);
                    GetTileCell(x, y).SetTilePosition(x, y);
                    GetTileCell(x, y).AddObject(eTileLayer.GROUND, tileObject);
                }
            }
        }

        //2층(랜덤)
        {
            //1. 일정간격으로 기둥을 박는다.
            int interval = 2;
            for (int y = 0; y < _height; y++)
            {
                if (0 == y % interval)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        if (0 == x % interval)
                        {
                            int spriteIndex = 252;

                            GameObject tileGameObject = GameObject.Instantiate(TileObjectPrefabs);
                            tileGameObject.transform.SetParent(transform);
                            tileGameObject.transform.localScale = Vector3.one;
                            tileGameObject.transform.localPosition = Vector3.zero;

                            TileObject tileObject = tileGameObject.GetComponent<TileObject>();
                            tileObject.Init(_spriteArray[spriteIndex]);
                            tileObject.SetCanMove(false);

                            GetTileCell(x, y).AddObject(eTileLayer.GROUND, tileObject);
                        }
                    }
                }
            }

            //2. 가지치기로 미로 생성
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if( false == GetTileCell(x,y).CanMove())    //기둥
                    {
                        //연결되지 않은 기둥일 경우
                        if(false == IsConnectedCell(x, y))
                        {
                            //랜덤한 한 방향으로 기둥이 연결될떄까지 이어준다
                            eMoveDirection direction = (eMoveDirection)Random.Range(1, (int)eMoveDirection.DOWN);

                            int searchTileX = x;
                            int searchTileY = y;

                            while(false == IsConnectedCell(searchTileX, searchTileY))
                            {
                                switch(direction)
                                {
                                    case eMoveDirection.LEFT:
                                        searchTileX--;
                                        break;
                                    case eMoveDirection.RIGHT:
                                        searchTileX++;
                                        break;
                                    case eMoveDirection.UP:
                                        searchTileY++;
                                        break;
                                    case eMoveDirection.DOWN:
                                        searchTileY--;
                                        break;
                                }

                                if(0 <= searchTileX && searchTileX < _width && 0 <= searchTileY && searchTileY < _height)
                                {
                                    //새로운 기둥을 심는다.
                                    int spriteIndex = 252;

                                    GameObject tileGameObject = GameObject.Instantiate(TileObjectPrefabs);
                                    tileGameObject.transform.SetParent(transform);
                                    tileGameObject.transform.localScale = Vector3.one;
                                    tileGameObject.transform.localPosition = Vector3.zero;

                                    TileObject tileObject = tileGameObject.GetComponent<TileObject>();
                                    tileObject.Init(_spriteArray[spriteIndex]);
                                    tileObject.SetCanMove(false);

                                    GetTileCell(searchTileX, searchTileY).AddObject(eTileLayer.GROUND, tileObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    bool IsConnectedCell(int tileX, int tileY)
    {
        //주변에 하나라도 붙은 기둥이 있으면 연결된 기둥
        for (int direction = (int)eMoveDirection.LEFT; direction <= (int)eMoveDirection.DOWN; direction++)
        {
            int searchTileX = tileX;
            int searchTileY = tileY;

            switch ((eMoveDirection)direction)
            {
                case eMoveDirection.LEFT:
                    searchTileX--;
                    break;
                case eMoveDirection.RIGHT:
                    searchTileX++;
                    break;
                case eMoveDirection.UP:
                    searchTileY++;
                    break;
                case eMoveDirection.DOWN:
                    searchTileY--;
                    break;
            }

            if (0 <= searchTileX && searchTileX < _width && 0 <= searchTileY && searchTileY < _height)
            {
                if (false == GetTileCell(searchTileX, searchTileY).IsPathfindable())
                    return true;
            }
            else
                return true;    //맵 밖
        }
        return false;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    
    //Tile

    public TileCell GetTileCell(int x, int y)
    {
        if (0 <= x && x < _width && 0 <= y && y < _height)
            return _tileCellList[y, x];
        return null;
    }

    public bool CanMoveTile(int tileX, int tileY)
    {
        if (tileX < 0 || _width <= tileX)
            return false;
        if (tileY < 0 || _height <= tileY)
            return false;

        TileCell tileCell = GetTileCell(tileX, tileY);
        return tileCell.CanMove();
    }

    public List<MapObject> GetCollisionList(int tileX, int tileY)
    {
        if (tileX < 0 || _width <= tileX)
            return null;
        if (tileY < 0 || _height <= tileY)
            return null;

        TileCell tileCell = GetTileCell(tileX, tileY);
        return tileCell.GetCollsionList();
    }

    public void ResetObject(int tileX, int tileY, MapObject mapObject)
    {
        TileCell tileCell = GetTileCell(tileX, tileY);
        tileCell.RemoveObject(mapObject);
    }

    public void SetObject(int tileX, int tileY, MapObject mapObject, eTileLayer layer)
    {
        TileCell tileCell = GetTileCell(tileX, tileY);
        tileCell.AddObject(layer, mapObject);
    }


    //Pathfinding

    public void ResetPathfinding()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GetTileCell(x, y).ResetPathfinding();
            }
        }
    }
}
