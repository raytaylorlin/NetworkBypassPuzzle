using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public abstract class ConnectionNode : NetworkNode
    {
        private const float tweenDuration = 0.3f;
        
        public enum ERotateState
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        public SpriteRenderer BackgroundSprite;
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
            float to = -90 * (int) RotateState;
//            Sprite.transform.eulerAngles = new Vector3(0, 0, to);
            TweenRotation tweenRotation = TweenRotation.Begin(Sprite.gameObject, tweenDuration, Quaternion.Euler(0, 0, to));
            tweenRotation.quaternionLerp = true;
            tweenRotation.animationCurve = AnimationCurve.EaseInOut(0, 0, tweenDuration, 1f);
        }
        
        public override void SetInput(Direction from, bool isActive)
        {
            base.SetInput(from, isActive);
            ResetRotation();
        }

        protected virtual void ResetRotation()
        {
            
        }
        
        protected override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            SetSpriteActiveColor(BackgroundSprite, isActive);
        }
    }
}