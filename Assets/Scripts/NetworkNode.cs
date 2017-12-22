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
		
		public NetworkNode North;
		public NetworkNode East;
		public NetworkNode South;
		public NetworkNode West;

		private NetworkNode lastNorth;
		private NetworkNode lastEast;
		private NetworkNode lastSouth;
		private NetworkNode lastWest;

		void OnValidate()
		{
//			if (South != null) South.North = this;
//			if (North != null) North.South = this;
//			if (East != null) East.West = this;
//			if (West != null) West.East = this;
		}

		private void ValidateNode(NetworkNode from, NetworkNode to)
		{
			
		}
	}
}