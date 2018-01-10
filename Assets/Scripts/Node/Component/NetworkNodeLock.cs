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
		public SpriteRenderer[] InputLockSprites = new SpriteRenderer[NetworkNode.NeighborNum];
		public SpriteRenderer LockBackgroundSprite;

		public bool IsUnlocked { get; set; }

		public override void OnInit(NetworkNode node)
		{
			base.OnInit(node);
			IsUnlocked = false;

			ShowLock();
			for (int i = 0; i < NetworkNode.NeighborNum; i++)
			{
				InputLockSprites[i].gameObject.SetActive(InputLockSetting[i]);
			}
		}
		
		public override void OnInput(NetworkNode.Direction from, bool isActive)
		{
			base.OnInput(from, isActive);
			int i = (int) from;
			if (InputLockSetting[i])
			{
//				OnLockChanged(from, isActive);
				NetworkNode.SetSpriteActiveColor(InputLockSprites[i], isActive);
			}
			CheckLocks();
		}

		private void CheckLocks()
		{
			IsUnlocked = true;
			for (int i = 0; i < NetworkNode.NeighborNum; i++)
			{
				if (InputLockSetting[i] && !node.Inputs[i])
				{
					IsUnlocked = false;
					break;
				}
			}
			NetworkNode.SetSpriteActiveColor(LockBackgroundSprite, IsUnlocked);
			if (OnCheckAllLock != null)
			{
				OnCheckAllLock(IsUnlocked);
			}
		}

		/// <summary>
		/// 供节点执行解锁用
		/// </summary>
		public void ExecuteUnlock()
		{
			HideLock();
			for (int i = 0; i < NetworkNode.NeighborNum; i++)
			{
				InputLockSprites[i].gameObject.SetActive(false);
			}
		}

		private void ShowLock()
		{
			if (LockBackgroundSprite != null)
			{
				node.MainSprite.gameObject.SetActive(false);
				LockBackgroundSprite.gameObject.SetActive(true);
			}
		}

		private void HideLock()
		{
			if (LockBackgroundSprite != null)
			{
				node.MainSprite.gameObject.SetActive(true);
				LockBackgroundSprite.gameObject.SetActive(false);
			}
		}
	}
}