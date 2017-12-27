using System;
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

        public event Action<NetworkNode> OnFocus;
        public event Action<NetworkNode> OnClick;
        public event Action<NetworkNode> OnDataChanged;

        [HideInInspector] public NetworkNode[] Neighbors = {null, null, null, null};
        [HideInInspector] public NetworkFlow[] Flows = {null, null, null, null};
        [HideInInspector] public bool[] Outputs = {false, false, false, false};
        [HideInInspector] public bool[] Inputs = {false, false, false, false};
        
        public SpriteRenderer Sprite;

        public const int NeighborNum = 4;
        
        private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
        private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

        #region Unity方法
        
        void Start()
        {
            CreateCollider();
            Init();	
        }
        
        private void CreateCollider()
        {
            // 某些节点不可交互，不需要碰撞体
            if (this is TransitionNode || this is StartNode)
                return;
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
        }
        
        void OnMouseDown()
        {
            OnClick(this);
        }

        void OnMouseEnter()
        {
            OnFocus(this);
        }

        void OnMouseExit()
        {
            OnFocus(null);
        }
        
        #endregion
        
        #region 虚方法

        protected virtual void Init()
        {

        }

        public virtual void ActivateInput(Direction from)
        {
            Inputs[(int) from] = true;
        }

        public virtual void Execute()
        {
            
        }

        public virtual void SetInput(Direction from, bool isActive)
        {
            int i = (int) from;
            Inputs[i] = isActive;
        }

        protected virtual void SetActive(bool isActive)
        {
            SetSpriteActiveColor(Sprite, isActive);
        }
        
        #endregion
        
        #region 工具方法

        protected void NotifyDataChanged()
        {
            OnDataChanged(this);
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
            Debug.Log(string.Format("{0}: {1}, {2}, {3}, {4}", gameObject.name, 
                Neighbors[0] != null ? Neighbors[0].ToString() : "null", 
                Neighbors[1] != null ? Neighbors[1].ToString() : "null", 
                Neighbors[2] != null ? Neighbors[2].ToString() : "null", 
                Neighbors[3] != null ? Neighbors[3].ToString() : "null"));
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
        }

        public bool HasInputFrom(Direction direction)
        {
            int i = (int) direction;
            return Inputs[i];
        }

        public bool HasInput
        {
            get
            {
                return HasInputFrom(Direction.Up) || HasInputFrom(Direction.Right)||
                       HasInputFrom(Direction.Down) || HasInputFrom(Direction.Left);
            }
        }

        public void SetOutput(Direction direction, bool flag)
        {
            int i = (int) direction;
            Outputs[i] = flag;
        }

        protected void SetSpriteActiveColor(SpriteRenderer sprite, bool isActive)
        {
            if (sprite != null)
                sprite.color = isActive ? NetworkBypassController.ActiveColor : NetworkBypassController.DeactiveColor;
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            return (Direction) (((int) direction + 2) % 4);
        }
        
        public static Direction GetOppositeDirection(int index)
        {
            return (Direction) ((index + 2) % 4);
        }
        
        public static Vector3 GetDrawLineOffset(NetworkNode node, Direction direction)
        {
            if (node is TransitionNode) return Vector3.zero;
            
            if (direction == Direction.Up) return verticalOffset;
            if (direction == Direction.Right) return horizontalOffset;
            if (direction == Direction.Down) return -verticalOffset;
            if (direction == Direction.Left) return -horizontalOffset;
            return Vector3.zero;
        }
        
        #endregion
    }
}