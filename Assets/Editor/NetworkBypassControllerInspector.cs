using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NetworkBypass.Editor
{
	[CustomEditor(typeof(NetworkBypassController), true)]
	public class NetworkBypassControllerInspector : UnityEditor.Editor
	{
		private NetworkBypassController self;
		private Dictionary<string, string> prefabDict;
		private string[] popupKeys;
		private static int selectIndex = 0;
		
		void OnEnable()
		{
			self = target as NetworkBypassController;
			
			prefabDict = new Dictionary<string, string>();
			prefabDict.Add("Start", "StartNode");
			prefabDict.Add("End", "EndNode");
			prefabDict.Add("Straight Connection", "StraightConnectionNode");
			prefabDict.Add("RightAngle Connection", "RightAngleConnectionNode");
			prefabDict.Add("T Connection", "TConnectionNode");
			prefabDict.Add("Cross Connection", "CrossConnectionNode");

			var keys = prefabDict.Keys;
			popupKeys = new string[keys.Count];
			keys.CopyTo(popupKeys, 0);
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Type");
				selectIndex = EditorGUILayout.Popup(selectIndex, popupKeys);
				if (GUILayout.Button("Create"))
				{
					string key = popupKeys[selectIndex];
					string assetName = prefabDict[key];
					string path = string.Format("Assets/Prefabs/{0}.prefab", assetName);
					GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
					GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
					go.name = go.name.Replace("(Clone)", "");
					go.transform.SetParent(self.transform);
					Undo.RegisterCreatedObjectUndo(go, "Instantiate node prefab");
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}