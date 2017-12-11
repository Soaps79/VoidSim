using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SecondaryInputNode : Node
{
    string newTrig = "";

    private void OnEnable()
    {
        baseHeight = 67;
        baseWidth = 300;
        windowTitle = "Secondary Input";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Input"))
        {
            cEvent.inputButtons.Add(new DialogueEventInputButton());
            nextNodesForInput.Add(new List<Node>());
        }
        if (cEvent.inputButtons.Count > 0)
        {
            if (GUILayout.Button("Remove Input"))
            {
                cEvent.inputButtons.Remove(cEvent.inputButtons[cEvent.inputButtons.Count - 1]);
                nextNodesForInput.Remove(nextNodesForInput[nextNodesForInput.Count - 1]);
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < cEvent.inputButtons.Count; i++)
        {
            height += 20;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Input Text:", GUILayout.Width(70));
            cEvent.inputButtons[i].buttonText = EditorGUILayout.TextField(cEvent.inputButtons[i].buttonText);
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                DialogueChainEditorWindow editor = EditorWindow.GetWindow<DialogueChainEditorWindow>();
                editor.connectingInputNode = true;
                editor.selectedInputNodeIndex = i;
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                DialogueChainEditorWindow editor = EditorWindow.GetWindow<DialogueChainEditorWindow>();
                editor.deletingInputConnection = true;
                editor.selectedInputNodeIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        //Initial buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Trigger"))
        {
            cEvent.triggerChecks.Add(null);
            cEvent.triggerCheckBools.Add(true);
        }
        if (GUILayout.Button("Item"))
        {
            cEvent.itemChecks.Add(null);
            cEvent.itemChecksString.Add(null);
        }
        if (GUILayout.Button("Int"))
        {
            cEvent.chainIntChecks.Add(new IntCheck());
        }

        EditorGUILayout.EndHorizontal();

        if (cEvent.triggerChecks.Count > 0 || cEvent.itemChecks.Count > 0 || cEvent.chainIntChecks.Count > 0)
        {
            height += 20;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            int count = 0;
            if (cEvent.triggerChecks.Count > 0)
            {
                count++;
            }
            if (cEvent.itemChecks.Count > 0)
            {
                count++;
            }
            if (cEvent.chainIntChecks.Count > 0)
            {
                count++;
            }
            if (count > 1)
            {
                height += 20 * (count - 1);
            }
        }


        //Trigger Check
        if (cEvent.triggerChecks.Count > 0)
        {
            height += 40;
            EditorGUILayout.LabelField("Trigger Checks", EditorStyles.boldLabel);

            for (int i = 0; i < cEvent.triggerChecks.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                height += 20;

                EditorGUILayout.LabelField("If ", GUILayout.Width(15));
                cEvent.triggerChecks[i] = (ChainTrigger)EditorGUILayout.ObjectField(cEvent.triggerChecks[i], typeof(ChainTrigger), false, GUILayout.Width(150));
                cEvent.triggerCheckBools[i] = EditorGUILayout.Toggle(cEvent.triggerCheckBools[i], GUILayout.Width(15));
                EditorGUILayout.LabelField("is " + ((cEvent.triggerCheckBools[i]) ? "true" : "false"), GUILayout.Width(50));

                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    cEvent.triggerChecks.RemoveAt(i);
                    cEvent.triggerCheckBools.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New", GUILayout.Width(65)))
            {
                if (newTrig == "")
                {
                    Debug.Log("Cannot add a trigger with no name");
                }
                else
                {
                    if (cEvent.triggerChecks[cEvent.triggerChecks.Count - 1] == null)
                    {
                        cEvent.triggerChecks[cEvent.triggerChecks.Count - 1] = TriggerNode.CreateTrigger(newTrig);
                        cEvent.triggerCheckBools[cEvent.triggerCheckBools.Count - 1] = false;
                    }
                    else
                    {
                        cEvent.triggerChecks.Add(TriggerNode.CreateTrigger(newTrig));
                        cEvent.triggerCheckBools.Add(false);
                    }
                    newTrig = "";
                }
            }
            newTrig = EditorGUILayout.TextField(newTrig);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(" ");
        }


        //Item Check
        if (cEvent.itemChecks.Count > 0)
        {
            height += 35;
            EditorGUILayout.LabelField("Item Checks", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("If inventory contains:", GUILayout.Width(130));

            for (int i = 0; i < cEvent.itemChecks.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                height += 20;
                if (DialogueChainPreferences.itemsAreScriptableObjects)
                {
                    cEvent.itemChecks[i] = EditorGUILayout.ObjectField(cEvent.itemChecks[i], typeof(Item), false, GUILayout.Width(245)) as Item;
                }
                else
                {
                    cEvent.itemChecksString[i] = EditorGUILayout.TextField(cEvent.itemChecksString[i], GUILayout.Width(245));
                }

                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    cEvent.itemChecks.RemoveAt(i);
                    cEvent.itemChecksString.RemoveAt(i);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField(" ");
        }


        //Int Check
        if (cEvent.chainIntChecks.Count > 0)
        {
            height += 20;
            EditorGUILayout.LabelField("Integer Checks", EditorStyles.boldLabel);

            if (cEvent.chainIntChecks.Count > 0)
            {
                for (int i = 0; i < cEvent.chainIntChecks.Count; i++)
                {
                    height += 20;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("If ", GUILayout.Width(20));
                    cEvent.chainIntChecks[i].intNeeded = (ChainIntType)EditorGUILayout.EnumPopup(cEvent.chainIntChecks[i].intNeeded, GUILayout.Width(125));

                    EditorGUILayout.LabelField("", GUILayout.Width(1));
                    cEvent.chainIntChecks[i].equator = EditorGUILayout.Popup(cEvent.chainIntChecks[i].equator, new string[3] { "<", ">", "=" }, GUILayout.Width(30));
                    cEvent.chainIntChecks[i].value = EditorGUILayout.IntField(cEvent.chainIntChecks[i].value, GUILayout.Width(30));
                    EditorGUILayout.LabelField(" ", GUILayout.Width(10));
                    if (GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        cEvent.chainIntChecks.RemoveAt(i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        windowRect.height = height;
        cEvent.windowRect = windowRect;
    }

    public override void DrawCurves()
    {
        for (int i = 0; i < nextNodesForInput.Count; i++)
        {
            List<int> nonCheckIndexes = new List<int>();
            int rankFirstNonCheck = 10000000;
            foreach (Node node2 in nextNodesForInput[i])
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

            foreach (Node node2 in nextNodesForInput[i])
            {
                Rect tempRect = windowRect;
                tempRect.height = 107 + 40 * i;
                if (node2.checkNode)
                {
                    if ((node2.maxRank == 0 || node2.maxRank == 1) || !hasNonCheckNodeNext)
                    {
                        DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.yellow, true);
                    }
                    else
                    {
                        bool drewLine = false;
                        for (int j = 0; j < nonCheckIndexes.Count; j++)
                        {
                            if (node2.cEvent.rank > nonCheckIndexes[j])
                            {
                                Debug.Log(node2.GetType().ToString() + " can never be reached");
                                DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.red, true);
                                drewLine = true;
                                break;
                            }
                        }
                        if (!drewLine)
                        {
                            DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.cyan, true);
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
                                Debug.Log("Null trigger event ID " + node2.GetType().ToString());
                                DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.red, true);
                                triggerOK = false;
                                break;
                            }
                        }
                    }
                    if (node2.cEvent.rank == rankFirstNonCheck && triggerOK)
                    {
                        DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.green, true);
                    }
                    else
                    {
                        DrawNodeCurve(tempRect, node2.windowRect, 50, 50, Color.red, true);
                        Debug.Log(node2.GetType().ToString() + " can never be reached");
                    }
                }
            }
        }
    }

    public override void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        base.DrawNodeCurve(start, end, sTanMod, eTanMod, color, rightLeftConnect);
    }
}
