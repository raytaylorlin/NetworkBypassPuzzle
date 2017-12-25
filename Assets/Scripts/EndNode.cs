using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class EndNode : NetworkNode
    {
        [HideInInspector] public bool[] InputLockSetting = {false, false, false, false};

        protected override void Init()
        {
            base.Init();
        }

        public override void ActivateInput(Direction from)
        {
            for (int i = 0; i < NeighborNum; i++)
            {
                if ((int) from == i && InputLockSetting[i])
                {
                    Debug.Log(string.Format("{0} unlocked", from));
                    Inputs[i] = true;
                }
            }
        }
    }
}