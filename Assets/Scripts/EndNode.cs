using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class EndNode : NetworkNode
    {
        [HideInInspector] public bool[] InputLockSetting = {false, false, false, false};

        public SpriteRenderer[] LockSprites = new SpriteRenderer[NeighborNum];

        private bool allUnlocked = false;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < NeighborNum; i++)
            {
                LockSprites[i].gameObject.SetActive(InputLockSetting[i]);
            }
        }

        public override void ActivateInput(Direction from)
        {
            int i = (int) from;
            if (InputLockSetting[i])
            {
                Debug.Log(string.Format("{0} unlocked", from));
                Inputs[i] = true;
                LockSprites[i].color = NetworkBypassController.ActiveColor;
            }
            CheckLocks();
        }

        protected void CheckLocks()
        {
            allUnlocked = true;
            for (int i = 0; i < NeighborNum; i++)
            {
                if (InputLockSetting[i] && !Inputs[i])
                {
                    allUnlocked = false;
                    break;
                }
            }
            Sprite.color = allUnlocked ? NetworkBypassController.ActiveColor : NetworkBypassController.DeactiveColor;
            if (allUnlocked)
            {
                
            }
        }

        public override void Execute()
        {
            base.Execute();
            if (allUnlocked)
            {
                Debug.Log("Puzzle Complete!!!");
            }
        }
    }
}