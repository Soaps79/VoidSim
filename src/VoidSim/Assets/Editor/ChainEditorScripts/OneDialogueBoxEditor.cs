using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(OneDialogueBox))]
public class OneDialogueBoxEditor : Editor
{
    bool hideList = true;

    public override void OnInspectorGUI()
    {
        OneDialogueBox editor = (OneDialogueBox)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(editor), typeof(OneDialogueBox), false);
        GUI.enabled = true;

        editor.useInteractionCode = EditorGUILayout.Toggle(new GUIContent("Use Interactable", "If true this dialogue will show when the player interacts with the object. " +
            "If you want to use your own code to interact and start the dialogue, set this to false and call OneDialogueBoxEditor.ShowDialogue()"), editor.useInteractionCode);

        editor.haltMovement = EditorGUILayout.Toggle("Halt Movement", editor.haltMovement);
        editor.textDelay = EditorGUILayout.FloatField ("Text Delay", editor.textDelay);

        //If there is an image for the speaker
        if (editor.showImage)
        {
            EditorGUILayout.BeginHorizontal();
            //Place Image on left if leftSide is true
            if (editor.leftSide)
            {
                editor.speakerImage = (Sprite)EditorGUILayout.ObjectField(editor.speakerImage, typeof(Sprite), false, GUILayout.Width(100), GUILayout.Height(100));
            }
            EditorGUILayout.BeginVertical();
            //Flip Image, choose image side, and choose if image from variable
            editor.flipImage = EditorGUILayout.Toggle("Flip Image", editor.flipImage);
            if (editor.leftSide)
            {
                EditorGUILayout.LabelField("Left Screen");
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    editor.leftSide = !editor.leftSide;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Right Screen", GUILayout.Width(130));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    editor.leftSide = !editor.leftSide;
                }
                EditorGUILayout.EndHorizontal();
            }

            editor.useCustomPlayerImage = EditorGUILayout.Toggle("Use variable for image?", editor.useCustomPlayerImage);

            if (editor.useCustomPlayerImage)
            {
                editor.speakerImage = null;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index for image list", GUILayout.Width(EditorGUIUtility.labelWidth));
                editor.playerImageIndex = EditorGUILayout.IntField(editor.playerImageIndex, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            //Place Image on right if leftSide is not true
            if (!editor.leftSide)
            {
                editor.speakerImage = (Sprite)EditorGUILayout.ObjectField(editor.speakerImage, typeof(Sprite), false, GUILayout.Width(100), GUILayout.Height(100));
            }
            EditorGUILayout.EndHorizontal();
        }

        //Dialogue Container
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogue Box Container", GUILayout.Width(170));
        editor.dialogueContainer = (ContainerType)EditorGUILayout.EnumPopup(editor.dialogueContainer);
        EditorGUILayout.EndHorizontal();

        //Handling the speaker
        if (editor.showSpeakerName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speaker:", GUILayout.Width(60));
            string[] speakersArray = DialogueChainPreferences.defaultSpeakerList;
            if (speakersArray.Length > 0 && !hideList)
            {
                Debug.Log(speakersArray.Length);
                int speakerIndex = 0;
                GUI.SetNextControlName("Popup");
                if (editor.speaker != "" && editor.speaker != null)
                {
                    speakerIndex = System.Array.IndexOf(DialogueChainPreferences.defaultSpeakerList, editor.speaker);
                }
                speakerIndex = EditorGUILayout.Popup(speakerIndex, speakersArray);
                editor.speaker = DialogueChainPreferences.defaultSpeakerList[speakerIndex];

                if (GUILayout.Button("New"))
                {
                    editor.speaker = "";
                    hideList = true;
                }
            }
            else
            {
                editor.speaker = EditorGUILayout.TextField(editor.speaker);

                if (DialogueChainPreferences.defaultSpeakerList.Length > 0)
                {
                    if (GUILayout.Button("List"))
                    {
                        editor.speaker = DialogueChainPreferences.defaultSpeakerList[0];
                        hideList = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUIStyle dialogueStyle = GUI.skin.GetStyle("TextArea");
        dialogueStyle.wordWrap = true;
        editor.dialogue = EditorGUILayout.TextArea(editor.dialogue, dialogueStyle, GUILayout.Height(60));


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("No Name", GUILayout.Width(75));
        editor.showSpeakerName = !EditorGUILayout.Toggle(!editor.showSpeakerName, GUILayout.Width(15));
        EditorGUILayout.LabelField("No Image", GUILayout.Width(60));
        editor.showImage = !EditorGUILayout.Toggle(!editor.showImage, GUILayout.Width(15));

        EditorGUILayout.LabelField("      Box Image", GUILayout.Width(90));
        editor.boxSpriteType = (BoxImage)EditorGUILayout.EnumPopup(editor.boxSpriteType);
        EditorGUILayout.EndHorizontal();

        if (!editor.showImage)
        {
            if (editor.leftSide)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Left Screen", GUILayout.Width(80));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    editor.leftSide = !editor.leftSide;
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Right Screen", GUILayout.Width(80));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    editor.leftSide = !editor.leftSide;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorUtility.SetDirty(editor);
    }
}
