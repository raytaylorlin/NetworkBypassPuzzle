using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NetworkBypass
{
    class InspectorTips
    {
        public string Message;
        public MessageType Type;
    }
    
    [CustomEditor(typeof(NetworkNode), true)]
    public class NetworkNodeInspector : Editor
    {
        private NetworkNode self;

        private InspectorTips inspectorTips;
        
        private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
        private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

        protected virtual void OnEnable () 
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
                for (int i = 0; i < self.Neighbors.Length; i++)
                {
                    var neighbor = self.Neighbors[i];
                    if (neighbor != null)
                    {
                        Vector3 offset = GetDrawLineOffset((NetworkNode.Direction) i);
                        Handles.DrawLine(self.transform.localPosition + offset,
                            neighbor.transform.localPosition - offset);
                    }
                }
            }
        }

        private Vector3 GetDrawLineOffset(NetworkNode.Direction direction)
        {
            if (direction == NetworkNode.Direction.Up) return verticalOffset;
            if (direction == NetworkNode.Direction.Right) return horizontalOffset;
            if (direction == NetworkNode.Direction.Down) return -verticalOffset;
            if (direction == NetworkNode.Direction.Left) return -horizontalOffset;
            return Vector3.zero;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            DrawFields();

            if (inspectorTips != null)
            {
                EditorGUILayout.HelpBox(inspectorTips.Message, inspectorTips.Type);
            }

            
            serializedObject.ApplyModifiedProperties();
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
                neighbor.SetNode(NetworkNode.GetOppositeDirection(direction), null);
                self.SetNode(direction, null);
                Debug.Log(string.Format("Set {0} to null", direction));
            }
            else
            {
                NetworkNode.Direction repeatDirection = FindRepeatNodeDirection(newValue, direction);
                if (repeatDirection != NetworkNode.Direction.Unknown)
                {
                    ShowTips(string.Format("节点{0}不能应用在多个出口，已调整到新的出口", newValue.name));
                    self.GetNode(repeatDirection).SetNode(NetworkNode.GetOppositeDirection(repeatDirection), null);
                    self.SetNode(repeatDirection, null);
                }
                newValue.SetNode(NetworkNode.GetOppositeDirection(direction), self);
                self.SetNode(direction, newValue);
                Debug.Log(string.Format("Set {0} to {1}", direction, newValue));
            }
        }

        private NetworkNode.Direction FindRepeatNodeDirection(NetworkNode newValue, NetworkNode.Direction direction)
        {
            for (int i = 0; i < self.Neighbors.Length; i++)
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
    }

    [CustomEditor(typeof(StartNode), true)]
    public class StartNodeInspector : NetworkNodeInspector
    {
        private SerializedProperty outputUpEnableProp;
        private SerializedProperty outputRightEnableProp;
        private SerializedProperty outputDownEnableProp;
        private SerializedProperty outputLeftEnableProp;
        
        protected override void OnEnable () 
        {
            base.OnEnable();
            outputUpEnableProp = serializedObject.FindProperty("OutputUpEnable");
            outputRightEnableProp = serializedObject.FindProperty("OutputRightEnable");
            outputDownEnableProp = serializedObject.FindProperty("OutputDownEnable");
            outputLeftEnableProp = serializedObject.FindProperty("OutputLeftEnable");
        }
        
        protected override void DrawFields()
        {
            base.DrawFields();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Output Setting", GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {
                    GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                outputUpEnableProp.boolValue = EditorGUILayout.ToggleLeft("↑", outputUpEnableProp.boolValue, option);
                outputRightEnableProp.boolValue = EditorGUILayout.ToggleLeft("→", outputRightEnableProp.boolValue, option);
                outputDownEnableProp.boolValue = EditorGUILayout.ToggleLeft("↓", outputDownEnableProp.boolValue, option);
                outputLeftEnableProp.boolValue = EditorGUILayout.ToggleLeft("←", outputLeftEnableProp.boolValue, option);
            }
            EditorGUILayout.EndHorizontal();
            
            if (!outputUpEnableProp.boolValue &&
                !outputRightEnableProp.boolValue &&
                !outputDownEnableProp.boolValue &&
                !outputLeftEnableProp.boolValue)
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
        private SerializedProperty inputUpLockProp;
        private SerializedProperty inputRightLockProp;
        private SerializedProperty inputDownLockProp;
        private SerializedProperty inputLeftLockProp;
        
        protected override void OnEnable () 
        {
            base.OnEnable();
            inputUpLockProp = serializedObject.FindProperty("InputUpLock");
            inputRightLockProp = serializedObject.FindProperty("InputRightLock");
            inputDownLockProp = serializedObject.FindProperty("InputDownLock");
            inputLeftLockProp = serializedObject.FindProperty("InputLeftLock");
        }
        
        protected override void DrawFields()
        {
            base.DrawFields();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Input Lock", GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {
                    GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                inputUpLockProp.boolValue = EditorGUILayout.ToggleLeft("↑", inputUpLockProp.boolValue, option);
                inputRightLockProp.boolValue = EditorGUILayout.ToggleLeft("→", inputRightLockProp.boolValue, option);
                inputDownLockProp.boolValue = EditorGUILayout.ToggleLeft("↓", inputDownLockProp.boolValue, option);
                inputLeftLockProp.boolValue = EditorGUILayout.ToggleLeft("←", inputLeftLockProp.boolValue, option);
            }
            EditorGUILayout.EndHorizontal();

            if (!inputUpLockProp.boolValue &&
                !inputRightLockProp.boolValue &&
                !inputDownLockProp.boolValue &&
                !inputLeftLockProp.boolValue)
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