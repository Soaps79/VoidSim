using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public abstract class Node : Editor
{
    public ChainEvent cEvent = new ChainEvent();

    [HideInInspector]
    public string windowTitle = "Needs Title";
    [HideInInspector]
    public string originalWindowTitle = "Needs original title";
    [HideInInspector]
    public Rect windowRect;
    [HideInInspector]
    public float baseHeight = 40;
    [HideInInspector]
    public float height;
    [HideInInspector]
    public float baseWidth = 375;

    public List<Node> nextNodes = new List<Node>();
    public List<Node> previousNodes = new List<Node>();
    public List<Node> lateralNodes = new List<Node>();
    public List<List<Node>> nextNodesForInput = new List<List<Node>>();

    public int maxRank = 0;
    Node maxRankNode = null;
    public bool setRank = false;
    bool rankFromInput = false;
    int rankInputIndex = 0;

    public bool hasNonCheckNodeNext = false;
    public bool checkNode = false;

    public virtual void DrawWindow(DialogueChain quest)
    {
        Undo.RecordObject(this, "Undo: " + this.GetType());

        height = baseHeight;
        windowRect.width = baseWidth;
        HandleRank();
    }

    public virtual void DrawCurves()
    {
        List<int> nonCheckIndexes = new List<int>();
        int rankFirstNonCheck = 10000000;
        foreach (Node node2 in nextNodes)
        {
            if (!node2.checkNode)
            { 
                nonCheckIndexes.Add(node2.cEvent.rank);
                if (node2.cEvent.rank < rankFirstNonCheck)
                {
                    rankFirstNonCheck = node2.cEvent.rank;
                }
            }
        }

        foreach (Node node2 in nextNodes)
        {
            if (node2.checkNode)
            {
                if ((node2.maxRank == 0 || node2.maxRank == 1) || !hasNonCheckNodeNext)
                {
                    DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.yellow, true);
                }
                else
                {
                    bool drewLine = false;
                    for (int i = 0; i < nonCheckIndexes.Count; i++)
                    {
                        if (node2.cEvent.rank > nonCheckIndexes[i])
                        {
                            DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.red, true);
                            Debug.Log(node2.GetType().ToString() + " can never be reached.");
                            drewLine = true;
                            break;
                        }
                    }
                    if (!drewLine)
                    {
                        bool triggerOK = true;
                        foreach (ChainTrigger trigger in node2.cEvent.triggers)
                        {
                            if (trigger == null)
                            {
                                Debug.Log("Null trigger event ID " + node2.GetType().ToString());
                                DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.red, true);
                                triggerOK = false;
                                break;
                            }
                        }
                        if (triggerOK)
                        {
                            DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.cyan, true);
                        }
                    }
                }
            }
            else
            {
                bool triggerOK = true;
                if (node2 is TriggerNode)
                {
                    foreach (ChainTrigger trigger in node2.cEvent.triggers)
                    {
                        if (trigger == null)
                        {
                            Debug.Log("Trigger not set to a ChainTrigger. Trigger node is null.");
                            DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.red, true);
                            triggerOK = false;
                            break;
                        }
                    }
                }
                if (node2.cEvent.rank == rankFirstNonCheck && triggerOK)
                {
                    DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.green, true);
                }
                else
                {
                    DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.red, true);
                    Debug.Log(node2.GetType().ToString() + " can never be reached");
                }
            }
        }
        foreach (Node node2 in lateralNodes)
        {
            DrawNodeCurve(windowRect, node2.windowRect, 20, -20, Color.white, false);
        }
    }

    public virtual void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        Vector3 startPos;
        Vector3 endPos;
        Vector3 startTan;
        Vector3 endTan;

        if (rightLeftConnect)
        {
            startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            endPos = new Vector3(end.x, end.y + end.height / 2, 0);

            startTan = startPos + Vector3.right * sTanMod;
            endTan = endPos + Vector3.left * eTanMod;
        }
        else
        {
            startPos = new Vector3(start.x + start.width, start.y + start.height, 0);
            endPos = new Vector3(end.x, end.y, 0);

            startTan = startPos + Vector3.up * sTanMod;
            endTan = endPos + Vector3.down * eTanMod;

        }


        Color shadowCol = new Color(color.r, color.g, color.b, 0.06f);

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 1);
    }

    public virtual void HandleTitle()
    {
        windowTitle = originalWindowTitle;

        if (cEvent.cEventType == ChainEventType.Start)
        {
            if (previousNodes.Count > 0)
            {
                windowTitle = originalWindowTitle + " (Has Previous)";
            }
        }
        else if (nextNodes.Count == 0)
        {
            windowTitle = originalWindowTitle + " (Ending)";
        }
        else if (previousNodes.Count == 0)
        {
            windowTitle = originalWindowTitle + " (No Previous)";
        }
    }

    public virtual void HandleRank()
    {
        try
        {
            EditorGUILayout.BeginHorizontal();
            if (cEvent.cEventType != ChainEventType.Start)
            {
                int tempMaxRank = 0;
                foreach (Node pNode in previousNodes)
                {
                    if (pNode is UserInputNode)
                    {
                        foreach (List<Node> nextInputNodes in pNode.nextNodesForInput)
                        {
                            if (nextInputNodes.Contains(this) && nextInputNodes.Count > tempMaxRank)
                            {
                                maxRankNode = pNode;
                                tempMaxRank = nextInputNodes.Count;
                                rankInputIndex = pNode.nextNodesForInput.IndexOf(nextInputNodes);
                                rankFromInput = true;
                            }
                        }
                    }
                    if (pNode.nextNodes.Count > tempMaxRank)
                    {
                        maxRankNode = pNode;
                        tempMaxRank = pNode.nextNodes.Count;
                        rankFromInput = false;
                    }
                }
                if (maxRank != tempMaxRank)
                {
                    CheckForChecks();
                    maxRank = tempMaxRank;
                }
                if (maxRank > 1)
                {
                    bool tempHasNonCheckNodeNext = false;
                    if (!setRank)
                    {
                        if (rankFromInput)
                        {
                            cEvent.rank = maxRankNode.nextNodesForInput[rankInputIndex].IndexOf(this);
                        }
                        else
                        {
                            cEvent.rank = maxRankNode.nextNodes.IndexOf(this);
                        }
                        setRank = true;
                    }
                    EditorGUILayout.LabelField("Rank ", GUILayout.Width(35));
                    int originalRank = cEvent.rank;
                    cEvent.rank = (int)EditorGUILayout.Slider(cEvent.rank + 1, 1, maxRank, GUILayout.Width(Mathf.Clamp(baseWidth - 50, 115, baseWidth - 50))) - 1;
                    if (originalRank != cEvent.rank)
                    {
                        int numberMoved = Mathf.Abs(cEvent.rank - originalRank);
                        if (rankFromInput)
                        {
                            foreach (Node dNode in maxRankNode.nextNodesForInput[rankInputIndex])
                            {
                                if (!dNode.checkNode)
                                {
                                    tempHasNonCheckNodeNext = true;
                                }
                                if (originalRank < cEvent.rank)
                                {
                                    if (dNode.cEvent.rank > originalRank && dNode.cEvent.rank <= originalRank + numberMoved && dNode != this)
                                    {
                                        dNode.cEvent.rank--;
                                    }
                                }
                                else
                                {
                                    if (dNode.cEvent.rank < originalRank && dNode.cEvent.rank >= originalRank - numberMoved && dNode != this)
                                    {
                                        dNode.cEvent.rank++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (Node dNode in maxRankNode.nextNodes)
                            {
                                if (!dNode.checkNode)
                                {
                                    tempHasNonCheckNodeNext = true;
                                }
                                if (originalRank < cEvent.rank)
                                {
                                    if (dNode.cEvent.rank > originalRank && dNode.cEvent.rank <= originalRank + numberMoved && dNode != this)
                                    {
                                        dNode.cEvent.rank--;
                                    }
                                }
                                else
                                {
                                    if (dNode.cEvent.rank < originalRank && dNode.cEvent.rank >= originalRank - numberMoved && dNode != this)
                                    {
                                        dNode.cEvent.rank++;
                                    }
                                }
                            }
                        }
                        maxRankNode.hasNonCheckNodeNext = tempHasNonCheckNodeNext;
                    }
                    height += 20;
                }
                else
                {
                    setRank = false;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        catch (System.ArgumentException)
        { }
    }

    public virtual void CheckForChecks()
    {
        bool tempHasNonCheckNodeNext = false;

        if (rankFromInput)
        {
            foreach (Node dNode in maxRankNode.nextNodesForInput[rankInputIndex])
            {
                if (!dNode.checkNode)
                {
                    tempHasNonCheckNodeNext = true;
                }             
            }
        }
        else
        {
            foreach (Node dNode in maxRankNode.nextNodes)
            {
                if (!dNode.checkNode)
                {
                    tempHasNonCheckNodeNext = true;
                }
            }
        }
        maxRankNode.hasNonCheckNodeNext = tempHasNonCheckNodeNext;      
    }
}

