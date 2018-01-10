using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NetworkBypass.Editor
{
    [CustomEditor(typeof(NetworkNodeLock), true)]
    public class LockComponentEditor : UnityEditor.Editor
    {
        protected static string[] directionString = {"↑", "→", "↓", "←"};
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var comp = target as NetworkNodeLock;
            bool hasInputLock = false;
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Input Lock", GUILayout.MaxWidth(120f));
                GUILayoutOption[] option = {
                    GUILayout.MaxWidth(80f), GUILayout.MinWidth(40f)};
                for (int i = 0; i < NetworkNode.NeighborNum; i++)
                {
                    comp.InputLockSetting[i] = EditorGUILayout.ToggleLeft(
                        directionString[i], comp.InputLockSetting[i], option);
                    if (comp.InputLockSetting[i]) 
                        hasInputLock = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorUtility.SetDirty(target);
            
            if (!hasInputLock)
            {
                EditorGUILayout.HelpBox("结束节点需要至少一个输入锁", MessageType.Error);
            }
        }
    }
}