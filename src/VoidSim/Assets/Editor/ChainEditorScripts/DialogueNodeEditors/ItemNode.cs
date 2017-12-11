using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemNode : Node
{
    private void OnEnable()
    {
        baseHeight = 83;
        baseWidth = 200;
        originalWindowTitle = "Items and " + DialogueChainPreferences.experienceString;
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Give Item", GUILayout.Width(130));
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            cEvent.itemsGiven.Add(null);
            cEvent.itemsGivenString.Add("");
        }
        if (GUILayout.Button("-", GUILayout.Width(25)))
        {
            if (cEvent.itemsGiven.Count > 0)
            {
                cEvent.itemsGiven.RemoveAt(cEvent.itemsGiven.Count - 1);
                cEvent.itemsGivenString.RemoveAt(cEvent.itemsGivenString.Count - 1);
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < cEvent.itemsGiven.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            height += 18;

            if (DialogueChainPreferences.itemsAreScriptableObjects)
            {
                cEvent.itemsGiven[i] = EditorGUILayout.ObjectField(cEvent.itemsGiven[i], typeof(Item), false, GUILayout.Width(135)) as Item;
            }
            else
            {
                cEvent.itemsGivenString[i] = EditorGUILayout.TextField(cEvent.itemsGivenString[i], GUILayout.Width(135));
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Take Item", GUILayout.Width(130));
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            cEvent.itemsTaken.Add(null);
            cEvent.itemsTakenString.Add("");
        }
        if (GUILayout.Button("-", GUILayout.Width(25)))
        {
            if (cEvent.itemsTaken.Count > 0)
            {
                cEvent.itemsTaken.RemoveAt(cEvent.itemsTaken.Count - 1);
                cEvent.itemsTakenString.RemoveAt(cEvent.itemsTakenString.Count - 1);
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < cEvent.itemsTaken.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            height += 18;

            if (DialogueChainPreferences.itemsAreScriptableObjects)
            {
                cEvent.itemsTaken[i] = EditorGUILayout.ObjectField(cEvent.itemsTaken[i], typeof(Item), false, GUILayout.Width(135)) as Item;
            }
            else
            {
                cEvent.itemsTakenString[i] = EditorGUILayout.TextField(cEvent.itemsTakenString[i], GUILayout.Width(135));
            }
            EditorGUILayout.EndHorizontal();
        } 

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Give " + DialogueChainPreferences.experienceString, GUILayout.Width(130));
        cEvent.experienceGiven = EditorGUILayout.IntField(cEvent.experienceGiven, GUILayout.Width(55));
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
