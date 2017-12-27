using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class EndNode : NetworkNode
    {
        public event Action OnPuzzleComplete;
        
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

        public override void SetInput(Direction from, bool isActive)
        {
            base.SetInput(from, isActive);
            int i = (int) from;
            if (InputLockSetting[i])
            {
                SetSpriteActiveColor(LockSprites[i], isActive);
            }
            CheckLocks();
        }

        private void CheckLocks()
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
            SetActive(allUnlocked);
        }

        public override void Execute()
        {
            base.Execute();
            if (allUnlocked)
            {
                OnPuzzleComplete();
            }
        }
    }
}