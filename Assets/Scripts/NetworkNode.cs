using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public class NetworkNode : MonoBehaviour
	{
		public class IOFlow
		{
			public bool IsActive = false;
			public NetworkFlow networkFlow;
		}

		public enum NodeType
		{
			Start,
			End
		}

		public enum Direction
		{
			Up = 0,
			Right = 1,
			Down = 2,
			Left = 3
		}

		public NodeType Type = NodeType.Start;

		[HideInInspector]
		public NetworkNode Up;
		[HideInInspector]
		public NetworkNode Right;
		[HideInInspector]
		public NetworkNode Down;
		[HideInInspector]
		public NetworkNode Left;

		[HideInInspector] public bool UpHasLine = false;
		[HideInInspector] public bool RightHasLine = false;
		[HideInInspector] public bool DownHasLine = false;
		[HideInInspector] public bool LeftHasLine = false;

		public IOFlow OutputUp;
		public IOFlow OutputRight;
		public IOFlow OutputDown;
		public IOFlow OutputLeft;

		public IOFlow InputUp;
		public IOFlow InputRight;
		public IOFlow InputDown;
		public IOFlow InputLeft;

		public bool IsOutputUpFlowActive
		{
			get { return OutputUp != null && OutputUp.IsActive && Up != null; }
		}

		public bool IsOutputRightFlowActive
		{
			get { return OutputRight != null && OutputRight.IsActive && Right != null; }
		}

		public bool IsOutputDownFlowActive
		{
			get { return OutputDown != null && OutputDown.IsActive && Down != null; }
		}

		public bool IsOutputLeftFlowActive
		{
			get { return OutputLeft != null && OutputLeft.IsActive && Left != null; }
		}

		void Start()
		{
			OnInit();			
		}

		protected virtual void OnInit()
		{
		}

		public virtual void OnInputActivate(Direction from)
		{

		}
	}
}