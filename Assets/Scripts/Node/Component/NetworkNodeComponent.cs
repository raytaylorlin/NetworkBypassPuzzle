using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	[DisallowMultipleComponent]
	public abstract class NetworkNodeComponent : MonoBehaviour
	{
		protected NetworkNode node;

//		public NetworkNodeComponent(NetworkNode referNode)
//		{
//			node = referNode;
//		}

		public virtual void OnInit(NetworkNode _node)
		{
			node = _node != null ? _node : GetComponent<NetworkNode>();  
		}
		
		public virtual void OnInput(NetworkNode.Direction from, bool isActive) {}

	}
}