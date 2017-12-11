using UnityEditor;
using UnityEngine;

public class DialogueNode : Node
{
    private GUIStyle dialogueStyle;
    bool newSpeaker = false;
    public bool initialFlag = false;
    int speakerIndex = 0;
    string newSpeakerName = "";
    float width;

    private void OnEnable()
    {
        baseHeight = 167;
        baseWidth = 377;
        originalWindowTitle = "Dialogue";
    }

    public override void DrawWindow(DialogueChain chain)
    {
        base.DrawWindow(chain);
        HandleTitle();

        width = baseWidth;   
        cEvent.windowRect = windowRect;

        //If this is a new node
        if (!initialFlag)
        {
            if (!chain.speakers.Contains(chain.defaultSpeaker))
            {
                chain.speakers.Add(chain.defaultSpeaker);
            }

            if (chain.defaultSpeaker != "")
            {
                cEvent.speaker = chain.defaultSpeaker;
            }

            cEvent.noSpeaker = !chain.defaultShowNames;
            cEvent.showImage = chain.defaultShowImages;

            if (cEvent.showImage)
            {
                cEvent.speakerImage = chain.defaultSprite;
            }

            cEvent.dialogueContainer = chain.defaultContainerType;
            cEvent.textDelay = chain.defaultTextDelay;

            initialFlag = true;
        }

        //If character count is over the limit
        if (cEvent.dialogue != null && cEvent.dialogue.Length > DialogueChainPreferences.maxCharCount && DialogueChainPreferences.maxCharCount != 0)
        {
            GUI.color = Color.red;
        }

        //If there is an image for the speaker
        EditorGUILayout.BeginHorizontal();
        if (cEvent.showImage)
        {
            height += 80;
            //Place Image on left if leftSide is true
            if (cEvent.leftSide)
            {
                cEvent.speakerImage = (Sprite)EditorGUILayout.ObjectField(cEvent.speakerImage, typeof(Sprite), false, GUILayout.Width(100), GUILayout.Height(100));
            }
            EditorGUILayout.BeginVertical();
            //Flip Image, choose image side, and choose if image from variable
            cEvent.flipImage = EditorGUILayout.Toggle("Flip Image", cEvent.flipImage);
            if (cEvent.leftSide)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Left Screen", GUILayout.Width(130));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    cEvent.leftSide = !cEvent.leftSide;
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Right Screen", GUILayout.Width(130));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    cEvent.leftSide = !cEvent.leftSide;
                }
                EditorGUILayout.EndHorizontal();
            }

            cEvent.useCustomPlayerImage = EditorGUILayout.Toggle("Use variable for image?", cEvent.useCustomPlayerImage);

            if (cEvent.useCustomPlayerImage)
            {
                cEvent.speakerImage = null;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index for image list", GUILayout.Width(145));
                cEvent.playerImageIndex = EditorGUILayout.IntField(cEvent.playerImageIndex, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Delay", GUILayout.Width(145));
            cEvent.textDelay = EditorGUILayout.FloatField(cEvent.textDelay, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            //Place Image on right if leftSide is not true
            if (!cEvent.leftSide)
            {
                cEvent.speakerImage = (Sprite)EditorGUILayout.ObjectField(cEvent.speakerImage, typeof(Sprite), false, GUILayout.Width(100), GUILayout.Height(100));
            }
        }
        EditorGUILayout.EndHorizontal();

        //Dialogue Container
        HandleDialogueContainerChoice();

        //Handling the speaker
        if (!cEvent.noSpeaker)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speaker:", GUILayout.Width(60));
            if (!chain.speakers.Contains("Player"))
            {
                chain.speakers.Insert(0, "Player");
            }
            string[] speakersArray = chain.speakers.ToArray();
            GUI.SetNextControlName("Popup");
            if (cEvent.speaker != null)
            {
                speakerIndex = chain.speakers.IndexOf(cEvent.speaker);
            }
            speakerIndex = EditorGUILayout.Popup(speakerIndex, speakersArray);
            cEvent.speaker = chain.speakers[speakerIndex];

            //Adding a new name to the speaker list
            if (newSpeaker)
            {
                if (GUILayout.Button("Cancel", GUILayout.Width(55)))
                {
                    newSpeaker = false;
                    GUI.FocusControl("Popup");
                    newSpeakerName = "";
                }
            }
            else
            {
                if (GUILayout.Button("Add/Remove", GUILayout.Width(85)))
                {
                    newSpeaker = true;
                    GUI.FocusControl("Popup");
                }
            }

            if (newSpeaker)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                height += 20;

                newSpeakerName = EditorGUILayout.TextField(newSpeakerName, GUILayout.Width(290));
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    if (newSpeakerName == "")
                    {
                        Debug.Log("Must specify a name to add for the new speaker.");
                    }
                    else
                    {
                        chain.speakers.Insert(0, newSpeakerName);
                        cEvent.speaker = newSpeakerName;
                        newSpeaker = false;
                        speakerIndex = chain.speakers.Count - 1;
                        GUI.FocusControl("Popup");
                    }
                }

                //Removing a name from the list of speakers
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (newSpeakerName == "")
                    {
                        Debug.Log("Must specify a name to remove for the speaker.");
                    }
                    else
                    {
                        if (chain.speakers.Contains(newSpeakerName))
                        {
                            if (cEvent.speaker == newSpeakerName)
                            {
                                if (speakerIndex == 0)
                                {
                                    cEvent.speaker = chain.speakers[speakerIndex + 1];
                                }
                                else
                                {
                                    cEvent.speaker = chain.speakers[speakerIndex - 1];
                                }
                            }
                            chain.speakers.Remove(newSpeakerName);
                            newSpeaker = false;
                            height -= 20;
                            speakerIndex = chain.speakers.Count - 1;
                            GUI.FocusControl("Popup");
                        }
                        else
                        {
                            Debug.Log("Name does not appear in list of speakers.");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            height -= 20;
            cEvent.speaker = "Player";
        }
    
        dialogueStyle = GUI.skin.GetStyle("TextArea");
        dialogueStyle.wordWrap = true;
        cEvent.dialogue = EditorGUILayout.TextArea(cEvent.dialogue, dialogueStyle, GUILayout.Height(60));


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("No Name", GUILayout.Width(75));
        cEvent.noSpeaker = EditorGUILayout.Toggle(cEvent.noSpeaker, GUILayout.Width(15));
        EditorGUILayout.LabelField("No Image", GUILayout.Width(60));
        cEvent.showImage = !EditorGUILayout.Toggle(!cEvent.showImage, GUILayout.Width(15));
        EditorGUILayout.LabelField("     Wait", GUILayout.Width(50));
        cEvent.dialogueWaitTime = EditorGUILayout.FloatField(cEvent.dialogueWaitTime, GUILayout.Width(45));
        EditorGUILayout.LabelField(" Fade", GUILayout.Width(34));
        cEvent.dialoguefadeTime = EditorGUILayout.FloatField(cEvent.dialoguefadeTime, GUILayout.Width(45));
        EditorGUILayout.EndHorizontal();

        if (!cEvent.showImage)
        {
            if (cEvent.leftSide)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Left Side", GUILayout.Width(80));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    cEvent.leftSide = !cEvent.leftSide;
                }
                EditorGUILayout.LabelField("                    Text Delay", GUILayout.Width(145));
                cEvent.textDelay = EditorGUILayout.FloatField(cEvent.textDelay, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Right Side", GUILayout.Width(80));
                if (GUILayout.Button("Switch", GUILayout.Width(50)))
                {
                    cEvent.leftSide = !cEvent.leftSide;
                }
                EditorGUILayout.LabelField("                    Text Delay", GUILayout.Width(145));
                cEvent.textDelay = EditorGUILayout.FloatField(cEvent.textDelay, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }
        }

        windowRect.height = height;
        windowRect.width = width;
    }

    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void DrawNodeCurve(Rect start, Rect end, float sTanMod, float eTanMod, Color color, bool rightLeftConnect)
    {
        base.DrawNodeCurve(start, end, sTanMod, eTanMod, color, rightLeftConnect);
    }

    public virtual void HandleDialogueContainerChoice()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogue Box Container", GUILayout.Width(170));
        cEvent.dialogueContainer = (ContainerType)EditorGUILayout.EnumPopup(cEvent.dialogueContainer);
        EditorGUILayout.EndHorizontal();
    }    
}

