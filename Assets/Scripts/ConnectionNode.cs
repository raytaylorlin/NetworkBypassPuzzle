using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public abstract class ConnectionNode : NetworkNode
    {
        public enum ERotateState
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        public ERotateState RotateState = ERotateState.Up;

        protected override void Init()
        {
            base.Init();
            ResetSprite();
        }

        public override void Execute()
        {
            base.Execute();
            Rotate();
            NotifyDataChanged();
        }
        
        private void Rotate()
        {
            int index = ((int) RotateState + 1) % 4;
            RotateState = (ERotateState) index;
            ResetSprite();
        }

        private void ResetSprite()
        {
            Sprite.transform.eulerAngles = new Vector3(0, 0, -90 * (int) RotateState);
        }
        
        public override void SetInput(Direction from, bool isActive)
        {
            base.SetInput(from, isActive);
            ResetRotation();
        }

        protected virtual void ResetRotation()
        {
            
        }
    }
}