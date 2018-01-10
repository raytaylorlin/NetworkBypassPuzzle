using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
	public class NetworkNodeLock : NetworkNodeComponent
	{
		public event Action<NetworkNode.Direction, bool> OnLockChanged;
		public event Action<bool> OnCheckAllLock;
		
		[HideInInspector] public bool[] InputLockSetting = {false, false, false, false};
		public SpriteRenderer[] LockSprites = new SpriteRenderer[NetworkNode.NeighborNum];
		
		private bool allUnlocked = false;

		public override void OnInit(NetworkNode node)
		{
			base.OnInit(node);
			for (int i = 0; i < NetworkNode.NeighborNum; i++)
			{
				LockSprites[i].gameObject.SetActive(InputLockSetting[i]);
			}
		}
		
		public override void OnInput(NetworkNode.Direction from, bool isActive)
		{
			base.OnInput(from, isActive);
			int i = (int) from;
			if (InputLockSetting[i])
			{
//				OnLockChanged(from, isActive);
				NetworkNode.SetSpriteActiveColor(LockSprites[i], isActive);
			}
			CheckLocks();
		}

		private void CheckLocks()
		{
			allUnlocked = true;
			for (int i = 0; i < NetworkNode.NeighborNum; i++)
			{
				if (InputLockSetting[i] && !node.Inputs[i])
				{
					allUnlocked = false;
					break;
				}
			}
			if (OnCheckAllLock != null)
			{
				OnCheckAllLock(allUnlocked);
			}
		}
	}
}