using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingState : State
{
    struct sPathCommand
    {
        public TileCell tileCell;
        public TileCell prevTileCell;
    }

    Queue<sPathCommand> _pathfindingQueue = new Queue<sPathCommand>();

    TileCell _targetTileCell;

    override public void Start ()
    {
        base.Start();

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
            command.prevTileCell = null;
            _pathfindingQueue.Enqueue(command);
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

        //큐의 데이터가 비어있을 때까지 검사
        if( 0 != _pathfindingQueue.Count)
        {
            //커맨드 하나를 꺼낸다
            sPathCommand command = _pathfindingQueue.Dequeue();

            //커맨드에 포함된 타일셀이 방문되지 않은 경우에만 검사
            if (false == command.tileCell.IsVisit())
            {
                //방문 표시
                command.tileCell.Visit();

                //목표에 도달했으면 종료
                if(command.tileCell.GetTileX() == _targetTileCell.GetTileX() &&
                    command.tileCell.GetTileY() == _targetTileCell.GetTileY())
                {
                    Debug.Log("finded");
                    _nextState = eStateType.IDLE;
                    return;
                }
                else
                {
                    //4방향 검사
                    for(int direction = (int)eMoveDirection.LEFT; direction <=(int)eMoveDirection.DOWN; direction++)
                    {
                        //각 방향별 타일셀을 도출
                        sPosition curPosition;
                        curPosition.x = command.tileCell.GetTileX();
                        curPosition.y = command.tileCell.GetTileY();
                        sPosition nextPosition = GetPositionByDirection(curPosition, direction);

                        TileCell nextTileCell = GameManager.Instance.GetMap().GetTileCell(nextPosition.x, nextPosition.y);

                        //지나갈 수 있고, 방문되지 않은 타일
                        if (true == nextTileCell.CanMove() && false == nextTileCell.IsVisit())
                        {
                            //거리값 계산
                            float distanceFromStart = command.tileCell.GetDistanceFromStart() + command.tileCell.GetDistanceWeight();

                            //새로운 커맨드를 만들어 큐에 삽입
                                //새로운 커맨드에 이전 타일을 세팅(현재 타일이 이전 타일)
                                    //큐에 삽입
                                    //방향에 따라 찾은 타일은 거리값을 갱신
                            nextTileCell.SetDistanceFromStart(distanceFromStart);

                            //검색범위를 그려준다.
                            nextTileCell.DrawColor();

                            sPathCommand newCommand;
                            newCommand.tileCell = nextTileCell;
                            newCommand.prevTileCell = command.tileCell;
                            _pathfindingQueue.Enqueue(newCommand);
                        }
                    }
                    
                }
            }
        }
    }

    //position
    struct sPosition
    {
        public int x;
        public int y;
    }

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