using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class NetworkBypassController : MonoBehaviour
    {
        public Transform NetworkFlows;
        
        [Header("Prefabs")]
        public GameObject NetworkFlowPrefab;

        [Header("Settings")]
        public Color ActiveColorSetting;
        public Color DeactiveColorSetting;

        private List<NetworkNode> activeNodeList;
        private List<NetworkFlow> activeFlowList;
        private StartNode startNode;
        private Dictionary<NetworkNode, bool> visitedNode = new Dictionary<NetworkNode, bool>();
        private Dictionary<NetworkFlow, bool> visitedFlow = new Dictionary<NetworkFlow, bool>();
        private List<NetworkFlow> visitedFlowList = new List<NetworkFlow>();
        private Queue<NetworkNode> queue = new Queue<NetworkNode>();
        private Queue<NetworkFlow> flowQueue = new Queue<NetworkFlow>();
        private bool isPuzzleComplete = false;
        private string debugGUIText;
        
        public static Color ActiveColor;
        public static Color DeactiveColor;
        private static Rect debugRect = new Rect(0, 0, 300, 400);
        
        void Awake()
        {
            InitSettings();
            CreateFlows();
            InitNodes();
            InitCamera();
        }
        
        void Start()
        {
            StartCoroutine(UpdateNetwork());   
//            TraverseNetwork();
        }

        void OnGUI()
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            // Debug文字
            labelStyle.alignment = TextAnchor.UpperLeft;
            labelStyle.fontSize = 12;
            GUI.Label(debugRect, debugGUIText);

            // 结束文字
            if (isPuzzleComplete)
            {
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 30;
                GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200), "Puzzle Complete", labelStyle);
            }
        }

        #region 初始化相关
        
        private void InitSettings()
        {
            ActiveColor = ActiveColorSetting;
            DeactiveColor = DeactiveColorSetting;
        }

        private void CreateFlows()
        {
            activeNodeList = new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());
            activeFlowList = new List<NetworkFlow>();
            
            foreach (var node in activeNodeList)
            {
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var neighbor = node.GetNeighbor(i);
                    if (neighbor != null)
                    {
                        CreateFlowBetween(node, neighbor, (NetworkNode.Direction) i);
                    }
                }
            }
        }

        private void CreateFlowBetween(NetworkNode first, NetworkNode second, NetworkNode.Direction direction)
        {
            NetworkFlow existFlow = activeFlowList.Find(x => x.FirstNode == second && x.SecondNode == first);
            // 两条有向边共享同一个flow GameObject
            GameObject flowObject = existFlow == null ? 
                CreateFlowGameObject(first, second, direction) :
                existFlow.FlowObject;
            NetworkFlow newFlow = new NetworkFlow(flowObject)
            {
                FirstNode = first,
                SecondNode = second,
                Direction = direction
            };
            first.SetFlow(direction, newFlow);
            activeFlowList.Add(newFlow);
        }

        private GameObject CreateFlowGameObject(NetworkNode first, NetworkNode second, NetworkNode.Direction direction)
        {
            Vector3 offset1 = NetworkNode.GetDrawLineOffset(first, direction);
            Vector3 offset2 = NetworkNode.GetDrawLineOffset(second, direction);
            
            GameObject flowGo = Instantiate(NetworkFlowPrefab, NetworkFlows);
            LineRenderer lineRenderer = flowGo.GetComponent<LineRenderer>();
            lineRenderer.SetPositions(new Vector3[]
            {
                first.transform.localPosition + offset1,
                second.transform.localPosition - offset2
            });
            return flowGo;
        }
        
        private void InitNodes()
        {
            foreach (var node in activeNodeList)
            {
                if (node is StartNode && startNode == null)
                {
                    startNode = node as StartNode;
                }
                RegisterEvents(node);
            }
        }
        
        private void RegisterEvents(NetworkNode node)
        {
            node.OnClick += OnNodeClick;
            node.OnFocus += OnNodeFocus;
            node.OnDataChanged += OnNodeDataChanged;
            if (node is EndNode)
            {
                (node as EndNode).OnPuzzleComplete += OnPuzzleComplete;
            }
        }

        private void InitCamera()
        {
            var comp = Camera.main.GetComponent<NetworkBypassCamera>();
            if (comp == null)
            {
                Camera.main.gameObject.AddComponent<NetworkBypassCamera>();
            }
        }
        
        #endregion
        
        #region 核心逻辑
        
        private IEnumerator UpdateNetwork()
        {
            if (startNode == null) yield break;
            yield return null;
//            TraverseNetwork();
            TraverseNetworkFlow();
        }

        private void TraverseNetwork()
        {
            foreach (var node in activeNodeList)
            {
                if (!(node is StartNode))
                {
                    node.Clear();
                }
            }
            foreach (var flow in activeFlowList)
            {
                flow.Clear();
            }
            
            visitedNode.Clear();
            visitedFlow.Clear();
            // 广度优先遍历
            VisitNode(startNode);
            while (queue.Count > 0)
            {
                NetworkNode node = queue.Dequeue();
                // 不允许从结束节点访问邻居节点
                if (node is EndNode)
                    continue;
                
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var direction = (NetworkNode.Direction) i;
                    var neighbor = node.GetNeighbor(direction);

                    if (neighbor == null)
                        continue;
                    var flow = node.GetFlow(direction);
                    // 特例：结束（带锁）节点是可以被重复访问的
                    if (visitedNode.ContainsKey(neighbor) &&
                        visitedFlow.ContainsKey(flow) && visitedFlow[flow] &&
                        !(neighbor is EndNode))
                        continue;

                    // 激活或反激活邻居节点的输入
                    if (node.IsReachableTo(direction))
                    {
                        flow.Activate(node, neighbor);
                        neighbor.SetInput(NetworkNode.GetOppositeDirection(direction), true);
                    }
                    else
                    {
                        flow.Deactivate(node, neighbor);
                        neighbor.SetInput(NetworkNode.GetOppositeDirection(direction), false);
                    }
                    VisitNode(neighbor);
                    VisitFlow(flow);
                }
            }
        }
        
        private void TraverseNetworkFlow()
        {
            // 清理工作：将所有节点和流都关闭
            foreach (var node in activeNodeList)
            {
                if (!(node is StartNode))
                {
                    node.Clear();
                }
            }
            foreach (var flow in activeFlowList)
            {
                flow.Clear();
            }
            visitedFlowList.Clear();
            
            // 从起点开始，查找周边的通路
            for (int i = 0; i < NetworkNode.NeighborNum; i++)
            {
                var flow = startNode.GetFlow(i);
                if (flow != null && startNode.IsReachableTo(flow.Direction))
                {
                    flowQueue.Enqueue(flow);
                    Log(string.Format("Enqueue: {0}", flow));
                }
            }
            
            // 开始遍历网络图
            while (flowQueue.Count > 0)
            {
                NetworkFlow flow = flowQueue.Dequeue();
                NetworkNode node = flow.FirstNode;
                NetworkNode neighbor = flow.SecondNode;
                
                visitedFlowList.Add(flow);
                Log(string.Format("Visit {0}", flow));

                // 激活流和输入状态
                NetworkNode.Direction direction = node.GetNeighborDirection(neighbor);
                flow.Activate(node, neighbor);
                neighbor.SetInput(NetworkNode.GetOppositeDirection(direction), true);

                bool nextFlowFound = false;
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var nextFlow = neighbor.GetFlow(i);
                    if (nextFlow == null ||
                        visitedFlowList.Contains(nextFlow) ||
                        flowQueue.Contains(nextFlow)||
                        // 第一趟查找，不包含那些已经走过一遍的通路
                        IsVisitedFlowExist(nextFlow.SecondNode, nextFlow.FirstNode))
                    {
                        continue;
                    }
                    if (neighbor.IsReachableTo(nextFlow.Direction))
                    {
                        Log(string.Format("Enqueue: {0}", nextFlow));
                        flowQueue.Enqueue(nextFlow);
                        nextFlowFound = true;
                    }
                }
                // 第二趟查找，可能包含反向的通路
                if (!nextFlowFound)
                {
                    for (int i = 0; i < NetworkNode.NeighborNum; i++)
                    {
                        var nextFlow = neighbor.GetFlow(i);
                        if (nextFlow == null ||
                            visitedFlowList.Contains(nextFlow) ||
                            flowQueue.Contains(nextFlow))
                        {
                            continue;
                        }
                        if (neighbor.IsReachableTo(nextFlow.Direction))
                        {
                            Log(string.Format("Enqueue2: {0}", nextFlow));
                            flowQueue.Enqueue(nextFlow);
                        }
                    }
                }
            }
        }

        private bool IsVisitedFlowExist(NetworkNode first, NetworkNode second)
        {
            return visitedFlowList.Find(
                x => x.FirstNode == first && x.SecondNode == second) != null;
        }

        private void VisitNode(NetworkNode node)
        {
            if (!visitedNode.ContainsKey(node))
                visitedNode.Add(node, true);
            queue.Enqueue(node);
        }

        private void VisitFlow(NetworkFlow flow)
        {
            if (!visitedFlow.ContainsKey(flow))
            {
                visitedFlow.Add(flow, false);
            }
            else
            {
                visitedFlow[flow] = true;
            }
        }

        private void OnNodeClick(NetworkNode node)
        {
            debugGUIText = "Click " + node.name;
            node.Execute();
        }
        
        private void OnNodeFocus(NetworkNode node)
        {
            debugGUIText = node != null ? 
                "Hover " + node.name : string.Empty;
        }

        private void OnNodeDataChanged(NetworkNode node)
        {
            isPuzzleComplete = false;
//            TraverseNetwork();
            TraverseNetworkFlow();
        }

        private void OnPuzzleComplete()
        {
            isPuzzleComplete = true;
        }
        
        #endregion

        private static void Log(string log)
        {
//            Debug.Log(log);
        }
    }
}