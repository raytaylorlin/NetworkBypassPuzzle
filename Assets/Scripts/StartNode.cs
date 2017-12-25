using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class StartNode : NetworkNode
    {
        [HideInInspector] public bool[] OutputSetting = {false, false, false, false};

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < NeighborNum; i++)
            {
                Outputs[i] = OutputSetting[i];
            }
        }
    }
}