using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public class StartNode : NetworkNode
	{
		[HideInInspector] public bool OutputUpEnable;
		[HideInInspector] public bool OutputRightEnable;
		[HideInInspector] public bool OutputDownEnable;
		[HideInInspector] public bool OutputLeftEnable;

		protected override void OnInit()
		{
			if (OutputUpEnable) OutputUp = new IOFlow();
			if (OutputRightEnable) OutputRight = new IOFlow();
			if (OutputDownEnable) OutputDown = new IOFlow();
			if (OutputLeftEnable) OutputLeft = new IOFlow();
		}
	}
}