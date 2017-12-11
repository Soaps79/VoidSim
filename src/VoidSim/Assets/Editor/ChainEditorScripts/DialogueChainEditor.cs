using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueChain))]
public class DialogueChainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueChain chain = (DialogueChain)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(chain), typeof(ScriptableObject), false);
        GUI.enabled = true;

        chain.haltMovement = EditorGUILayout.Toggle("Halt Movement", chain.haltMovement);
        chain.defaultShowImages = EditorGUILayout.Toggle("Default Show Portraits", chain.defaultShowImages);
        chain.defaultShowNames = EditorGUILayout.Toggle("Default Show Names", chain.defaultShowNames);

        if (chain.defaultShowImages)
        {
            chain.defaultSprite = (Sprite)EditorGUILayout.ObjectField("Default Portrait", chain.defaultSprite, typeof(Sprite), false);
        }

        if (chain.defaultShowNames)
        {
            chain.defaultSpeaker = EditorGUILayout.TextField("Default Speaker Name", chain.defaultSpeaker);
        }

        chain.defaultContainerType = (ContainerType)EditorGUILayout.EnumPopup("Default Container", chain.defaultContainerType);
        chain.defaultTextDelay = EditorGUILayout.FloatField("Default Text Delay", chain.defaultTextDelay);

        if (GUILayout.Button("Open Node Editor"))
        {
            DialogueChainEditorWindow editor = EditorWindow.GetWindow<DialogueChainEditorWindow>();
            editor.dialogueChain = chain;

            editor.OnDestroy();
            editor.LoadCurrentQuest();
        }

        EditorUtility.SetDirty(chain);
    }
}





