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
		}
		
		public enum NodeType
		{
			Start,
			End
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

		void Start()
		{
			OnInit();			
		}

		protected virtual void OnInit()
		{
		}
	}
}