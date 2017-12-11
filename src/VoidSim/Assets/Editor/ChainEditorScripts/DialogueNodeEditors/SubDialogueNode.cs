using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SubDialogueNode : Node
{
    private void OnEnable()
    {
        baseHeight = 45;
        baseWidth = 230;
        originalWindowTitle = "Sub Dialogue Chain";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sub Chain", GUILayout.Width(65));       
        cEvent.subDialogue = EditorGUILayout.ObjectField(cEvent.subDialogue, typeof(DialogueChain), false, GUILayout.Width(145)) as DialogueChain;
        EditorGUILayout.EndHorizontal();

        windowRect.height = height;
        cEvent.windowRect = windowRect;
    }

    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        base.DrawNodeCurve(start, end, sTanMod, eTanMod, color, rightLeftConnect);
    }
}
