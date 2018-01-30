using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingImmediateState : PathfindingState
{
    override public void Start()
    {
        base.Start();

        while(0!= _pathfindingQueue.Count)
        {
            if (eUpdateState.BUILD_PATH == _updateState)
                break;

            UpdatePathfinding();
        }

        while(eStateType.MOVE != _nextState)
        {
            UpdateBuildPath();
        }
    }

    override public void Update()
    {

    }

    override public void Stop()
    {
        base.Stop();
    }

}
