using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerNode : Node
{
    string newTrig = "";

    private void OnEnable()
    {
        baseHeight = 68;
        baseWidth = 300;
        originalWindowTitle = "Trigger Set";
    }

    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Trigger"))
        {
            cEvent.triggers.Add(null);
            cEvent.triggerBools.Add(true);
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < cEvent.triggers.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();         
            height += 20;

            EditorGUILayout.LabelField("Set", GUILayout.Width(25));            
            cEvent.triggers[i] = (ChainTrigger)EditorGUILayout.ObjectField(cEvent.triggers[i], typeof(ChainTrigger), false, GUILayout.Width(150));
            cEvent.triggerBools[i] = EditorGUILayout.Toggle(cEvent.triggerBools[i], GUILayout.Width(15));
            EditorGUILayout.LabelField("to " + ((cEvent.triggerBools[i]) ? "true" : "false"), GUILayout.Width(50));
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                cEvent.triggers.RemoveAt(i);
                cEvent.triggerBools.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New", GUILayout.Width(80)))
        {
            if (newTrig == "")
            {
                Debug.Log("Cannot add a trigger with no name");
            }
            else if (Resources.Load(DialogueChainPreferences.triggerAssetPathway + "/" + newTrig) != null)
            {
                Debug.Log("This trigger already exists");
                Resources.UnloadUnusedAssets();
            }
            else
            {
                if (cEvent.triggers[cEvent.triggers.Count - 1] == null)
                {
                    cEvent.triggers[cEvent.triggers.Count - 1] = CreateTrigger(newTrig);
                    cEvent.triggerBools[cEvent.triggerBools.Count - 1] = true;
                }
                else
                {
                    cEvent.triggers.Add(CreateTrigger(newTrig));
                    cEvent.triggerBools.Add(true);
                }
                newTrig = "";
            }
        }

        newTrig = EditorGUILayout.TextField(newTrig);
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

    public static ChainTrigger CreateTrigger(string newName)
    {
        ChainTrigger asset = CreateInstance<ChainTrigger>();

        string path = DialogueChainPreferences.triggerAssetPathway;
        string assetPathAndName = "";

        if (newName.Contains("/"))
        {
            //char[] separator = new char[] { '/' };
            string[] nameSplit = newName.Split('/');
            for (int i = 0; i < nameSplit.Length - 1; i++)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources/" + path + "/" + nameSplit[i]))
                {
                    AssetDatabase.CreateFolder("Assets/Resources/" + path, nameSplit[i]);
                }

                path += "/" + nameSplit[i];
            }

            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + path + "/" + nameSplit[nameSplit.Length - 1] + ".asset");
        }
        else
        {
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + path + "/" + newName + ".asset");
        }

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
}