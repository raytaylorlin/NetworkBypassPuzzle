using System.Collections;
using System.Collections.Generic;
using NetworkBypass;
using UnityEngine;

namespace NetworkBypass
{
    public class NetworkFlow : MonoBehaviour
    {
        public Color ActivateColor;
        public Color DeactivateColor;
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
            lineRenderer.startColor = lineRenderer.endColor = ActivateColor;
        }
    
        public void Deactivate()
        {
            IsActive = false;
            lineRenderer.startColor = lineRenderer.endColor = DeactivateColor;
        }
    }   
}
