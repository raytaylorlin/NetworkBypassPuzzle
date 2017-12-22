using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public class NetworkNode : MonoBehaviour
	{
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
		
		public bool OutputUp;
		public bool OutputRight;
		public bool OutputDown;
		public bool OutputLeft;
	}
}