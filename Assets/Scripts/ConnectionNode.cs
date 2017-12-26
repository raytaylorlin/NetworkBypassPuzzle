using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public abstract class ConnectionNode : NetworkNode
	{
		public enum ERotateState
		{
			Up = 0,
			Right = 1,
			Down = 2,
			Left = 3
		}

		public ERotateState RotateState = ERotateState.Up;

		protected override void Init()
		{
			base.Init();
			ResetRotation();
		}

		public override void Execute()
		{
			base.Execute();
			Rotate();
			NotifyDataChanged();
		}
		
		private void Rotate()
		{
			int index = ((int) RotateState + 1) % 4;
			RotateState = (ERotateState) index;
			ResetRotation();
		}

		protected virtual void ResetRotation()
		{
			Sprite.transform.eulerAngles = new Vector3(0, 0, (int) RotateState * 90);
		}
	}
}