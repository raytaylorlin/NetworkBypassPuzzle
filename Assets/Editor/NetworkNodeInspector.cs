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
        private SerializedProperty upProp;
        private SerializedProperty rightProp;
        private SerializedProperty downProp;
        private SerializedProperty leftProp;
        
        private InspectorTips inspectorTips;
        
        private static Vector3 horizontalOffset = new Vector3(0.45f, 0, 0);
        private static Vector3 verticalOffset = new Vector3(0, 0.45f, 0);

        void OnEnable () 
        {
            self = target as NetworkNode;
            upProp = serializedObject.FindProperty("Up");
            rightProp = serializedObject.FindProperty("Right");
            downProp = serializedObject.FindProperty("Down");
            leftProp = serializedObject.FindProperty("Left");
        }

        protected virtual void OnSceneGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (upProp.objectReferenceValue != null)
                {
                    var node = upProp.objectReferenceValue as NetworkNode;
                    Handles.DrawLine(self.transform.localPosition + verticalOffset,
						node.transform.localPosition - verticalOffset);
				}
				if (rightProp.objectReferenceValue != null)
				{
				    var node = rightProp.objectReferenceValue as NetworkNode;
				    Handles.DrawLine(self.transform.localPosition + horizontalOffset,
						node.transform.localPosition - horizontalOffset);
				}
				if (downProp.objectReferenceValue != null)
				{
				    var node = downProp.objectReferenceValue as NetworkNode;
				    Handles.DrawLine(self.transform.localPosition - verticalOffset,
						node.transform.localPosition + verticalOffset);
				}
				if (leftProp.objectReferenceValue != null)
				{
				    var node = leftProp.objectReferenceValue as NetworkNode;
				    Handles.DrawLine(self.transform.localPosition - horizontalOffset,
						node.transform.localPosition + horizontalOffset);
				}
            }
        }

        public override void OnInspectorGUI()
        {
            NetworkNode script = target as NetworkNode;

            Draw(upProp, "Up");
            Draw(rightProp, "Right");
            Draw(downProp, "Down");
            Draw(leftProp, "Left");

            if (inspectorTips != null)
            {
                EditorGUILayout.HelpBox(inspectorTips.Message, inspectorTips.Type);
            }
//            base.OnInspectorGUI();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void Draw(SerializedProperty prop, string key)
        {
            Object lastValue = prop.objectReferenceValue;
            EditorGUILayout.PropertyField(prop);
            Object currentValue = prop.objectReferenceValue;
            if (currentValue == target)
            {
                prop.objectReferenceValue = lastValue;
                ShowTips("不能将自己设为目标");
                return;
            }
            if (currentValue == lastValue)
            {
                if (currentValue == null) prop.objectReferenceValue = null;
                return;
            }
            if (currentValue == null)
            {
                SetOppositeNode(lastValue, key, null);
                Debug.Log(string.Format("Set {0} to null", key));
            }
            else 
            {
                SerializedProperty sp = FindRepeatProperty(currentValue, key);
                if (sp != null)
                {
                    ShowTips(string.Format("节点{0}不能应用在多个出口，已调整到新的出口", sp.objectReferenceValue.name));
                    SetOppositeNode(sp.objectReferenceValue, sp.displayName, null);
                    sp.objectReferenceValue = null;
                }
                SetOppositeNode(currentValue, key, target);
                Debug.Log(string.Format("Set {0} to {1}", key, currentValue));
            }
        }

        private void SetOppositeNode(Object nodeObj, string key, Object value)
        {
            NetworkNode node = nodeObj as NetworkNode;
            if (node == null) return;
            if (key == "Up") node.Down = value as NetworkNode;
            if (key == "Down") node.Up = value as NetworkNode;
            if (key == "Right") node.Left = value as NetworkNode;
            if (key == "Left") node.Right = value as NetworkNode;
        }
        
        private SerializedProperty FindRepeatProperty(Object currentValue, string key)
        {
            if (key == "Up")
            {
                if (rightProp.objectReferenceValue == currentValue) return rightProp;
                if (downProp.objectReferenceValue == currentValue) return downProp;
                if (leftProp.objectReferenceValue == currentValue) return leftProp;
            }
            if (key == "Right")
            {
                if (upProp.objectReferenceValue == currentValue) return upProp;
                if (downProp.objectReferenceValue == currentValue) return downProp;
                if (leftProp.objectReferenceValue == currentValue) return leftProp;
            }
            if (key == "Down")
            {
                if (upProp.objectReferenceValue == currentValue) return upProp;
                if (rightProp.objectReferenceValue == currentValue) return rightProp;
                if (leftProp.objectReferenceValue == currentValue) return leftProp;
            }
            if (key == "Left")
            {
                if (upProp.objectReferenceValue == currentValue) return upProp;
                if (rightProp.objectReferenceValue == currentValue) return rightProp;
                if (downProp.objectReferenceValue == currentValue) return downProp;
            }
            return null;
        }

        private void ShowTips(string message, MessageType type = MessageType.Info)
        {
            inspectorTips = new InspectorTips
            {
                Message = message,
                Type = type
            };
        }
    }
}