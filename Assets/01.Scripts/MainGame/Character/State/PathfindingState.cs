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

	override public void Start ()
    {
        base.Start();

        //시작타일셀을 큐에 넣는다.

        //길찾기 관련 변수 초기화

        //시작지점을 sPathCommand로 만들어서 큐에 삽입.
	}
	
	override public void Update ()
    {
        base.Start();

        //큐의 데이터가 비어있을 때까지 검사
        {
            //커맨드 하나를 꺼낸다
            //커맨드에 포함된 타일셀이 방문되지 않은 경우에만 검사
            {
                //방문 표시

                //목표에 도달했으면 종료

                //4방향 검사
                {
                    //각 방향별 타일셀을 도출
                    //지나갈 수 있고, 방문되지 않은 타일
                        //거리값 계산
                        //새로운 커맨드를 만들어 큐에 삽입
                            //새로운 커맨드에 이전 타일을 세팅(현재 타일이 이전 타일)
                            //큐에 삽입
                            //방향에 따라 찾은 타일은 거리값을 갱신
                }
            }
        }
    }
}
