using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NetworkBypass.Editor
{
    class InspectorTips
    {
        public string Message;
        public MessageType Type;
    }
    
    [CustomEditor(typeof(NetworkNode), true)]
    public class NetworkNodeInspector : UnityEditor.Editor
    {
        protected NetworkNode self;
        private InspectorTips inspectorTips;
        
        protected static string[] directionString = {"↑", "→", "↓", "←"};

        protected void OnEnable()
        {
            self = target as NetworkNode;
        }

        protected virtual void OnSceneGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    var neighbor = self.Neighbors[i];
                    if (neighbor != null)
                    {
                        Vector3 offset = NetworkNode.GetDrawLineOffset((NetworkNode.Direction) i);
                        Handles.DrawLine(self.transform.localPosition + offset,
                            neighbor.transform.localPosition - offset);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorTools.DrawHeader("Designer"))
            {
                EditorTools.BeginContents();
                {
                    DrawFields();
                }
                EditorTools.EndContents();
            }
            if (inspectorTips != null)
            {
                EditorGUILayout.HelpBox(inspectorTips.Message, inspectorTips.Type);
            }

            serializedObject.ApplyModifiedProperties();
            
            if (EditorTools.DrawHeader("Debug"))
            {
                EditorTools.BeginContents();
                {
                    DrawDebugFields();
                }
                EditorTools.EndContents();
            }
        }

        protected virtual void DrawFields()
        {
            DrawNode(NetworkNode.Direction.Up);
            DrawNode(NetworkNode.Direction.Right);
            DrawNode(NetworkNode.Direction.Down);
            DrawNode(NetworkNode.Direction.Left);
        }

        private void DrawNode(NetworkNode.Direction direction)
        {
            NetworkNode neighbor = self.Neighbors[(int) direction];
            NetworkNode newValue = EditorGUILayout.ObjectField(
                direction.ToString(), neighbor, typeof(NetworkNode), true) as NetworkNode;
            if (newValue == self)
            {
                ShowTips("不能将自己设为目标");
                return;
            }
            if (newValue == neighbor)
            {
                return;
            }
            if (newValue == null)
            {
                neighbor.SetNeighbor(NetworkNode.GetOppositeDirection(direction), null);
                self.SetNeighbor(direction, null);
                Debug.Log(string.Format("Set {0} to null", direction));
            }
            else
            {
                NetworkNode.Direction repeatDirection = FindRepeatNodeDirection(newValue, direction);
                if (repeatDirection != NetworkNode.Direction.Unknown)
                {
                    ShowTips(string.Format("节点{0}不能应用在多个出口，已调整到新的出口", newValue.name));
                    self.GetNeighbor(repeatDirection).SetNeighbor(NetworkNode.GetOppositeDirection(repeatDirection), null);
                    self.SetNeighbor(repeatDirection, null);
                }
                newValue.SetNeighbor(NetworkNode.GetOppositeDirection(direction), self);
                self.SetNeighbor(direction, newValue);
                Debug.Log(string.Format("Set {0} to {1}", direction, newValue));
            }
        }

        private NetworkNode.Direction FindRepeatNodeDirection(NetworkNode newValue, NetworkNode.Direction direction)
        {
            for (int i = 0; i < NetworkNode.NeighborNum; i++)
            {
                // 跳过自身
                if (i == (int) direction) continue;
                if (self.Neighbors[i] == newValue) return (NetworkNode.Direction) i;
            }
            return NetworkNode.Direction.Unknown;
        }

        protected void ShowTips(string message, MessageType type = MessageType.Info)
        {
            inspectorTips = new InspectorTips
            {
                Message = message,
                Type = type
            };
        }

        protected void ClearTips()
        {
            inspectorTips = null;
        }

        protected virtual void DrawDebugFields()
        {
            DrawIOFields(self.Inputs, "Input");
            DrawIOFields(self.Outputs, "Output");
        }

        private void DrawIOFields(bool[] flags, string title)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(title, GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                for (int i = 0; i < flags.Length; i++)
                {
                    EditorGUILayout.ToggleLeft(directionString[i], flags[i], option);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    [CustomEditor(typeof(StartNode), true)]
    public class StartNodeInspector : NetworkNodeInspector
    {
        protected override void DrawFields()
        {
            StartNode node = target as StartNode;
            
            base.DrawFields();
            bool hasOutput = false;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Output Setting", GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {
                    GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    node.OutputSetting[i] = EditorGUILayout.ToggleLeft(
                        directionString[i], node.OutputSetting[i], option);
                    if (node.OutputSetting[i]) 
                        hasOutput = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            if (!hasOutput)
            {
                ShowTips("开始节点需要至少一个输出", MessageType.Warning);
            }
            else
            {
                ClearTips();
            }
        }
    }
    
    [CustomEditor(typeof(EndNode), true)]
    public class EndNodeInspector : NetworkNodeInspector
    {
        protected override void DrawFields()
        {
            EndNode node = target as EndNode;
            
            base.DrawFields();
            bool hasInputLock = false;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Input Lock", GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {
                    GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    node.InputLockSetting[i] = EditorGUILayout.ToggleLeft(
                        directionString[i], node.InputLockSetting[i], option);
                    if (node.InputLockSetting[i]) 
                        hasInputLock = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!hasInputLock)
            {
                ShowTips("结束节点需要至少一个输入锁", MessageType.Error);
            }
            else
            {
                ClearTips();
            }
        }
    }
}