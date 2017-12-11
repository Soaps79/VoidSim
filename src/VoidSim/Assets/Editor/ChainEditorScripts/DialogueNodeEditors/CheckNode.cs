using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CheckNode : Node
{
    string newTrig = "";

    private void OnEnable()
    {
        baseHeight = 47;
        baseWidth = 300;
        originalWindowTitle = "Check";
    }
    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        //Initial buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Trigger Check"))
        {
            cEvent.triggerChecks.Add(null);
            cEvent.triggerCheckBools.Add(true);
        }
        if (GUILayout.Button("Item Check"))
        {
            cEvent.itemChecks.Add(null);
            cEvent.itemChecksString.Add(null);
        }
        if (GUILayout.Button("Integer Check"))
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

                    //Change ChainIntType to reflect your integers that you want to use
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
        base.DrawCurves();
    }

    public override void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        base.DrawNodeCurve(start, end, sTanMod, eTanMod, color, rightLeftConnect);
    }
}
