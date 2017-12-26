using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace NetworkBypass
{ 
	public class RightAngleConnectionNode : ConnectionNode
	{
		protected override void ResetRotation()
		{
			base.ResetRotation();
			// 右上
			if (RotateState == ERotateState.Up)
			{
				bool hasTopRightInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Right);
				SetOutput(Direction.Up, hasTopRightInput);
				SetOutput(Direction.Right, hasTopRightInput);
				SetOutput(Direction.Down, false);
				SetOutput(Direction.Left, false);
			}
			// 右下
			else if (RotateState == ERotateState.Right)
			{
				bool hasBottomRightInput = HasInputFrom(Direction.Right) || HasInputFrom(Direction.Down);
				SetOutput(Direction.Up, false);
				SetOutput(Direction.Right, hasBottomRightInput);
				SetOutput(Direction.Down, hasBottomRightInput);
				SetOutput(Direction.Left, false);
			}
			// 左下
			else if (RotateState == ERotateState.Down)
			{
				bool hasBottomLeftInput = HasInputFrom(Direction.Down) || HasInputFrom(Direction.Left);
				SetOutput(Direction.Up, false);
				SetOutput(Direction.Right, false);
				SetOutput(Direction.Down, hasBottomLeftInput);
				SetOutput(Direction.Left, hasBottomLeftInput);
			}
			// 左上
			else if (RotateState == ERotateState.Left)
			{
				bool hasTopLeftInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Left);
				SetOutput(Direction.Up, hasTopLeftInput);
				SetOutput(Direction.Right, false);
				SetOutput(Direction.Down, false);
				SetOutput(Direction.Left, hasTopLeftInput);
			}
		}
	}
}