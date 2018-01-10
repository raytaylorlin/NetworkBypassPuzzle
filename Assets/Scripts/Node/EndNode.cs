using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class EndNode : NetworkNode
    {
        public event Action OnPuzzleComplete;

        private bool isAllUnlocked = false;
            
        protected override void Init()
        {
            base.Init();
//            lockDecorator.OnLockChanged += OnLockChanged;
            if (networkNodeLock != null)
            {
                networkNodeLock.OnCheckAllLock += OnCheckAllNetworkNodeLock;
            }
        }

        private void OnCheckAllNetworkNodeLock(bool allUnLocked)
        {
            isAllUnlocked = allUnLocked;
            SetActive(allUnLocked);
        }

        public override void Execute()
        {
            base.Execute();
            if (isAllUnlocked && OnPuzzleComplete != null)
            {
                OnPuzzleComplete();
            }
        }
    }
}