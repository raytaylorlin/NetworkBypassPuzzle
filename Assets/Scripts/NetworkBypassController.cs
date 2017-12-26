using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkBypass
{
    public class NetworkBypassController : MonoBehaviour
    {
        public Transform NetworkFlows;
        public GameObject NetworkFlowPrefab;
        public List<NetworkNode> ActiveList;

        private StartNode startNode;
        private Dictionary<NetworkNode, bool> visited = new Dictionary<NetworkNode, bool>();
        private Queue<NetworkNode> queue = new Queue<NetworkNode>();
        private string debugGUIText;
        
        public static Color ActiveColor = new Color(0f, 191 / 255f, 1f);
        public static Color DeactiveColor = Color.white;
        private static Rect debugRect = new Rect(0, 0, 300, 400);

        void Awake()
        {
            CreateFlows();
            InitNodes();
        }
        
        void Start()
        {
            StartCoroutine(UpdateNetwork());   
//            TraverseNetwork();
        }

        private void RegisterEvents(NetworkNode node)
        {
            node.OnClick += OnNodeClick;
            node.OnFocus += (go, e) => { debugGUIText = go != null ? 
                "Hover " + go : string.Empty; };
            node.OnDataChanged += OnNodeDataChanged;
        }

        void OnGUI()
        {
            GUI.Label(debugRect, debugGUIText);
        }

        private void CreateFlows()
        {
            ActiveList = new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());
            foreach (var node in ActiveList)
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
//			first.OutputUp.networkFlow = flow;
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
            foreach (var node in ActiveList)
            {
                if (node is StartNode && startNode == null)
                {
                    startNode = node as StartNode;
                }
                RegisterEvents(node);
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

                    if (neighbor == null || visited.ContainsKey(neighbor))
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
            visited.Add(node, true);
            queue.Enqueue(node);
        }

        private void OnNodeClick(object obj, EventArgs e)
        {
            NetworkNode node = obj as NetworkNode;
            debugGUIText = "Click " + node.name;
            node.Execute();
        }

        private void OnNodeDataChanged(object obj, EventArgs e)
        {
            TraverseNetwork();
        }
    }
}