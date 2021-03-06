﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingState : State
{
    public enum eUpdateState
    {
        PATHFINDING,
        BUILD_PATH,
    }

    protected struct sPathCommand
    {
        public TileCell tileCell;
        public float heuristic;
    }

    protected List<sPathCommand> _pathfindingQueue = new List<sPathCommand>();

    protected TileCell _targetTileCell;
    protected TileCell _reverseTileCell = null;

    protected eUpdateState _updateState;

    override public void Start ()
    {
        base.Start();
        _updateState = eUpdateState.PATHFINDING;
        _reverseTileCell = null;

        //시작타일셀을 큐에 넣는다.
        //길찾기 관련 변수 초기화
        //시작지점을 sPathCommand로 만들어서 큐에 삽입.
        _targetTileCell = _character.GetTargetTileCell();
        if(null != _targetTileCell)
        {
            GameManager.Instance.GetMap().ResetPathfinding();    //나중에 최적화: 체크가 된 부분만

            TileCell startTileCell = GameManager.Instance.GetMap().GetTileCell(_character.GetTileX(), _character.GetTileY());

            sPathCommand command;
            command.tileCell = startTileCell;
            command.heuristic = 0.0f;
            PushCommand(command);
        }
        else
        {
            _nextState = eStateType.IDLE;
        }
    }

    public override void Stop()
    {
        base.Stop();
        _pathfindingQueue.Clear();
        _character.ResetTargetTileCell();
    }

    override public void Update ()
    {
        base.Update();

        //좀 더 유연하게 하고 싶으면 상태로 빼면됨
        switch (_updateState)
        {
            case eUpdateState.PATHFINDING:
                UpdatePathfinding();
                break;
            case eUpdateState.BUILD_PATH:
                UpdateBuildPath();
                break;
        }
    }

    protected void UpdatePathfinding()
    {
        //큐의 데이터가 비어있을 때까지 검사
        if (0 != _pathfindingQueue.Count)
        {
            //커맨드 하나를 꺼낸다
            sPathCommand command = _pathfindingQueue[0];
            _pathfindingQueue.RemoveAt(0);

            //커맨드에 포함된 타일셀이 방문되지 않은 경우에만 검사
            if (false == command.tileCell.IsVisit())
            {
                //방문 표시
                command.tileCell.Visit();

                //목표에 도달했으면 종료
                if (command.tileCell.GetTileX() == _targetTileCell.GetTileX() &&
                    command.tileCell.GetTileY() == _targetTileCell.GetTileY())
                {
                    _reverseTileCell = command.tileCell;
                    _updateState = eUpdateState.BUILD_PATH;
                    return;
                }
                else
                {
                    //4방향 검사
                    for (int direction = (int)eMoveDirection.LEFT; direction <= (int)eMoveDirection.DOWN; direction++)
                    {
                        //각 방향별 타일셀을 도출
                        sPosition curPosition;
                        curPosition.x = command.tileCell.GetTileX();
                        curPosition.y = command.tileCell.GetTileY();
                        sPosition nextPosition = GetPositionByDirection(curPosition, direction);

                        TileCell nextTileCell = GameManager.Instance.GetMap().GetTileCell(nextPosition.x, nextPosition.y);

                        //지나갈 수 있고, 방문되지 않은 타일
                        if (true == nextTileCell.IsPathfindable() && false == nextTileCell.IsVisit() && null != nextTileCell)
                        {
                            float distanceFromStart = command.tileCell.GetDistanceFromStart() + command.tileCell.GetDistanceWeight();

                            if (null == nextTileCell.GetPrevTileCell())
                            {
                                nextTileCell.SetDistanceFromStart(distanceFromStart);
                                nextTileCell.SetPrevTileCell(command.tileCell);

                                //검색범위를 그려준다.
                                nextTileCell.DrawColor();

                                sPathCommand newCommand;
                                newCommand.tileCell = nextTileCell;
                                newCommand.heuristic = CalAstarHeuristic(distanceFromStart, nextTileCell, _targetTileCell); ;
                                PushCommand(newCommand);
                            }
                            else
                            {
                                if(distanceFromStart < nextTileCell.GetDistanceFromStart())
                                {
                                    nextTileCell.SetDistanceFromStart(distanceFromStart);
                                    nextTileCell.SetPrevTileCell(command.tileCell);

                                    sPathCommand newCommand;
                                    newCommand.tileCell = nextTileCell;
                                    newCommand.heuristic = CalAstarHeuristic(distanceFromStart, nextTileCell, _targetTileCell); ;
                                    PushCommand(newCommand);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    protected void UpdateBuildPath()
    {
        if(null != _reverseTileCell)
        {
            _reverseTileCell.ResetPathfindingMark();
            _character.PushPathfindingTileCell(_reverseTileCell);
            _reverseTileCell = _reverseTileCell.GetPrevTileCell();
        }
        else
        {
            _nextState = eStateType.MOVE;
        }
    }

    void PushCommand(sPathCommand command)
    {
        _pathfindingQueue.Add(command);

        //sorting
        _pathfindingQueue.Sort(delegate (sPathCommand c1, sPathCommand c2)
        {
            if (c1.heuristic < c2.heuristic)
                return -1;
            if (c1.heuristic > c2.heuristic)
                return 1;
            return 0;
        });
    }

    float CalSimpleHeuristic(TileCell tileCell, TileCell nextTileCell, TileCell targetTileCell)
    {
        float heuristic = 0.0f;

        int diffFromCurrent = 0;
        int diffFromNext = 0;

        //x축
        {
            //현재타일 ~ 목표타일까지 거리
            diffFromCurrent = tileCell.GetTileX() - targetTileCell.GetTileX();
            diffFromCurrent = Mathf.Abs(diffFromCurrent);
            //다음타일 ~ 목표타일까지 거리
            diffFromNext = nextTileCell.GetTileX() - targetTileCell.GetTileX();
            diffFromNext = Mathf.Abs(diffFromNext);

            if (diffFromCurrent < diffFromNext)
                heuristic += 1.0f;
            else if (diffFromNext < diffFromCurrent)
                heuristic -= 1.0f;
        }   

        //y축
        {
            //현재타일 ~ 목표타일까지 거리
            diffFromCurrent = tileCell.GetTileY() - targetTileCell.GetTileY();
            diffFromCurrent = Mathf.Abs(diffFromCurrent);
            //다음타일 ~ 목표타일까지 거리
            diffFromNext = nextTileCell.GetTileY() - targetTileCell.GetTileY();
            diffFromNext = Mathf.Abs(diffFromNext);

            if (diffFromCurrent < diffFromNext)
                heuristic += 1.0f;
            else if (diffFromNext < diffFromCurrent)
                heuristic -= 1.0f;
        }

        return heuristic;
    }

    float CalComplexHeuristic(TileCell nextTileCell, TileCell targetTileCell)
    {
        int distanceW = nextTileCell.GetTileX() - targetTileCell.GetTileX();
        int distanceH = nextTileCell.GetTileY() - targetTileCell.GetTileY();

        distanceW = distanceW * distanceW;
        distanceH = distanceH * distanceH;

        return (float)(distanceW + distanceH);
    }

    float CalAstarHeuristic(float distanceFromStart, TileCell nextTileCell, TileCell targetTileCell)
    {
        return distanceFromStart + CalComplexHeuristic(nextTileCell, targetTileCell);
    }


    //position

    sPosition GetPositionByDirection(sPosition position, int direction)
    {
        int moveX = position.x;
        int moveY = position.y;

        switch ((eMoveDirection)direction)
        {
            case eMoveDirection.LEFT:
                moveX--;
                break;
            case eMoveDirection.RIGHT:
                moveX++;
                break;
            case eMoveDirection.UP:
                moveY++;
                break;
            case eMoveDirection.DOWN:
                moveY--;
                break;
        }

        sPosition newPosition;
        newPosition.x = moveX;
        newPosition.y = moveY;

        return newPosition;
    }
}