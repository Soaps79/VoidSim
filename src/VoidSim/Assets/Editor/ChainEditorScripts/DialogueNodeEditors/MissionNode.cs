using Assets.Narrative.Missions;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ChainEditorScripts.DialogueNodeEditors
{
    public class MissionNode : Node
    {
        private void OnEnable()
        {
            baseHeight = 60;
            baseWidth = 200;
            originalWindowTitle = "New Mission";
        }
        public override void DrawWindow(DialogueChain chain)
        {
            base.DrawWindow(chain);
            HandleTitle();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Begin Missions", GUILayout.Width(130));
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                cEvent.missions.Add(null);
            }
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                if (cEvent.missions.Count > 0)
                {
                    cEvent.missions.RemoveAt(cEvent.missions.Count - 1);
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < cEvent.missions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                height += 18;
                cEvent.missions[i] = EditorGUILayout.ObjectField(cEvent.missions[i], typeof(MissionSO), false, GUILayout.Width(190)) as MissionSO;

                EditorGUILayout.EndHorizontal();
            }

            windowRect.height = height;
            cEvent.windowRect = windowRect;
        }
    }
}