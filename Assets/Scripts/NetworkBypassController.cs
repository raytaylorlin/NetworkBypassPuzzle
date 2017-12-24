using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		
		private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
		private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

		void Start()
		{
			ActiveList = new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());
			CreateLines();
			foreach (var node in ActiveList)
			{
				if (node is StartNode && startNode == null)
				{
					startNode = node as StartNode;
				}
				Debug.Log(node);
			}
			StartCoroutine(UpdateNetwork());
		}

		void Update()
		{

		}

		private IEnumerator UpdateNetwork()
		{
			if (startNode == null) yield break;

			while (true)
			{
				visited.Clear();
				VisitNode(startNode);
				while (queue.Count > 0)
				{
					NetworkNode node = queue.Dequeue();
					if (node.IsOutputUpFlowActive && !visited.ContainsKey(node.Up))
					{
						node.OutputUp.networkFlow.Activate();
						node.Up.OnInputActivate(NetworkNode.Direction.Down);
						VisitNode(node.Up);
					}
					if (node.IsOutputRightFlowActive && !visited.ContainsKey(node.Right))
					{
						node.OutputRight.networkFlow.Activate();
						node.Right.OnInputActivate(NetworkNode.Direction.Left);
						VisitNode(node.Right);
					}
					if (node.IsOutputDownFlowActive && !visited.ContainsKey(node.Down))
					{
						node.OutputDown.networkFlow.Activate();
						node.Down.OnInputActivate(NetworkNode.Direction.Up);
						VisitNode(node.Down);
					}
					if (node.IsOutputLeftFlowActive && !visited.ContainsKey(node.Left))
					{
						node.OutputLeft.networkFlow.Activate();
						node.Left.OnInputActivate(NetworkNode.Direction.Right);
						VisitNode(node.Left);
					}
				}

				yield return new WaitForSeconds(1f);
			}
		}

		private void VisitNode(NetworkNode node)
		{
			visited.Add(node, true);
			queue.Enqueue(node);
		}

		private void CreateLines()
		{
			foreach (var node in ActiveList)
			{
				if (node.Up != null && !node.UpHasLine && !node.Up.DownHasLine)
				{
					CreateLine(node, node.Up, "Up");
//					CreateLine(node.transform.localPosition + verticalOffset,
//						node.Up.transform.localPosition - verticalOffset);
//					node.UpHasLine = node.Up.DownHasLine = true;
				}
				if (node.Right != null && !node.RightHasLine && !node.Right.LeftHasLine)
				{
					CreateLine(node, node.Right, "Right");
//					CreateLine(node.transform.localPosition + horizontalOffset,
//						node.Right.transform.localPosition - horizontalOffset);
//					node.RightHasLine = node.Right.LeftHasLine = true;
				}
				if (node.Down != null && !node.DownHasLine && !node.Down.UpHasLine)
				{
					CreateLine(node, node.Down, "Down");
//					CreateLine(node.transform.localPosition - verticalOffset,
//						node.Down.transform.localPosition + verticalOffset);
//					node.DownHasLine = node.Down.UpHasLine = true;
				}
				if (node.Left != null && !node.LeftHasLine && !node.Left.RightHasLine)
				{
					CreateLine(node, node.Right, "Left");
//					CreateLine(node.transform.localPosition - horizontalOffset,
//						node.Left.transform.localPosition + horizontalOffset);
//					node.LeftHasLine = node.Left.RightHasLine = true;
				}
			}
		}

		private void CreateLine(NetworkNode first, NetworkNode second, string key)
		{
			GameObject flowGo = null;
			if (key == "Up")
			{
				flowGo = CreateLine(first.transform.localPosition + verticalOffset,
					               second.transform.localPosition - verticalOffset);
				first.UpHasLine = second.DownHasLine = true;

				NetworkFlow flow = flowGo.GetComponent<NetworkFlow>();
				flow.FirstNode = first;
				flow.SecondNode = second;
				first.OutputUp.networkFlow = flow;
			}
			else if (key == "Right")
			{
				flowGo = CreateLine(first.transform.localPosition + horizontalOffset,
					               second.transform.localPosition - horizontalOffset);
				first.RightHasLine = second.LeftHasLine = true;

				NetworkFlow flow = flowGo.GetComponent<NetworkFlow>();
				flow.FirstNode = first;
				flow.SecondNode = second;
				first.OutputRight.networkFlow = flow;
			}
			else if (key == "Down")
			{
				flowGo = CreateLine(first.transform.localPosition - verticalOffset,
								   second.transform.localPosition + verticalOffset);
				first.DownHasLine = second.UpHasLine = true;

				NetworkFlow flow = flowGo.GetComponent<NetworkFlow>();
				flow.FirstNode = first;
				flow.SecondNode = second;
				first.OutputDown.networkFlow = flow;
			}
			else if (key == "Left")
			{
				flowGo = CreateLine(first.transform.localPosition - horizontalOffset,
								   second.transform.localPosition + horizontalOffset);
				first.LeftHasLine = second.RightHasLine = true;

				NetworkFlow flow = flowGo.GetComponent<NetworkFlow>();
				flow.FirstNode = first;
				flow.SecondNode = second;
				first.OutputLeft.networkFlow = flow;
			}
		}

		private GameObject CreateLine(Vector3 from, Vector3 to)
		{
			GameObject flow = Instantiate(NetworkFlowPrefab, NetworkFlows);
			LineRenderer lineRenderer = flow.GetComponent<LineRenderer>();
			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			lineRenderer.SetPositions(new Vector3[] {from, to});
			return flow;
		}
	}
}