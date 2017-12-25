using UnityEngine;

namespace NetworkBypass
{
    [DisallowMultipleComponent]
    public class NetworkNode : MonoBehaviour
    {
        public enum Direction
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
            Unknown = -1
        }

        [HideInInspector] public NetworkNode[] Neighbors = {null, null, null, null};
        [HideInInspector] public NetworkFlow[] Flows = {null, null, null, null};
        [HideInInspector] public bool[] Outputs = {false, false, false, false};
        [HideInInspector] public bool[] Inputs = {false, false, false, false};

        public const int NeighborNum = 4;
        
        private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
        private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

        void Start()
        {
            OnInit();			
        }

        protected virtual void OnInit()
        {
        }

        public virtual void OnInputActivate(Direction from)
        {
            Inputs[(int) from] = true;
        }

        public NetworkNode GetNeighbor(Direction direction)
        {
            return Neighbors[(int) direction];
        }
        
        public NetworkNode GetNeighbor(int index)
        {
            if (index >= 0 && index < NeighborNum) 
                return Neighbors[index];
            return null;
        }

        public void SetNeighbor(Direction direction, NetworkNode value)
        {
            Neighbors[(int) direction] = value;
        }
        
        public NetworkFlow GetFlow(Direction direction)
        {
            return Flows[(int) direction];
        }
        
        public NetworkFlow GetFlow(int index)
        {
            if (index >= 0 && index < NeighborNum) 
                return Flows[index];
            return null;
        }
        
        public void SetFlow(Direction direction, NetworkFlow value)
        {
            Flows[(int) direction] = value;
        }

        public bool IsReachableTo(Direction direction)
        {
            int i = (int) direction;
            return Outputs[i] && GetNeighbor(i) != null;
//			return OutputUp != null && OutputUp.IsActive && Up != null;
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            return (Direction) (((int) direction + 2) % 4);
        }
        
        public static Direction GetOppositeDirection(int index)
        {
            return (Direction) ((index + 2) % 4);
        }
        
        public static Vector3 GetDrawLineOffset(Direction direction)
        {
            if (direction == Direction.Up) return verticalOffset;
            if (direction == Direction.Right) return horizontalOffset;
            if (direction == Direction.Down) return -verticalOffset;
            if (direction == Direction.Left) return -horizontalOffset;
            return Vector3.zero;
        }
    }
}