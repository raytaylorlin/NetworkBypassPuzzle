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

        private List<NetworkNode> activeList;
        private StartNode startNode;
        private Dictionary<NetworkNode, bool> visited = new Dictionary<NetworkNode, bool>();
        private Queue<NetworkNode> queue = new Queue<NetworkNode>();
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

        private void InitSettings()
        {
            ActiveColor = ActiveColorSetting;
            DeactiveColor = DeactiveColorSetting;
        }

        private void CreateFlows()
        {
            activeList = new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());
            foreach (var node in activeList)
            {
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var neighbor = node.GetNeighbor(i);
                    var direction = (NetworkNode.Direction) i;
                    if (neighbor != null &&
                        neighbor.GetFlow(direction) == null &&
                        neighbor.GetFlow(NetworkNode.GetOppositeDirection(direction)) == null)
                    {
                        CreateFlowBetween(node, neighbor, direction);
                    }
                }
            }
        }

        private void CreateFlowBetween(NetworkNode first, NetworkNode second, NetworkNode.Direction direction)
        {
            Vector3 offset = NetworkNode.GetDrawLineOffset(direction);
            NetworkFlow flow = CreateFlow(first.transform.localPosition + offset,
                second.transform.localPosition - offset);
            first.SetFlow(direction, flow);
            second.SetFlow(NetworkNode.GetOppositeDirection(direction), flow);

            flow.FirstNode = first;
            flow.SecondNode = second;
        }

        private NetworkFlow CreateFlow(Vector3 from, Vector3 to)
        {
            GameObject flowGo = Instantiate(NetworkFlowPrefab, NetworkFlows);
            LineRenderer lineRenderer = flowGo.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetPositions(new Vector3[] {from, to});
            return flowGo.GetComponent<NetworkFlow>();
        }
        
        private void InitNodes()
        {
            foreach (var node in activeList)
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
        
        private IEnumerator UpdateNetwork()
        {
            if (startNode == null) yield break;
            yield return null;
            TraverseNetwork();
        }

        private void TraverseNetwork()
        {
            visited.Clear();
            // 广度优先遍历
            VisitNode(startNode);
            while (queue.Count > 0)
            {
                NetworkNode node = queue.Dequeue();
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var direction = (NetworkNode.Direction) i;
                    var neighbor = node.GetNeighbor(direction);

                    if (neighbor == null)
                        continue;
                    // 特例：结束（带锁）节点是可以被重复访问的
                    if (visited.ContainsKey(neighbor) && !(neighbor is EndNode))
                        continue;
                    var flow = node.GetFlow(direction);
                    if (node.IsReachableTo(direction))
                    {
                        flow.Activate();
                        neighbor.SetInput(NetworkNode.GetOppositeDirection(direction), true);
                    }
                    else
                    {
                        flow.Deactivate();
                        neighbor.SetInput(NetworkNode.GetOppositeDirection(direction), false);
                    }
                    VisitNode(neighbor);
                }
            }
        }

        private void VisitNode(NetworkNode node)
        {
            if (!visited.ContainsKey(node))
                visited.Add(node, true);
            queue.Enqueue(node);
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
            TraverseNetwork();
        }

        private void OnPuzzleComplete()
        {
            isPuzzleComplete = true;
        }
    }
}