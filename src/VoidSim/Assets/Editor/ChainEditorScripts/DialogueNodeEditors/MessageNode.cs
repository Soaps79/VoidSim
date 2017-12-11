using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MessageNode : Node
{
    private void OnEnable()
    {
        baseHeight = 80;
        baseWidth = 165;
        originalWindowTitle = "Send Message";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        cEvent.sendMessage[0] = EditorGUILayout.Toggle(cEvent.sendMessage[0], GUILayout.Width(15));
        EditorGUILayout.LabelField("Float: ", GUILayout.Width(50));
        if (cEvent.sendMessage[0])
        {
            cEvent.messageFloat = EditorGUILayout.FloatField(cEvent.messageFloat, GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        cEvent.sendMessage[1] = EditorGUILayout.Toggle(cEvent.sendMessage[1], GUILayout.Width(15));
        EditorGUILayout.LabelField("String: ", GUILayout.Width(50));
        if (cEvent.sendMessage[1])
        {
            cEvent.messageString = EditorGUILayout.TextField(cEvent.messageString, GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        cEvent.sendMessage[2] = EditorGUILayout.Toggle(cEvent.sendMessage[2], GUILayout.Width(15));
        EditorGUILayout.LabelField("Bool: ", GUILayout.Width(50));
        if (cEvent.sendMessage[2])
        {
            cEvent.messageBool = EditorGUILayout.Toggle(cEvent.messageBool, GUILayout.Width(80));
        }
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
