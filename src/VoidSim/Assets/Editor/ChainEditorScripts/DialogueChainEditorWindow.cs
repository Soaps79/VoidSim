using System.Collections;
using System.Collections.Generic;
using Assets.Editor.ChainEditorScripts.DialogueNodeEditors;
using UnityEngine;
using UnityEditor;

public class DialogueChainEditorWindow : EditorWindow
{
    #region Declarations
    public DialogueChain dialogueChain;
    public List<Node> nodes = new List<Node>();

    public Node startingNode;

    private bool connectingNodes;
    public bool connectingInputNode;
    private bool connectingNodesLaterally;
    private bool deletingConnection;
    private bool deletingLatConnection;
    public bool deletingInputConnection;

    private Node selectedNode;
    public int selectedInputNodeIndex;

    private bool initiatedPlay;
    private Vector2 mousePos;
    private Vector2 mousePosForNewNode;
    private Event e;
    #endregion

    #region DrawingTheNodes
    private void OnGUI()
    {
        e = Event.current;

        mousePos = e.mousePosition;
    
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), Resources.Load("ChainResources/Textures/Grey") as Texture, ScaleMode.StretchToFill);

        //Reloads everything when playmode starts and ends
        if (Application.isPlaying && !initiatedPlay)
        {
            initiatedPlay = true;
            OnDestroy();
            LoadCurrentQuest();
        }
        if (!Application.isPlaying && initiatedPlay)
        {
            initiatedPlay = false;
            OnDestroy();
            LoadCurrentQuest();
        }


        if (e.type == EventType.mouseDown)
        {
            selectedNode = GetSelectedNode();

            if (e.button == 1 && selectedNode == null)
            {
                RightClickEmpty();
            }
            else if (e.button == 1)
            {
                RightClickNode();
            }
        }

        if (e.button == 0)
        {
            PanWindow();
        }

        StartDrawingCurves();

        BeginWindows();

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].windowRect.size == Vector2.zero)
            {
                Rect tempRect = nodes[i].windowRect;
                tempRect.width = 500;
                tempRect.height = 500;
            }
            nodes[i].windowRect = GUI.Window(i, nodes[i].windowRect, DrawNodeWindow, nodes[i].windowTitle);
        }

        EndWindows();

        if (connectingNodes)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            DrawNodeCurve(selectedNode.windowRect, mouseRect, Color.black, true);
        }
        else if (connectingNodesLaterally)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            DrawNodeCurve(selectedNode.windowRect, mouseRect, Color.black, false);
        }
        else if (connectingInputNode)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            Rect tempRect = selectedNode.windowRect;

            if (selectedNode is UserInputNode)
            {
                tempRect.height = selectedNode.height * 2 - 20 - 40 * (selectedNode.cEvent.inputButtons.Count - 1 - selectedInputNodeIndex);
            }
            else
            {
                tempRect.height = 107 + 40 * selectedInputNodeIndex;
            }

            DrawNodeCurve(tempRect, mouseRect, Color.black, true);
        }

        Repaint();
        EditorUtility.SetDirty(dialogueChain);
    }

    public void DrawNodeWindow(int id)
    {
        try
        {
            nodes[id].DrawWindow(dialogueChain);
            GUI.DragWindow();
        }
        catch (System.ArgumentException)
        {
            //Getting control 0's position in a group with only 0 controls when doing MouseDown. -- Suspected: Trying to draw window before event.layout.
        }
    }

    public void StartDrawingCurves()
    {
        foreach (Node node in nodes)
        {
            node.DrawCurves();
        }
    }

    public void DrawNodeCurve(Rect start, Rect end, Color color, bool rightLeftConnect)
    {
        Vector3 startPos;
        Vector3 endPos;
        Vector3 startTan;
        Vector3 endTan;

        if (rightLeftConnect)
        {
            startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            endPos = new Vector3(end.x, end.y + end.height / 2, 0);

            startTan = startPos + Vector3.right * 50;
            endTan = endPos + Vector3.left * 50;
        }
        else
        {
            startPos = new Vector3(start.x + start.width, start.y + start.height, 0);
            endPos = new Vector3(end.x, end.y, 0);

            startTan = startPos + Vector3.up * 50;
            endTan = endPos + Vector3.down * 50;

        }


        Color shadowCol = new Color(color.r, color.g, color.b, 0.06f);

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 1);
    }

    public void PanWindow()
    {
        if (e.type == EventType.MouseDrag && selectedNode == null)
        {
            foreach (Node node in nodes)
            {
                Rect winRect = node.windowRect;
                winRect.x += e.delta.x;
                winRect.y += e.delta.y;
                node.windowRect = winRect;
            }
        }
        else if (e.type == EventType.ScrollWheel)
        {
            foreach (Node node in nodes)
            {
                Rect winRect = node.windowRect;
                winRect.x -= e.delta.y * 10;
                node.windowRect = winRect;
            }
        }
    }

    void ResetConnectionBools()
    {
        connectingNodes = false;
        deletingConnection = false;
        connectingNodesLaterally = false;
        connectingInputNode = false;
        deletingInputConnection = false;
        selectedInputNodeIndex = 0;
    }
    #endregion

    #region LoadCurrentQuest
    public void LoadCurrentQuest()
    {
        titleContent = new GUIContent(dialogueChain.name);

        foreach (ChainEvent qEvent in dialogueChain.chainEvents)
        {
            if (qEvent.cEventType == ChainEventType.Dialogue)
            {
                DialogueNode newNode = CreateInstance("DialogueNode") as DialogueNode;
                AssignEventToNode(newNode, qEvent);
                newNode.initialFlag = true;
            }
            else if (qEvent.cEventType == ChainEventType.SetTrigger)
            {
                TriggerNode newNode = CreateInstance("TriggerNode") as TriggerNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.UserInput)
            {
                UserInputNode newNode = CreateInstance("UserInputNode") as UserInputNode;
                AssignEventToNode(newNode, qEvent);
                newNode.initialFlag = true;
            }
            else if (qEvent.cEventType == ChainEventType.ItemManagement)
            {
                ItemNode newNode = CreateInstance("ItemNode") as ItemNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.Mission)
            {
                var newNode = CreateInstance("MissionNode") as MissionNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.Pause)
            {
                PauseNode newNode = CreateInstance("PauseNode") as PauseNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.SubDialogue)
            {
                SubDialogueNode newNode = CreateInstance("SubDialogueNode") as SubDialogueNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.Audio)
            {
                AudioNode newNode = CreateInstance("AudioNode") as AudioNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.IntAdjustment)
            {
                IntAdjustmentNode newNode = CreateInstance("IntAdjustmentNode") as IntAdjustmentNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.Start)
            {
                StartingNode newNode = CreateInstance("StartingNode") as StartingNode;
                AssignEventToNode(newNode, qEvent);
            }
            else if (qEvent.cEventType == ChainEventType.Check)
            {
                CheckNode newNode = CreateInstance("CheckNode") as CheckNode;
                AssignEventToNode(newNode, qEvent);
                newNode.checkNode = true;
            }
            else if (qEvent.cEventType == ChainEventType.SecondaryInput)
            {
                SecondaryInputNode newNode = CreateInstance("SecondaryInputNode") as SecondaryInputNode;
                AssignEventToNode(newNode, qEvent);
                newNode.checkNode = true;
            }
            else if (qEvent.cEventType == ChainEventType.Message)
            {
                MessageNode newNode = CreateInstance("MessageNode") as MessageNode;
                AssignEventToNode(newNode, qEvent);
            }
        }

        AssignConnectingNodes();
    }

    void AssignEventToNode(Node newNode, ChainEvent qEvent)
    {
        newNode.cEvent = qEvent;
        newNode.windowRect = newNode.cEvent.windowRect;
        nodes.Add(newNode);
        newNode.setRank = true;

        if (qEvent.cEventType == ChainEventType.Start)
        {
            startingNode = newNode;
            dialogueChain.startEvent = qEvent;
        }
    }

    void AssignConnectingNodes()
    {
        foreach (Node node in nodes)
        {
            //Next Nodes
            if (!(node is UserInputNode) && !(node is SecondaryInputNode))
            {
                for (int i = 0; i < node.cEvent.nextEventIDs.Count; i++)
                {
                    foreach (Node node2 in nodes)
                    {
                        if (node2.cEvent.eventID == node.cEvent.nextEventIDs[i])
                        {
                            node.nextNodes.Add(node2);
                        }
                    }
                }
            }
            else
            {
                for (int b = 0; b < node.cEvent.inputButtons.Count; b++)
                {
                    node.nextNodesForInput.Add(new List<Node>());
                    for (int i = 0; i < node.cEvent.inputButtons[b].nextEventIDsForInputs.Count; i++)
                    {
                        foreach (Node node2 in nodes)
                        {
                            if (node2.cEvent.eventID == node.cEvent.inputButtons[b].nextEventIDsForInputs[i])
                            {
                                node.nextNodesForInput[b].Add(node2);
                            }
                        }
                    }
                }
            }
            //Previous Nodes
            for (int i = 0; i < node.cEvent.previousEventIDs.Count; i++)
            {
                foreach (Node node2 in nodes)
                {
                    if (node2.cEvent.eventID == node.cEvent.previousEventIDs[i])
                    {
                        node.previousNodes.Add(node2);
                    }
                }
            }
            //Lateral Nodes
            for (int i = 0; i < node.cEvent.lateralConnections.Count; i++)
            {
                foreach (Node node2 in nodes)
                {
                    if (node2.cEvent.eventID == node.cEvent.lateralConnections[i])
                    {
                        node.lateralNodes.Add(node2);
                    }
                }
            }
        }
    }
    #endregion

    #region RightClick
    private void RightClickEmpty()
    {
        mousePosForNewNode = mousePos;

        GenericMenu menu = new GenericMenu();

        ResetConnectionBools();

        if (startingNode == null)
        {
            menu.AddItem(new GUIContent("Make Starting Node"), false, CallBack, "startNode");
        }
        else
        {
            menu.AddItem(new GUIContent("Dialogue/Dialogue"), false, CallBack, "dialogueNode");
            menu.AddItem(new GUIContent("Dialogue/User Input"), false, CallBack, "userInput");
            menu.AddItem(new GUIContent("Dialogue/Secondary Input"), false, CallBack, "secondInput");
            
            menu.AddItem(new GUIContent("Set/Set Trigger"), false, CallBack, "setTrigger");
            menu.AddItem(new GUIContent("Set/Integer Adjustment"), false, CallBack, "iAdjust");

            menu.AddItem(new GUIContent("Scene/Pause Dialogue"), false, CallBack, "pause");
            menu.AddItem(new GUIContent("Scene/Sub Dialogue"), false, CallBack, "subQuest");
            menu.AddItem(new GUIContent("Scene/Send Holder Message"), false, CallBack, "message");

            menu.AddItem(new GUIContent("Mission"), false, CallBack, "mission");
            menu.AddItem(new GUIContent("Item-" + DialogueChainPreferences.experienceString), false, CallBack, "item");

            menu.AddItem(new GUIContent("Audio"), false, CallBack, "audio");

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Add Check Node"), false, CallBack, "check");
        }

        menu.ShowAsContext();
        e.Use();
    }

    private void RightClickNode()
    {
        GenericMenu menu = new GenericMenu();

        if (!(selectedNode is UserInputNode || selectedNode is SecondaryInputNode))
        {
            menu.AddItem(new GUIContent("Make Connection"), false, CallBack, "makeConnection");
            menu.AddItem(new GUIContent("Delete Connection"), false, CallBack, "deleteConnection");
            menu.AddSeparator("");
        }
        else
        {
            if (selectedNode is UserInputNode)
            {
                menu.AddItem(new GUIContent("Make Secondary Connection"), false, CallBack, "makeLatConnection");
                if (selectedNode.lateralNodes.Count > 0)
                {
                    menu.AddItem(new GUIContent("Delete Secondary Connection"), false, CallBack, "deleteLatConnection");
                }
            }
        }

        /*if (startingNode != selectedNode)
        /{
            if (!(selectedNode is PauseNode) && selectedNode.dEvent.dEventType != DialogueEventType.TriggerCheck)
            {
                menu.AddItem(new GUIContent("Make this the starting node"), false, CallBack, "startNode");
                menu.AddSeparator("");
            }
        }*/

        menu.AddItem(new GUIContent("Delete Node"), false, CallBack, "delete");

        menu.ShowAsContext();
        e.Use();
    }
    #endregion

    #region LeftClickNode
    private Node GetSelectedNode()
    {
        //If Left Click
        if (e.button == 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                //If clicked on window
                if (nodes[i].windowRect.Contains(mousePos))
                {
                    //If a previous node is selected
                    if (selectedNode != null)
                    {
                        if (connectingNodes && (selectedNode.lateralNodes.Contains(nodes[i]) || nodes[i].lateralNodes.Contains(selectedNode)))
                        {
                            Debug.Log("Can't have lateral and regularly connected nodes.");
                        }
                        else if (connectingNodes)
                        {
                            //If you're drawing a connection and the nodes do not already connect
                            if (!selectedNode.nextNodes.Contains(nodes[i]) && !nodes[i].nextNodes.Contains(selectedNode) && selectedNode != nodes[i])
                            {
                                if (!(nodes[i] is SecondaryInputNode))
                                {
                                    selectedNode.cEvent.nextEventIDs.Add(nodes[i].cEvent.eventID);
                                    nodes[i].cEvent.previousEventIDs.Add(selectedNode.cEvent.eventID);
                                    selectedNode.nextNodes.Add(nodes[i]);
                                    nodes[i].previousNodes.Add(selectedNode);
                                    connectingNodes = false;
                                    return null;
                                }
                                else
                                {
                                    connectingNodes = false;
                                    return null;
                                }
                            }
                            //If you're drawing a connection and the nodes already connect
                            else if ((selectedNode.nextNodes.Contains(nodes[i]) || nodes[i].nextNodes.Contains(selectedNode)) || selectedNode == nodes[i])
                            {
                                return selectedNode;
                            }
                        }
                        //If you're drawing a LATERAL connection and the nodes do not already connect
                        else if (connectingNodesLaterally && !selectedNode.lateralNodes.Contains(nodes[i]) && selectedNode != nodes[i] && nodes[i].cEvent.cEventType == ChainEventType.SecondaryInput)
                        {
                            selectedNode.cEvent.lateralConnections.Add(nodes[i].cEvent.eventID);
                            selectedNode.lateralNodes.Add(nodes[i]);
                            nodes[i].cEvent.lateralConnections.Add(selectedNode.cEvent.eventID);
                            nodes[i].lateralNodes.Add(selectedNode);
                            connectingNodesLaterally = false;
                            return null;
                        }
                        //If you're drawing a LATERAL connection and the nodes already connect
                        else if (connectingNodesLaterally && (selectedNode.lateralNodes.Contains(nodes[i]) || selectedNode != nodes[i]))
                        {
                            return selectedNode;
                        }
                        //If you're deleting a connection and the nodes connect
                        else if (deletingConnection)
                        {
                            if (selectedNode.nextNodes.Contains(nodes[i]))
                            {
                                selectedNode.cEvent.nextEventIDs.Remove(nodes[i].cEvent.eventID);
                                nodes[i].cEvent.previousEventIDs.Remove(selectedNode.cEvent.eventID);
                                selectedNode.nextNodes.Remove(nodes[i]);
                                nodes[i].previousNodes.Remove(selectedNode);
                                deletingConnection = false;
                                return null;
                            }
                            else if (selectedNode.previousNodes.Contains(nodes[i]))
                            {
                                if (!(nodes[i] is UserInputNode))
                                {
                                    nodes[i].cEvent.nextEventIDs.Remove(selectedNode.cEvent.eventID);
                                    selectedNode.cEvent.previousEventIDs.Remove(nodes[i].cEvent.eventID);
                                    nodes[i].nextNodes.Remove(selectedNode);
                                    selectedNode.previousNodes.Remove(nodes[i]);
                                    deletingConnection = false;
                                    return null;
                                }
                            }
                            else
                            {
                                return selectedNode;
                            }
                        }
                        //If you're deleting a LATERAL connection and the nodes connect
                        else if (deletingLatConnection)
                        {
                            if (selectedNode.lateralNodes.Contains(nodes[i]) || nodes[i].lateralNodes.Contains(selectedNode))
                            {
                                selectedNode.cEvent.lateralConnections.Remove(nodes[i].cEvent.eventID);
                                nodes[i].cEvent.lateralConnections.Remove(selectedNode.cEvent.eventID);
                                nodes[i].lateralNodes.Remove(selectedNode);
                                selectedNode.lateralNodes.Remove(nodes[i]);
                                deletingLatConnection = false;
                                return null;
                            }
                            else
                            {
                                return selectedNode;
                            }
                        }
                        //If you're deleting an input curve
                        else if (deletingInputConnection)
                        {
                            if (selectedNode.cEvent.cEventType != ChainEventType.SecondaryInput)
                            {
                                UserInputNode iNode = (UserInputNode)selectedNode;
                                if (iNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i]))
                                {
                                    iNode.cEvent.inputButtons[selectedInputNodeIndex].nextEventIDsForInputs.Remove(nodes[i].cEvent.eventID);
                                    nodes[i].cEvent.previousEventIDs.Remove(selectedNode.cEvent.eventID);
                                    selectedNode.nextNodesForInput[selectedInputNodeIndex].Remove(nodes[i]);
                                    nodes[i].previousNodes.Remove(selectedNode);
                                    deletingInputConnection = false;
                                    return null;
                                }
                            }
                            else
                            {
                                SecondaryInputNode sNode = (SecondaryInputNode)selectedNode;
                                if (sNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i]))
                                {
                                    sNode.cEvent.inputButtons[selectedInputNodeIndex].nextEventIDsForInputs.Remove(nodes[i].cEvent.eventID);
                                    nodes[i].cEvent.previousEventIDs.Remove(selectedNode.cEvent.eventID);
                                    selectedNode.nextNodesForInput[selectedInputNodeIndex].Remove(nodes[i]);
                                    nodes[i].previousNodes.Remove(selectedNode);
                                    deletingInputConnection = false;
                                    return null;
                                }
                            }
                        }
                        //If you're connecting an input node
                        else if (connectingInputNode)
                        {
                            //If the selected node is a user input node
                            if (selectedNode.cEvent.cEventType != ChainEventType.SecondaryInput)
                            {
                                //If you're drawing a connection and the nodes do not already connect
                                UserInputNode iNode = (UserInputNode)selectedNode;
                                if (iNode.nextNodesForInput[selectedInputNodeIndex].Count == 0 || (iNode.nextNodesForInput[selectedInputNodeIndex].Count > 0 && !iNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i])))
                                {
                                    iNode.cEvent.inputButtons[selectedInputNodeIndex].nextEventIDsForInputs.Add(nodes[i].cEvent.eventID);
                                    nodes[i].cEvent.previousEventIDs.Add(iNode.cEvent.eventID);
                                    iNode.nextNodesForInput[selectedInputNodeIndex].Add(nodes[i]);
                                    nodes[i].previousNodes.Add(iNode);
                                    connectingInputNode = false;
                                    return null;
                                }
                                //If you're drawing a connection and the nodes already connect
                                else if (iNode.nextNodesForInput[selectedInputNodeIndex].Count > 0 && iNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i]))
                                {
                                    return iNode;
                                }
                            }
                            //if the selected node is a secondary input
                            else
                            {
                                //If you're drawing a connection and the nodes do not already connect
                                SecondaryInputNode sNode = (SecondaryInputNode)selectedNode;
                                if (sNode.nextNodesForInput[selectedInputNodeIndex].Count == 0 || (sNode.nextNodesForInput[selectedInputNodeIndex].Count > 0 && !sNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i])))
                                {
                                    Debug.Log("Connecting");
                                    sNode.cEvent.inputButtons[selectedInputNodeIndex].nextEventIDsForInputs.Add(nodes[i].cEvent.eventID);
                                    nodes[i].cEvent.previousEventIDs.Add(sNode.cEvent.eventID);
                                    sNode.nextNodesForInput[selectedInputNodeIndex].Add(nodes[i]);
                                    nodes[i].previousNodes.Add(sNode);
                                    connectingInputNode = false;
                                    return null;
                                }
                                //If you're drawing a connection and the nodes already connect
                                else if (sNode.nextNodesForInput[selectedInputNodeIndex].Count > 0 && sNode.nextNodesForInput[selectedInputNodeIndex].Contains(nodes[i]))
                                {
                                    return sNode;
                                }
                            }
                        }
                        return nodes[i];
                    }
                    else
                    {
                        return nodes[i];
                    }
                }
            }
            //If no nodes are clicked on, but there is a left click
            ResetConnectionBools();
            return null;
        }
        //If Right Click
        if (e.button == 1)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].windowRect.Contains(mousePos))
                {
                    return nodes[i];
                }
            }
            return null;
        }

        return selectedNode;
    }
    #endregion

    #region DeleteNode
    private void DeleteNode(Node node)
    {
        //Remove from connected nodes     
        foreach (Node node2 in node.nextNodes)
        {
            node2.previousNodes.Remove(node);
            node2.cEvent.previousEventIDs.Remove(node.cEvent.eventID);
        }
        foreach (Node node2 in node.previousNodes)
        {
            if (node2 is UserInputNode || node2 is SecondaryInputNode)
            {
                for (int i = 0; i < node2.nextNodesForInput.Count; i++)
                {
                    node2.nextNodesForInput[i].Remove(node);
                    node2.cEvent.inputButtons[i].nextEventIDsForInputs.Remove(node.cEvent.eventID);
                }
            }
            else
            {
                node2.nextNodes.Remove(node);
                node2.cEvent.nextEventIDs.Remove(node.cEvent.eventID);
            }
        }
        foreach (Node node2 in node.lateralNodes)
        {
            node2.lateralNodes.Remove(node);
        }
        foreach (Node node2 in nodes)
        {
            node2.lateralNodes.Remove(node);
            node2.cEvent.lateralConnections.Remove(node.cEvent.eventID);
        }

        dialogueChain.chainEvents.Remove(node.cEvent);
        
        //Remove starting event
        if (startingNode == node)
        {
            Debug.Log("No starting event!");
            dialogueChain.startEvent = null;
            startingNode = null;
        }

        nodes.Remove(node);

        if (node == selectedNode)
        {
            selectedNode = null;
        }

        DestroyImmediate(node);
    }
    #endregion

    #region MenuCallBacks
    public void CallBack(object obj)
    {
        string callBackString = obj.ToString();

        //New Nodes
        if (callBackString == "check")
        {
            CheckNode newNode = CreateInstance("CheckNode") as CheckNode;
            MakeNewNode(newNode, ChainEventType.Check);
            newNode.checkNode = true;
        }
        else if(callBackString == "dialogueNode")
        {
            DialogueNode newNode = CreateInstance("DialogueNode") as DialogueNode;
            MakeNewNode(newNode, ChainEventType.Dialogue);
        }
        else if (callBackString == "userInput")
        {
            UserInputNode newNode = CreateInstance("UserInputNode") as UserInputNode;
            MakeNewNode(newNode, ChainEventType.UserInput);
        }     
        else if (callBackString == "setTrigger")
        {
            TriggerNode newNode = CreateInstance("TriggerNode") as TriggerNode;
            MakeNewNode(newNode, ChainEventType.SetTrigger);
        }
        else if (callBackString == "pause")
        {
            PauseNode newNode = CreateInstance("PauseNode") as PauseNode;
            MakeNewNode(newNode, ChainEventType.Pause);
        }
        else if (callBackString == "item")
        {
            ItemNode newNode = CreateInstance("ItemNode") as ItemNode;
            MakeNewNode(newNode, ChainEventType.ItemManagement);
        }
        else if (callBackString == "mission")
        {
            var newNode = CreateInstance("MissionNode") as MissionNode;
            MakeNewNode(newNode, ChainEventType.Mission);
        }
        else if (callBackString == "subQuest")
        {
            SubDialogueNode newNode = CreateInstance("SubDialogueNode") as SubDialogueNode;
            MakeNewNode(newNode, ChainEventType.SubDialogue);
        }
        else if (callBackString == "audio")
        {
            AudioNode newNode = CreateInstance("AudioNode") as AudioNode;
            MakeNewNode(newNode, ChainEventType.Audio);
        }      
        else if (callBackString == "iAdjust")
        {
            IntAdjustmentNode newNode = CreateInstance("IntAdjustmentNode") as IntAdjustmentNode;
            MakeNewNode(newNode, ChainEventType.IntAdjustment);
        }
        else if (callBackString == "message")
        {
            MessageNode newNode = CreateInstance("MessageNode") as MessageNode;
            MakeNewNode(newNode, ChainEventType.Message);
        }
        else if (callBackString == "secondInput")
        {
            SecondaryInputNode newNode = CreateInstance("SecondaryInputNode") as SecondaryInputNode;
            MakeNewNode(newNode, ChainEventType.SecondaryInput);
        }
        else if (callBackString == "startNode")
        {
            StartingNode newNode = CreateInstance("StartingNode") as StartingNode;
            MakeNewNode(newNode, ChainEventType.Start);

            startingNode = newNode;
            dialogueChain.startEvent = newNode.cEvent;
        }

        //RightClicks
        else if (callBackString == "delete")
        {
            DeleteNode(selectedNode);
        }
        else if (callBackString == "makeConnection")
        {
            connectingNodes = true;
        }
        else if (callBackString == "makeLatConnection")
        {
            connectingNodesLaterally = true;
        }
        else if (callBackString == "deleteConnection") 
        {
            deletingConnection = true;
        }
        else if (callBackString == "deleteLatConnection")
        {
            deletingLatConnection = true;
        }
    }

    void MakeNewNode(Node newNode, ChainEventType eventType)
    {
        dialogueChain.chainEvents.Add(new ChainEvent());
        dialogueChain.chainEvents[dialogueChain.chainEvents.Count - 1].cEventType = eventType;
        dialogueChain.chainEvents[dialogueChain.chainEvents.Count - 1].eventID = dialogueChain.nodeIDCount;
        dialogueChain.nodeIDCount++;
        newNode.cEvent = dialogueChain.chainEvents[dialogueChain.chainEvents.Count - 1];

        newNode.windowRect = new Rect((int)mousePosForNewNode.x, (int)(mousePosForNewNode.y - newNode.baseHeight * 0.25f), newNode.baseWidth, newNode.height);
        dialogueChain.chainEvents[dialogueChain.chainEvents.Count - 1].windowRect = newNode.windowRect; //new Rect(mousePos.x - 100, mousePos.y - 30, newNode.baseWidth, newNode.height);
        nodes.Add(newNode);
    }
    #endregion

    #region OnDestroy
    public void OnDestroy()
    {
        nodes.Clear();
        selectedNode = null;
        startingNode = null;
    }
    #endregion    
}

