using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public class EndNode : NetworkNode
	{
		[HideInInspector] public bool InputUpLock;
		[HideInInspector] public bool InputRightLock;
		[HideInInspector] public bool InputDownLock;
		[HideInInspector] public bool InputLeftLock;

		protected override void OnInit()
		{
//			if (OutputUpEnable) OutputUp = new IOFlow();
//			if (OutputRightEnable) OutputRight = new IOFlow();
//			if (OutputDownEnable) OutputDown = new IOFlow();
//			if (OutputLeftEnable) OutputLeft = new IOFlow();
//			InputUp = new IOFlow();
//			InputRight = new IOFlow();
//			InputDown = new IOFlow();
//			InputLeft = new IOFlow();
		}
	}
}