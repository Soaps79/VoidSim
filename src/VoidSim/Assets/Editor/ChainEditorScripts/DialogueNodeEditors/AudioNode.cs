using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudioNode : Node
{
    private void OnEnable()
    {
        baseHeight = 78;
        baseWidth = 180;
        originalWindowTitle = "Audio";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Loop", GUILayout.Width(30));
        cEvent.loop = EditorGUILayout.Toggle(cEvent.loop, GUILayout.Width(15));
        EditorGUIUtility.labelWidth = 2;
        EditorGUILayout.LabelField("Fade (sec)", GUILayout.Width(68));
        cEvent.fadeTime = EditorGUILayout.FloatField(":", Mathf.Clamp(cEvent.fadeTime, 0, cEvent.fadeTime), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Overlay", GUILayout.Width(50));
        cEvent.overlay = EditorGUILayout.Toggle(cEvent.overlay, GUILayout.Width(15));
        EditorGUILayout.LabelField("Volume", GUILayout.Width(48));
        cEvent.audioVolume = EditorGUILayout.Slider(cEvent.audioVolume, 0, 1, GUILayout.Width(35));
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 0;

        if (!cEvent.overlay && !cEvent.loop)
        {
            height += 20;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Play Original Music After", GUILayout.Width(150));
            cEvent.playOriginalAfter = EditorGUILayout.Toggle(cEvent.playOriginalAfter, GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            cEvent.playOriginalAfter = false;
        }

        if (cEvent.playOriginalAfter)
        {
            height += 20;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Original Music Fade", GUILayout.Width(115));
            EditorGUIUtility.labelWidth = 2;
            cEvent.originalFadeTime = EditorGUILayout.FloatField(":", cEvent.originalFadeTime, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
        }


        cEvent.audio = EditorGUILayout.ObjectField(cEvent.audio, typeof(AudioClip), false, GUILayout.Width(170)) as AudioClip;

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
