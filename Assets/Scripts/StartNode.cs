using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class StartNode : NetworkNode
    {
        public bool[] OutputSetting = {false, false, false, false};

        protected override void OnInit()
        {
            for (int i = 0; i < NeighborNum; i++)
            {
                Outputs[i] = OutputSetting[i];
            }
        }
    }
}