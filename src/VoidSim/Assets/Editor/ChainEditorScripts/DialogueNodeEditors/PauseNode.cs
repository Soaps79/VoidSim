using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseNode : Node
{
    private void OnEnable()
    {
        baseHeight = 40;
        baseWidth = 165;
        originalWindowTitle = "Pause";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();
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
