using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IntAdjustmentNode : Node
{
    private void OnEnable()
    {
        baseHeight = 47;
        baseWidth = 200;
        originalWindowTitle = "Integer Adjustment";
    }

    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            cEvent.chainIntAdjustments.Add(new IntAdjustment());
        }
        if (GUILayout.Button("Remove"))
        {
            if (cEvent.chainIntAdjustments.Count > 0)
            {
                cEvent.chainIntAdjustments.Remove(cEvent.chainIntAdjustments[cEvent.chainIntAdjustments.Count - 1]);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (cEvent.chainIntAdjustments.Count == 0)
        {
            cEvent.chainIntAdjustments.Add(new IntAdjustment());
        }
        
        for (int i = 0; i < cEvent.chainIntAdjustments.Count; i++)
        {
            height += 18;

            EditorGUILayout.BeginHorizontal();
            cEvent.chainIntAdjustments[i].intAdjusted = (ChainIntType)EditorGUILayout.EnumPopup(cEvent.chainIntAdjustments[i].intAdjusted, GUILayout.Width(120));
            EditorGUILayout.LabelField("+", GUILayout.Width(15));
            cEvent.chainIntAdjustments[i].value = EditorGUILayout.IntField(cEvent.chainIntAdjustments[i].value, GUILayout.Width(45));
            EditorGUILayout.EndHorizontal();
        }

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
