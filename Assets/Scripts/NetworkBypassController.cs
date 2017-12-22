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
		
		private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
		private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

		void Start()
		{
			ActiveList = new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());
			CreateLines();
			foreach (var node in ActiveList)
			{
				Debug.Log(node);
			}
		}

		void Update()
		{
//			foreach (var node in activeList)
//			{
//				if (node.Up != null)
//				{
//					DrawLine(node.transform.localPosition + verticalOffset,
//						node.Up.transform.localPosition - verticalOffset);
//				}
//				if (node.Right != null)
//				{
//					DrawLine(node.transform.localPosition + horizontalOffset,
//						node.Right.transform.localPosition - horizontalOffset);
//				}
//				if (node.Down != null)
//				{
//					DrawLine(node.transform.localPosition - verticalOffset,
//						node.Down.transform.localPosition + verticalOffset);
//				}
//				if (node.Left != null)
//				{
//					DrawLine(node.transform.localPosition - horizontalOffset,
//						node.Left.transform.localPosition + horizontalOffset);
//				}
//			}
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
			}
			else if (key == "Right")
			{
				flowGo = CreateLine(first.transform.localPosition + horizontalOffset,
					               second.transform.localPosition - horizontalOffset);
				first.RightHasLine = second.LeftHasLine = true;
			}
			else if (key == "Down")
			{
				flowGo = CreateLine(first.transform.localPosition - verticalOffset,
								   second.transform.localPosition + verticalOffset);
				first.UpHasLine = second.UpHasLine = true;
			}
			else if (key == "Left")
			{
				flowGo = CreateLine(first.transform.localPosition - horizontalOffset,
								   second.transform.localPosition + horizontalOffset);
				first.DownHasLine = second.RightHasLine = true;
			}
			if (flowGo != null)
			{
				NetworkFlow flow = flowGo.GetComponent<NetworkFlow>();
				flow.FirstNode = first;
				flow.SecondNode = second;
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