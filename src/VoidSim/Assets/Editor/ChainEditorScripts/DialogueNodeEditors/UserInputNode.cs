using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UserInputNode : DialogueNode
{
    private void OnEnable()
    {
        baseHeight = 185;
        originalWindowTitle = "Input";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);

        for (int i = 0; i < nextNodesForInput.Count; i++)
        {
            if (nextNodesForInput[i].Count == 0)
            {
                windowTitle = "Input (Missing Input Connection)";
                break;
            }
            else
            {
                if (previousNodes.Count > 0)
                {
                    windowTitle = "Input";
                }
                else
                {
                    windowTitle = "Input (Needs Previous)";
                }           
            }
        }
        

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
                tempRect.height = height * 2 - 30 - 40 * (cEvent.inputButtons.Count - 1 - i);            
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


        foreach (Node node2 in lateralNodes)
        {
            DrawNodeCurve(windowRect, node2.windowRect, 50, 50, Color.white, false);
        }
    }

    public override void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        
        base.DrawNodeCurve(start, end, sTanMod, eTanMod, color, rightLeftConnect);
    }

    public override void HandleDialogueContainerChoice()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogue Box Container", GUILayout.Width(170));

        string[] containerNames = System.Enum.GetNames(typeof(ContainerType));
        List<ContainerType> inputContainers = new List<ContainerType>();
        for (int i = 0; i < containerNames.Length; i++)
        {
            if (containerNames[i].Contains("input") || containerNames[i].Contains("Input"))
            {
                inputContainers.Add((ContainerType)i);
            }
        }
        string[] inputNames = new string[inputContainers.Count];
        for (int i = 0; i < inputContainers.Count; i++)
        {
            inputNames[i] = inputContainers[i].ToString();
        }

        if (!inputContainers.Contains(cEvent.dialogueContainer))
        {
            Debug.Log(cEvent.dialogueContainer);
            cEvent.dialogueContainer = inputContainers[0];
        }

        cEvent.dialogueContainer = (ContainerType)ArrayUtility.IndexOf(containerNames, inputNames[EditorGUILayout.Popup(ArrayUtility.IndexOf(inputNames, cEvent.dialogueContainer.ToString()), inputNames)]);

        EditorGUILayout.EndHorizontal();
    }
}