using System.Collections;
using System.Collections.Generic;
using NetworkBypass;
using UnityEngine;

namespace NetworkBypass
{
    public class NetworkFlow : MonoBehaviour
    {
        [ReadOnly] public NetworkNode FirstNode;
        [ReadOnly] public NetworkNode SecondNode;
        [ReadOnly] public bool IsActive;
        
        private LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        public void Activate()
        {
            IsActive = true;
            lineRenderer.startColor = lineRenderer.endColor = NetworkBypassController.ActiveColor;
            lineRenderer.material.color = NetworkBypassController.ActiveColor;
        }
    
        public void Deactivate()
        {
            IsActive = false;
            lineRenderer.startColor = lineRenderer.endColor = NetworkBypassController.DeactiveColor;
            lineRenderer.material.color = NetworkBypassController.DeactiveColor;
        }
    }   
}
