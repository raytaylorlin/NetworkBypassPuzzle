using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace NetworkBypass
{ 
    public class TConnectionNode : ConnectionNode
    {
        protected override void ResetRotation()
        {
            base.ResetRotation();
            // T上
            if (RotateState == ERotateState.Up)
            {
                bool hasInput = HasInputFrom(Direction.Left) || HasInputFrom(Direction.Up) || HasInputFrom(Direction.Right);
                SetOutput(Direction.Up, hasInput);
                SetOutput(Direction.Right, hasInput);
                SetOutput(Direction.Down, false);
                SetOutput(Direction.Left, hasInput);
                SetActive(hasInput);
            }
            // T右
            else if (RotateState == ERotateState.Right)
            {
                bool hasInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Right) || HasInputFrom(Direction.Down);
                SetOutput(Direction.Up, hasInput);
                SetOutput(Direction.Right, hasInput);
                SetOutput(Direction.Down, hasInput);
                SetOutput(Direction.Left, false);
                SetActive(hasInput);
            }
            // T下
            else if (RotateState == ERotateState.Down)
            {
                bool hasInput = HasInputFrom(Direction.Right) || HasInputFrom(Direction.Down) || HasInputFrom(Direction.Left);
                SetOutput(Direction.Up, false);
                SetOutput(Direction.Right, hasInput);
                SetOutput(Direction.Down, hasInput);
                SetOutput(Direction.Left, hasInput);
                SetActive(hasInput);
            }
            // T左
            else if (RotateState == ERotateState.Left)
            {
                bool hasInput = HasInputFrom(Direction.Down) || HasInputFrom(Direction.Left) || HasInputFrom(Direction.Up);
                SetOutput(Direction.Up, hasInput);
                SetOutput(Direction.Right, false);
                SetOutput(Direction.Down, hasInput);
                SetOutput(Direction.Left, hasInput);
                SetActive(hasInput);
            }
        }
    }
}