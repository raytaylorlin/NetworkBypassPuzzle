using System;
using UnityEngine;

namespace NetworkBypass
{
    /// <summary>
    /// 网络流通路逻辑类，本质上是一个有向边
    /// </summary>
    public class NetworkFlow
    {
        public NetworkNode FirstNode;
        public NetworkNode SecondNode;
        public NetworkNode.Direction Direction;
        public bool IsActive;
        public GameObject FlowObject;

        private bool fromFirstNodeActive = false;
        private bool fromSecondNodeActive = false;
        private LineRenderer lineRenderer;

        public NetworkFlow(GameObject flowObject)
        {
            FlowObject = flowObject;
            lineRenderer = flowObject.GetComponent<LineRenderer>();
        }
        
        public void Activate(NetworkNode from, NetworkNode to)
        {
            if (from == FirstNode && to == SecondNode)
            {
                fromFirstNodeActive = true;
            }
            else if (from == SecondNode && to == FirstNode)
            {
                fromSecondNodeActive = true;
            }

            if (fromFirstNodeActive || fromSecondNodeActive)
            {
                IsActive = true;
                lineRenderer.startColor = lineRenderer.endColor = NetworkBypassController.ActiveColor;
                lineRenderer.material.color = NetworkBypassController.ActiveColor;
            }
        }
    
        public void Deactivate(NetworkNode from, NetworkNode to)
        {
            if (from == FirstNode && to == SecondNode)
            {
                fromFirstNodeActive = false;
            }
            else if (from == SecondNode && to == FirstNode)
            {
                fromSecondNodeActive = false;
            }

            if (!fromFirstNodeActive && !fromSecondNodeActive)
            {
                IsActive = false;
                lineRenderer.startColor = lineRenderer.endColor = NetworkBypassController.DeactiveColor;
                lineRenderer.material.color = NetworkBypassController.DeactiveColor;
            }
        }
        
        public void Deactivate()
        {
            IsActive = false;
            lineRenderer.startColor = lineRenderer.endColor = NetworkBypassController.DeactiveColor;
            lineRenderer.material.color = NetworkBypassController.DeactiveColor;
        }

        public void Clear()
        {
            fromFirstNodeActive = false;
            fromSecondNodeActive = false;
            Deactivate();
        }

        public override string ToString()
        {
            return string.Format("({0} -> {1})", FirstNode.name, SecondNode.name);
        }
    }   
}
