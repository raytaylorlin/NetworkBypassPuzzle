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
        private SerializedProperty northProp;
        private SerializedProperty eastProp;
        private SerializedProperty southProp;
        private SerializedProperty westProp;
        
        private InspectorTips inspectorTips;
        float size = 1f;

        void OnEnable () 
        {
            northProp = serializedObject.FindProperty("North");
            eastProp = serializedObject.FindProperty("East");
            southProp = serializedObject.FindProperty("South");
            westProp = serializedObject.FindProperty("West");
        }

        protected virtual void OnSceneGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
            }
        }

        public override void OnInspectorGUI()
        {
            NetworkNode script = target as NetworkNode;

//            base.OnInspectorGUI();
            Draw(northProp, "North");
            Draw(eastProp, "East");
            Draw(southProp, "South");
            Draw(westProp, "West");

            if (inspectorTips != null)
            {
                EditorGUILayout.HelpBox(inspectorTips.Message, inspectorTips.Type);
            }
            
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
            if (key == "North") node.South = value as NetworkNode;
            if (key == "South") node.North = value as NetworkNode;
            if (key == "East") node.West = value as NetworkNode;
            if (key == "West") node.East = value as NetworkNode;
        }
        
        private SerializedProperty FindRepeatProperty(Object currentValue, string key)
        {
            if (key == "North")
            {
                if (eastProp.objectReferenceValue == currentValue) return eastProp;
                if (southProp.objectReferenceValue == currentValue) return southProp;
                if (westProp.objectReferenceValue == currentValue) return westProp;
            }
            if (key == "East")
            {
                if (northProp.objectReferenceValue == currentValue) return northProp;
                if (southProp.objectReferenceValue == currentValue) return southProp;
                if (westProp.objectReferenceValue == currentValue) return westProp;
            }
            if (key == "South")
            {
                if (northProp.objectReferenceValue == currentValue) return northProp;
                if (eastProp.objectReferenceValue == currentValue) return eastProp;
                if (westProp.objectReferenceValue == currentValue) return westProp;
            }
            if (key == "West")
            {
                if (northProp.objectReferenceValue == currentValue) return northProp;
                if (eastProp.objectReferenceValue == currentValue) return eastProp;
                if (southProp.objectReferenceValue == currentValue) return southProp;
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