using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace NetworkBypass
{ 
	public class StraightConnectionNode : ConnectionNode
	{
		protected override void ResetRotation()
		{
			base.ResetRotation();
			// 横向
			if (RotateState == ERotateState.Up || RotateState == ERotateState.Down)
			{
				bool hasHorizontalInput = HasInputFrom(Direction.Left) || HasInputFrom(Direction.Right);
				SetOutput(Direction.Left, hasHorizontalInput);
				SetOutput(Direction.Right, hasHorizontalInput);
				SetOutput(Direction.Up, false);
				SetOutput(Direction.Down, false);
			}
			// 纵向
			else
			{
				bool hasVerticalInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Down);
				SetOutput(Direction.Left, false);
				SetOutput(Direction.Right, false);
				SetOutput(Direction.Up, hasVerticalInput);
				SetOutput(Direction.Down, hasVerticalInput);
			}
		}
	}
}