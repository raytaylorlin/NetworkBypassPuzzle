using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace NetworkBypass
{ 
	public class CrossConnectionNode : ConnectionNode
	{
		protected override void ResetRotation()
		{
			base.ResetRotation();
			bool hasInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Right) || 
			                HasInputFrom(Direction.Down) || HasInputFrom(Direction.Left);
			SetOutput(Direction.Up, hasInput);
			SetOutput(Direction.Right, hasInput);
			SetOutput(Direction.Down, hasInput);
			SetOutput(Direction.Left, hasInput);
		}
	}
}