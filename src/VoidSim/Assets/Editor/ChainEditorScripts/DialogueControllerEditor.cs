using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(DialogueController))]
public class DialogueControllerEditor : Editor
{
    #region Declarations
    Canvas loadedCanvas;
    GameObject loadedContainer;
    DialogueContainer editingContainer = null;

    bool editing = false;
    ContainerType cType;

  
    //Editing Container Variables
    float containerWidth = 550;
    Vector2 containerPos = new Vector2(0, -220);
    bool seeContainerWidth = false;
    BoxImage dialogueImage = 0;

    bool lockImageAspectRatio = true;
    float imageWidth = 100;
    float imageHeight = 100;
    Vector2 imagePos = Vector2.zero;
    float imageOpacity = 1;
    Sprite testImage;

    Vector2 namePos = Vector2.zero;
    int speakerNameLeftPadding = 10;
    int speakerNameRightPadding = 10;
    int speakerNameTopPadding = 5;
    int speakerNameBottomPadding = 5;
    Color speakerNameFontColor = Color.black;
    Font speakerNameFont;
    int speakerNameFontSize = 19;
    string testSpeaker = "Speaker";
    float speakerOpacity = 1;

    float dialogueOpacity = 1;
    int dialogueLeftPadding = 10;
    int dialogueRightPadding = 10;
    int dialogueTopPadding = 5;
    int dialogueBottomPadding = 5;
    float dialogueBoxMinimumWidth = 250;
    Color dialogueFontColor = Color.black;
    Font dialogueFont;
    int dialogueFontSize = 14;
    string testDialogue = "Dialogue Box";

    int inputIndent = 20;
    int inputInitialSpacing = 5; //Spacing from Dialogue
    float inputOverallSpacing = 2; //Spacing from other inputs
    Font inputFont;
    int inputFontSize = 14;
    Color inputNormalColor = Color.black;
    Color inputHighlightColor = Color.yellow;
    List<GameObject> dButtons = new List<GameObject>();

    List<UndoStateDialogueController> undoStates = new List<UndoStateDialogueController>();
    int undoIndex = -1;
    int undoMax = 20;
    bool draggingMouse = false;
    bool guiChanged = false;

    bool seeTestValues = false;
    bool initialFlag = false;
    bool tempLockRatio = false;

    bool showContainers = false;
    bool showBoxImages = false;
    bool showTriggers = false;

    DialogueController dController;
    #endregion

    #region OnInspectorGUI
    public override void OnInspectorGUI()
    {
        dController = (DialogueController)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((DialogueController)target), typeof(DialogueController), false);
        GUI.enabled = true;

        dController.dialogueCanvas = (Canvas)EditorGUILayout.ObjectField("Canvas Prefab", dController.dialogueCanvas, typeof(Canvas), false);
        dController.ViewModelPrefab = (ConversationViewModel)EditorGUILayout.ObjectField("Convo Prefab", dController.ViewModelPrefab, typeof(ConversationViewModel), false);
        dController.ButtonPrefab = (Button)EditorGUILayout.ObjectField("Button Prefab", dController.ButtonPrefab, typeof(Button), false);

        dController.chainDataReset = EditorGUILayout.Toggle(new GUIContent("Reset data after Playmode", "If true, triggers will return to their original " +
            "state when exiting playmode. If false, triggers will retain their playmode altered state like other assets."), dController.chainDataReset);

        //Fold out for triggers
        showTriggers = EditorGUILayout.Foldout(showTriggers, "Trigger Assets");
        if (showTriggers)
        {
            //Full path to chain triggers
            string sAssetFolderPath = "Assets/Resources/" + DialogueChainPreferences.triggerAssetPathway;
            // Construct the system path of the asset folder 
            string sDataPath = Application.dataPath;
            string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + sAssetFolderPath;
            // get the system file paths of all the files in the asset folder
            string[] aFilePaths = Directory.GetFiles(sFolderPath, searchPattern: "*", searchOption: SearchOption.AllDirectories);
            // enumerate through the list of files loading the assets they represent and getting their type

            foreach (string triggerPath in aFilePaths)
            {
                if (!triggerPath.Contains(".meta"))
                {
                    string sAssetPath = triggerPath.Substring(sDataPath.Length - 6);
                    ChainTrigger trigger = (ChainTrigger)AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(ChainTrigger));

                    trigger.triggered = EditorGUILayout.Toggle(trigger.name, trigger.triggered);
                }
            }
        }

        //Fold out for containers
        int containerEnumCount = System.Enum.GetValues(typeof(ContainerType)).Length;
        if (dController.containers.Count > containerEnumCount)
        {
            for (int i = containerEnumCount; i < dController.containers.Count; i++)
            {
                dController.containers.RemoveAt(i);
            }
        }
        else if (dController.containers.Count < containerEnumCount)
        {
            for (int i = dController.containers.Count; i < containerEnumCount; i++)
            {
                dController.containers.Add(null);
            }
        }
        showContainers = EditorGUILayout.Foldout(showContainers, "Containers");
        if (showContainers)
        {
            for (int i = 0; i < containerEnumCount; i++)
            {
                ContainerType tempType = (ContainerType)i;
                dController.containers[i] = (GameObject)EditorGUILayout.ObjectField(tempType.ToString(), dController.containers[i], typeof(GameObject), false);
            }
        }

        //Fold out for box images
        int boxEnumCount = System.Enum.GetValues(typeof(BoxImage)).Length;
        if (dController.boxImages.Count > boxEnumCount)
        {
            for (int i = boxEnumCount; i < dController.boxImages.Count; i++)
            {
                dController.boxImages.RemoveAt(i);
            }
        }
        else if (dController.boxImages.Count < boxEnumCount)
        {
            for (int i = dController.boxImages.Count; i < boxEnumCount; i++)
            {
                dController.boxImages.Add(null);
            }
        }
        if (dController.speakerBoxImages.Count > boxEnumCount)
        {
            for (int i = boxEnumCount; i < dController.speakerBoxImages.Count; i++)
            {
                dController.speakerBoxImages.RemoveAt(i);
            }
        }
        else if (dController.speakerBoxImages.Count < boxEnumCount)
        {
            for (int i = dController.speakerBoxImages.Count; i < boxEnumCount; i++)
            {
                dController.speakerBoxImages.Add(null);
            }
        }
        showBoxImages = EditorGUILayout.Foldout(showBoxImages, "Box Images");
        if (showBoxImages)
        {
            float inspectorWidth = Screen.width;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Box Image Name", EditorStyles.boldLabel, GUILayout.Width(inspectorWidth/3));
            EditorGUILayout.LabelField("Dialogue Box", EditorStyles.boldLabel, GUILayout.Width(inspectorWidth / 3));
            EditorGUILayout.LabelField("Speaker Box", EditorStyles.boldLabel, GUILayout.Width(inspectorWidth / 3));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < boxEnumCount; i++)
            {
                if (i > 0)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
                BoxImage tempType = (BoxImage)i;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tempType.ToString(), GUILayout.Width(inspectorWidth / 3 - 50));
                dController.boxImages[i] = (Sprite)EditorGUILayout.ObjectField("", dController.boxImages[i], typeof(Sprite), false, GUILayout.Width(120));
                dController.speakerBoxImages[i] = (Sprite)EditorGUILayout.ObjectField("", dController.speakerBoxImages[i], typeof(Sprite), false, GUILayout.Width(inspectorWidth / 3));
                EditorGUILayout.EndHorizontal();        
            }
        }


        //Current Dialogue Chain (Greyed out). Get rid of both GUI.enabled lines to access in inspector.
        if (dController.currentDialogueChain != null)
        {
            GUI.enabled = false;
            dController.currentDialogueChain = (DialogueChain)EditorGUILayout.ObjectField("Current Dialogue Chain", dController.currentDialogueChain, typeof(DialogueChain), false);
            GUI.enabled = true;
        }

        //Deciding if the mouse button is being held down
        //Records undo state if the mouse is not dragging, or if the GUI had changed during mouse dragging and the mouse button is realeased.
        if (Event.current.type == EventType.MouseDown)
        {
            if (!draggingMouse && guiChanged)
            {
                RecordStateForUndo();
                guiChanged = false;
            }
            draggingMouse = true;
        }
        if (Event.current.type == EventType.MouseUp)
        {
            if (initialFlag)
            {
                if (guiChanged && draggingMouse)
                {
                    RecordStateForUndo();
                    guiChanged = false;
                }

                draggingMouse = false;
            }
        }

        //Buttons to start or stop editing of containers.
        if (!editing)
        {
            if (GUILayout.Button("Edit This Container"))
            {
                loadedCanvas = PrefabUtility.InstantiatePrefab(dController.dialogueCanvas) as Canvas;
                editingContainer = new DialogueContainer();
                editing = true;
            }
            cType = (ContainerType)EditorGUILayout.EnumPopup(cType);

        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
            {
                int buttonCount = dButtons.Count;
                if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
                {
                    PrefabUtility.ReplacePrefab(dButtons[0], PrefabUtility.GetPrefabParent(dButtons[0]), ReplacePrefabOptions.ConnectToPrefab);
                    foreach (GameObject button in dButtons)
                    {
                        DestroyImmediate(button);
                    }
                    dButtons.Clear();
                }
                PrefabUtility.ReplacePrefab(editingContainer.container, PrefabUtility.GetPrefabParent(editingContainer.container), ReplacePrefabOptions.ConnectToPrefab);
                if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
                {
                    for (int i = 0; i < buttonCount; i++)
                    { 
                        AddDialogueButton();
                    }
                }
            }
            if (GUILayout.Button("Save Changes"))
            {
                editingContainer.container.GetComponent<Image>().enabled = false;
                if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
                {
                    PrefabUtility.ReplacePrefab(dButtons[0], PrefabUtility.GetPrefabParent(dButtons[0]), ReplacePrefabOptions.ConnectToPrefab);
                    foreach (Transform child in editingContainer.inputPanel.transform)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
                PrefabUtility.ReplacePrefab(editingContainer.container, PrefabUtility.GetPrefabParent(editingContainer.container), ReplacePrefabOptions.ConnectToPrefab);
                OnDestroy();
                return;
            }
            if (GUILayout.Button("Delete Changes"))
            {
                OnDestroy();
                return;
            }
            EditorGUILayout.EndHorizontal();

            //Setting the intial GUI values to represent the external variables
            if (!initialFlag)
            {
                loadedContainer = PrefabUtility.InstantiatePrefab(dController.containers[(int)cType]) as GameObject;
                loadedContainer.transform.SetParent(loadedCanvas.transform, false);
                SetupContainer();
                
                containerWidth = editingContainer.container.GetComponent<LayoutElement>().preferredWidth;
                containerPos = editingContainer.container.GetComponent<RectTransform>().anchoredPosition;

                for (int i = 0; i < boxEnumCount; i++)
                {
                    if (editingContainer.dialogueBox.GetComponent<Image>().sprite == dController.boxImages[i] && editingContainer.speakerNameBox.GetComponent<Image>().sprite == dController.speakerBoxImages[i])
                    {
                        dialogueImage = (BoxImage)i;
                        break;
                    }
                    else
                    {
                        dialogueImage = 0;
                    }
                }

                imageWidth = editingContainer.speakerImage.GetComponent<RectTransform>().rect.width;
                imageHeight = editingContainer.speakerImage.GetComponent<RectTransform>().rect.height;
                testImage = editingContainer.speakerImage.GetComponent<Image>().sprite;
                imageOpacity = editingContainer.speakerImage.GetComponent<Image>().color.a;

                speakerNameLeftPadding = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left;
                speakerNameRightPadding = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right;
                speakerNameTopPadding = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.top;
                speakerNameBottomPadding = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.bottom;
                speakerNameFont = editingContainer.speakerNameText.gameObject.GetComponent<Text>().font;
                speakerNameFontSize = editingContainer.speakerNameText.gameObject.GetComponent<Text>().fontSize;
                speakerNameFontColor = editingContainer.speakerNameText.gameObject.GetComponent<Text>().color;
                testSpeaker = editingContainer.speakerNameText.gameObject.GetComponent<Text>().text;
                speakerOpacity = editingContainer.speakerNameBox.GetComponent<Image>().color.a;

                dialogueLeftPadding = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.left;
                dialogueRightPadding = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.right;
                dialogueTopPadding = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.top;
                dialogueBottomPadding = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom;
                dialogueBoxMinimumWidth = editingContainer.dialogueBox.gameObject.GetComponent<LayoutElement>().minWidth;
                dialogueFont = editingContainer.dialogueText.gameObject.GetComponent<Text>().font;
                dialogueFontSize = editingContainer.dialogueText.gameObject.GetComponent<Text>().fontSize;
                dialogueFontColor = editingContainer.dialogueText.gameObject.GetComponent<Text>().color;
                dialogueOpacity = editingContainer.dialogueBox.GetComponent<Image>().color.a;

                imagePos = editingContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition;
                namePos = editingContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition;
                testDialogue = editingContainer.dialogueText.gameObject.GetComponent<Text>().text;

                if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
                {
                    inputIndent = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.left;
                    inputInitialSpacing = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.top;
                    inputOverallSpacing = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().spacing;

                    dButtons.Clear();
                    AddDialogueButton();
                    inputFont = dButtons[0].transform.GetChild(0).GetComponent<Text>().font;
                    inputFontSize = dButtons[0].transform.GetChild(0).GetComponent<Text>().fontSize;

                    inputNormalColor = dButtons[0].GetComponent<Button>().colors.normalColor;
                    inputHighlightColor = dButtons[0].GetComponent<Button>().colors.highlightedColor;
                }

                initialFlag = true;
                RecordStateForUndo();
                return;
            }
            else
            {
                //Handling the GUI
                try
                {
                    //Undo
                    Undo.RegisterCompleteObjectUndo(this, "Undo: Container Variable");
                    if (Event.current.commandName == "UndoRedoPerformed" && Event.current.type == EventType.ValidateCommand && undoIndex > 0)
                    {
                        RevertToRecordedState();
                    }



                    EditorGUILayout.LabelField("Editing: " + cType.ToString() + " (" + editingContainer.container.name + ")", EditorStyles.boldLabel);

                    //Container (the whole thing)
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Box Image", GUILayout.Width(90));
                    dialogueImage = (BoxImage)EditorGUILayout.EnumPopup(dialogueImage);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Container Width", GUILayout.Width(EditorGUIUtility.labelWidth));
                    containerWidth = EditorGUILayout.Slider(containerWidth, 0, editingContainer.container.transform.parent.GetComponentInParent<RectTransform>().rect.width, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Container Position", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    containerPos.x = EditorGUILayout.IntField("X", (int)containerPos.x, GUILayout.Width(50));
                    containerPos.y = EditorGUILayout.IntField("Y", (int)containerPos.y, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;
                   
                    EditorGUILayout.LabelField("---");

                    //Image
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Image Position", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    imagePos.x = EditorGUILayout.IntField("X", (int)imagePos.x, GUILayout.Width(50));
                    imagePos.y = EditorGUILayout.IntField("Y", (int)imagePos.y, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Image Size", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    imageWidth = EditorGUILayout.IntField("X", (int)Mathf.Clamp(imageWidth, 0, 2000), GUILayout.Width(100));
                    if (!lockImageAspectRatio)
                    {
                        imageHeight = EditorGUILayout.IntField("Y", (int)Mathf.Clamp(imageHeight, 0, 2000), GUILayout.Width(100));
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.FloatField("Y", editingContainer.speakerImage.GetComponent<RectTransform>().rect.height, GUILayout.Width(100));
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Lock Aspect Ratio", GUILayout.Width(EditorGUIUtility.labelWidth));
                    tempLockRatio = lockImageAspectRatio;
                    lockImageAspectRatio = EditorGUILayout.Toggle(lockImageAspectRatio);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Image Opacity", GUILayout.Width(EditorGUIUtility.labelWidth));
                    imageOpacity = EditorGUILayout.Slider(imageOpacity, 0, 1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("---");

                    //Speaker Name
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Position", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    namePos.x = EditorGUILayout.IntField("X", (int)namePos.x, GUILayout.Width(50));
                    namePos.y = EditorGUILayout.IntField("Y", (int)namePos.y, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Padding", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    EditorGUILayout.BeginVertical(GUILayout.Width(50));
                    EditorGUILayout.BeginHorizontal();
                    speakerNameLeftPadding = EditorGUILayout.IntField("L", Mathf.Clamp(speakerNameLeftPadding, 0, (int)containerWidth), GUILayout.MaxWidth(50));
                    speakerNameRightPadding = EditorGUILayout.IntField("R", Mathf.Clamp(speakerNameRightPadding, 0, (int)containerWidth), GUILayout.MaxWidth(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    speakerNameTopPadding = EditorGUILayout.IntField("T", Mathf.Clamp(speakerNameTopPadding, 0, (int)containerWidth), GUILayout.MaxWidth(50));
                    speakerNameBottomPadding = EditorGUILayout.IntField("B", Mathf.Clamp(speakerNameBottomPadding, 0, (int)containerWidth), GUILayout.MaxWidth(50));
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Font", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 5;
                    speakerNameFont = (Font)EditorGUILayout.ObjectField(speakerNameFont, typeof(Font), false, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Font Size", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                    EditorGUIUtility.labelWidth = 5;
                    speakerNameFontSize = EditorGUILayout.IntField(":", speakerNameFontSize, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Font Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                    speakerNameFontColor = EditorGUILayout.ColorField(speakerNameFontColor, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Opacity", GUILayout.Width(EditorGUIUtility.labelWidth));
                    speakerOpacity = EditorGUILayout.Slider(speakerOpacity, 0, 1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("---");

                    //Dialogue
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("See Max Width", GUILayout.Width(EditorGUIUtility.labelWidth));
                    seeContainerWidth = EditorGUILayout.Toggle(seeContainerWidth);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dialogue Min Width", GUILayout.Width(EditorGUIUtility.labelWidth));
                    dialogueBoxMinimumWidth = EditorGUILayout.Slider(dialogueBoxMinimumWidth, 0, containerWidth, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dialogue Padding", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 15;
                    EditorGUILayout.BeginVertical(GUILayout.Width(50));
                    EditorGUILayout.BeginHorizontal();
                    dialogueLeftPadding = EditorGUILayout.IntField("L", Mathf.Clamp(dialogueLeftPadding, 0, (int)containerWidth), GUILayout.Width(50));
                    dialogueRightPadding = EditorGUILayout.IntField("R", Mathf.Clamp(dialogueRightPadding, 0, (int)containerWidth), GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    dialogueTopPadding = EditorGUILayout.IntField("T", Mathf.Clamp(dialogueTopPadding, 0, (int)containerWidth), GUILayout.Width(50));
                    dialogueBottomPadding = EditorGUILayout.IntField("B", Mathf.Clamp(dialogueBottomPadding, 0, (int)containerWidth), GUILayout.Width(50));
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dialogue Font", GUILayout.Width(EditorGUIUtility.labelWidth));
                    EditorGUIUtility.labelWidth = 5;
                    dialogueFont = (Font)EditorGUILayout.ObjectField(dialogueFont, typeof(Font), false, GUILayout.Width(150));
                    editingContainer.dialogueText.gameObject.GetComponent<Text>().font = dialogueFont;
                    editingContainer.speakerNameText.gameObject.GetComponent<Text>().font = speakerNameFont;
                    EditorGUILayout.EndHorizontal();

                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dialogue Font Size", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                    EditorGUIUtility.labelWidth = 5;
                    dialogueFontSize = EditorGUILayout.IntField(":", dialogueFontSize, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = 0;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dialogue Font Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                    dialogueFontColor = EditorGUILayout.ColorField(dialogueFontColor, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Box Opacity", GUILayout.Width(EditorGUIUtility.labelWidth));
                    dialogueOpacity = EditorGUILayout.Slider(dialogueOpacity, 0, 1);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("---");

                    //Input
                    if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Input Indentation", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                        EditorGUIUtility.labelWidth = 5;
                        inputIndent = EditorGUILayout.IntField(":", inputIndent, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.LabelField("Input Initial Spacing", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                        EditorGUIUtility.labelWidth = 5;
                        inputInitialSpacing = EditorGUILayout.IntField(":", inputInitialSpacing, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.LabelField("Input Overall Spacing", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                        EditorGUIUtility.labelWidth = 5;
                        inputOverallSpacing = EditorGUILayout.FloatField(":", inputOverallSpacing, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUIUtility.labelWidth = 0;
                       
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Input Font", GUILayout.Width(EditorGUIUtility.labelWidth));
                        inputFont = (Font)EditorGUILayout.ObjectField(inputFont, typeof(Font), false, GUILayout.Width(150));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Input Font Size", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                        EditorGUIUtility.labelWidth = 5;
                        inputFontSize = EditorGUILayout.IntField(":", inputFontSize, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Normal Text Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                        inputNormalColor = EditorGUILayout.ColorField(inputNormalColor, GUILayout.Width(150));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Highlighted Text Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                        inputHighlightColor = EditorGUILayout.ColorField(inputHighlightColor, GUILayout.Width(150));
                        EditorGUILayout.EndHorizontal();

                        if (GUILayout.Button("Add Test Button"))
                        {
                            AddDialogueButton();
                        }

                        EditorGUILayout.LabelField("---");
                    }


                    //Test Values
                    EditorGUILayout.BeginHorizontal();
                    seeTestValues = EditorGUILayout.Foldout(seeTestValues, "Test Dialogue");
                    EditorGUILayout.EndHorizontal();

                    if (seeTestValues)
                    {                      
                        EditorGUILayout.BeginHorizontal();
                        testImage = (Sprite)EditorGUILayout.ObjectField(testImage, typeof(Sprite), false, GUILayout.Width(100), GUILayout.Height(100));
                        EditorGUILayout.BeginVertical();
                        testSpeaker = EditorGUILayout.TextField(testSpeaker, GUILayout.MaxWidth(200));
                        GUIStyle dialogueStyle = GUI.skin.GetStyle("TextArea");
                        dialogueStyle.wordWrap = true;
                        testDialogue = EditorGUILayout.TextArea(testDialogue, dialogueStyle, GUILayout.Height(60), GUILayout.MaxWidth(200));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
                catch (System.ArgumentException)
                {}
            }

            //If GUI changed, record undo state if the mouse button is not down, otherwise trigger guiChanged so the state will be recorded after the mouse is released.
            //Then set external variables
            if (GUI.changed && editing)
            {
                if (!draggingMouse)
                {
                    RecordStateForUndo();
                }
                else
                {
                    guiChanged = true;
                }

                SetExternals();
            }

            //Destroy the editing menu and objects when you leave the inspector
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnDestroy();
            }
        }
        EditorUtility.SetDirty(dController);
    }
    #endregion

    #region Additional Funtions
    void OnDestroy()
    {
        undoStates.Clear();
        undoIndex = -1;

        if (editing)
        {
            editingContainer.container.GetComponent<Image>().enabled = false;
            editing = false;

            foreach (Transform child in loadedCanvas.transform)
            {
                DestroyImmediate(child.gameObject, false);
            }
            DestroyImmediate(loadedCanvas.gameObject, false);
            editingContainer = null;
            initialFlag = false;
        }
    }

    void SetupContainer()
    {
        editingContainer.container = loadedContainer;
        editingContainer.speakerImage = loadedContainer.transform.Find("SpeakerImage").GetComponent<Image>();
        editingContainer.speakerNameBox = loadedContainer.transform.Find("SpeakerNameBox").gameObject;
        editingContainer.speakerNameText = loadedContainer.transform.Find("SpeakerNameBox").transform.Find("SpeakerName").GetComponent<Text>();
        editingContainer.dialogueBox = loadedContainer.transform.Find("DialogueBox").gameObject;
        editingContainer.dialogueText = loadedContainer.transform.Find("DialogueBox").transform.Find("Dialogue").GetComponent<Text>();
        editingContainer.inputPanel = loadedContainer.transform.Find("DialogueBox").transform.Find("InputArea").gameObject;
    }

    void SetExternals()
    {
        //Container
        editingContainer.container.GetComponent<LayoutElement>().preferredWidth = containerWidth;
        editingContainer.dialogueBox.GetComponent<Image>().sprite = dController.GetBoxSprite(dialogueImage);
        editingContainer.speakerNameBox.GetComponent<Image>().sprite = dController.GetSpeakerBoxSprite(dialogueImage);

        //Positions
        editingContainer.container.GetComponent<RectTransform>().anchoredPosition = containerPos;
        editingContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition = imagePos;
        editingContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition = namePos;

        //Image
        if (!lockImageAspectRatio)
        {
            editingContainer.speakerImage.GetComponent<RectTransform>().sizeDelta = new Vector2(imageWidth, imageHeight);
        }
        else
        {
            float ratio = imageWidth / editingContainer.speakerImage.GetComponent<RectTransform>().rect.width;
            imageHeight = imageHeight * ratio;
            editingContainer.speakerImage.GetComponent<RectTransform>().sizeDelta = new Vector2(imageWidth, imageHeight);
        }
        editingContainer.speakerImage.GetComponent<Image>().sprite = testImage;
        Color tempColor = editingContainer.speakerImage.GetComponent<Image>().color;
        tempColor.a = imageOpacity;
        editingContainer.speakerImage.GetComponent<Image>().color = tempColor;

        //Speaker Name
        editingContainer.speakerNameText.gameObject.GetComponent<Text>().font = speakerNameFont;
        editingContainer.speakerNameText.gameObject.GetComponent<Text>().fontSize = speakerNameFontSize;
        editingContainer.speakerNameText.gameObject.GetComponent<Text>().color = speakerNameFontColor;
        editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left = speakerNameLeftPadding;
        editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right = speakerNameRightPadding;
        editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.top = speakerNameTopPadding;
        editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.bottom = speakerNameBottomPadding;
        editingContainer.speakerNameBox.gameObject.GetComponent<LayoutElement>().ignoreLayout = !editingContainer.speakerNameBox.gameObject.GetComponent<LayoutElement>().ignoreLayout;
        editingContainer.speakerNameBox.gameObject.GetComponent<LayoutElement>().ignoreLayout = !editingContainer.speakerNameBox.gameObject.GetComponent<LayoutElement>().ignoreLayout;
        editingContainer.speakerNameText.GetComponent<Text>().text = testSpeaker;
        tempColor = editingContainer.speakerNameBox.GetComponent<Image>().color;
        tempColor.a = speakerOpacity;
        editingContainer.speakerNameBox.GetComponent<Image>().color = tempColor;

        //Dialogue
        editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.left = dialogueLeftPadding;
        editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.right = dialogueRightPadding;
        editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.top = dialogueTopPadding;
        editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = dialogueBottomPadding;
        editingContainer.dialogueBox.gameObject.GetComponent<LayoutElement>().minWidth = dialogueBoxMinimumWidth;
        editingContainer.dialogueText.gameObject.GetComponent<Text>().font = dialogueFont;
        editingContainer.dialogueText.gameObject.GetComponent<Text>().fontSize = dialogueFontSize;
        editingContainer.dialogueText.gameObject.GetComponent<Text>().color = dialogueFontColor;
        editingContainer.dialogueText.GetComponent<Text>().text = testDialogue;
        editingContainer.container.GetComponent<Image>().enabled = seeContainerWidth;
        tempColor = editingContainer.dialogueBox.GetComponent<Image>().color;
        tempColor.a = dialogueOpacity;
        editingContainer.dialogueBox.GetComponent<Image>().color = tempColor;

        //Input
        if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
        {
            editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.left = inputIndent;
            editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.top = inputInitialSpacing;
            editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().spacing = inputOverallSpacing;

            foreach (Transform child in editingContainer.inputPanel.transform)
            {
                child.GetChild(0).GetComponent<Text>().font = inputFont;
                child.GetChild(0).GetComponent<Text>().fontSize = inputFontSize;
                var colors = child.GetComponent<Button>().colors;
                colors.normalColor = inputNormalColor;
                colors.highlightedColor = inputHighlightColor;
                child.GetComponent<Button>().colors = colors;
            }
        }

    }

    void AddDialogueButton()
    {
        editingContainer.inputPanel.SetActive(true);
        GameObject tempButton = PrefabUtility.InstantiatePrefab(Resources.Load("ChainResources/Prefabs/UI/CanvasAndButton/DialogueButton")) as GameObject;
        tempButton.transform.SetParent(editingContainer.inputPanel.transform, false);
        dButtons.Add(tempButton);
    }
    #endregion

    #region Undo Functions
    //Undo Functions
    void RecordStateForUndo()
    {
        if (initialFlag)
        {
            undoStates.Add(new UndoStateDialogueController());

            undoStates[undoStates.Count - 1].seeContainerWidthUndo = editingContainer.container.GetComponent<Image>().enabled;
            undoStates[undoStates.Count - 1].lockImageAspectRatioUndo = tempLockRatio;

 
            //Container
            undoStates[undoStates.Count - 1].containerWidthUndo = editingContainer.container.GetComponent<LayoutElement>().preferredWidth;

            //Positions
            undoStates[undoStates.Count - 1].containerPosUndo = editingContainer.container.GetComponent<RectTransform>().anchoredPosition;
            undoStates[undoStates.Count - 1].imagePosUndo = editingContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition;
            undoStates[undoStates.Count - 1].namePosUndo = editingContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition;

            //Image
            undoStates[undoStates.Count - 1].imageWidthUndo = editingContainer.speakerImage.GetComponent<RectTransform>().sizeDelta.x;
            undoStates[undoStates.Count - 1].imageHeightUndo = editingContainer.speakerImage.GetComponent<RectTransform>().sizeDelta.y;
            undoStates[undoStates.Count - 1].testImageUndo = editingContainer.speakerImage.GetComponent<Image>().sprite;

            //Speaker Name
            undoStates[undoStates.Count - 1].speakerNameFontUndo = editingContainer.speakerNameText.gameObject.GetComponent<Text>().font;
            undoStates[undoStates.Count - 1].speakerNameFontSizeUndo = editingContainer.speakerNameText.gameObject.GetComponent<Text>().fontSize;
            undoStates[undoStates.Count - 1].speakerNameFontColorUndo = editingContainer.speakerNameText.GetComponent<Text>().color;
            undoStates[undoStates.Count - 1].speakerNameLeftPaddingUndo = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.left;
            undoStates[undoStates.Count - 1].speakerNameRightPaddingUndo = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.right;
            undoStates[undoStates.Count - 1].speakerNameTopPaddingUndo = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.top;
            undoStates[undoStates.Count - 1].speakerNameBottomPaddingUndo = editingContainer.speakerNameBox.gameObject.GetComponent<HorizontalLayoutGroup>().padding.bottom;
            undoStates[undoStates.Count - 1].testSpeakerUndo = editingContainer.speakerNameText.GetComponent<Text>().text;

            //Dialogue
            undoStates[undoStates.Count - 1].dialogueLeftPaddingUndo = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.left;
            undoStates[undoStates.Count - 1].dialogueRightPaddingUndo = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.right;
            undoStates[undoStates.Count - 1].dialogueFontColorUndo = editingContainer.dialogueText.GetComponent<Text>().color;
            undoStates[undoStates.Count - 1].dialogueTopPaddingUndo = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.top;
            undoStates[undoStates.Count - 1].dialogueBottomPaddingUndo = editingContainer.dialogueBox.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom;
            undoStates[undoStates.Count - 1].dialogueBoxMinimumWidthUndo = editingContainer.dialogueBox.gameObject.GetComponent<LayoutElement>().minWidth;
            undoStates[undoStates.Count - 1].dialogueFontUndo = editingContainer.dialogueText.gameObject.GetComponent<Text>().font;
            undoStates[undoStates.Count - 1].dialogueFontSizeUndo = editingContainer.dialogueText.gameObject.GetComponent<Text>().fontSize;
            undoStates[undoStates.Count - 1].testDialogueUndo = editingContainer.dialogueText.GetComponent<Text>().text;


            //Input
            if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
            {
                undoStates[undoStates.Count - 1].inputIndentUndo = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.left;
                undoStates[undoStates.Count - 1].inputInitialSpacingUndo = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().padding.top;
                undoStates[undoStates.Count - 1].inputOverallSpacingUndo = editingContainer.inputPanel.GetComponent<VerticalLayoutGroup>().spacing;
                undoStates[undoStates.Count - 1].inputFontUndo = dButtons[0].transform.GetChild(0).GetComponent<Text>().font;
                undoStates[undoStates.Count - 1].inputFontSizeUndo = dButtons[0].transform.GetChild(0).GetComponent<Text>().fontSize;
                undoStates[undoStates.Count - 1].inputNormalUndo = dButtons[0].GetComponent<Button>().colors.normalColor;
                undoStates[undoStates.Count - 1].inputHighlightUndo = dButtons[0].GetComponent<Button>().colors.highlightedColor;
            }

            undoIndex++;
            if (undoIndex >= undoMax)
            {
                undoIndex--;
                undoStates.RemoveAt(0);
            }
        }
    }

    void RevertToRecordedState()
    {
        undoIndex--;

        containerWidth = undoStates[undoIndex].containerWidthUndo;
        containerPos = undoStates[undoIndex].containerPosUndo;
        seeContainerWidth = undoStates[undoIndex].seeContainerWidthUndo;

        lockImageAspectRatio = undoStates[undoIndex].lockImageAspectRatioUndo;
        imageWidth = undoStates[undoIndex].imageWidthUndo;
        imageHeight = undoStates[undoIndex].imageHeightUndo;
        imagePos = undoStates[undoIndex].imagePosUndo;
        testImage = undoStates[undoIndex].testImageUndo;

        namePos = undoStates[undoIndex].namePosUndo;
        speakerNameLeftPadding = undoStates[undoIndex].speakerNameLeftPaddingUndo;
        speakerNameRightPadding = undoStates[undoIndex].speakerNameRightPaddingUndo;
        speakerNameTopPadding = undoStates[undoIndex].speakerNameTopPaddingUndo;
        speakerNameBottomPadding = undoStates[undoIndex].speakerNameBottomPaddingUndo;
        speakerNameFont = undoStates[undoIndex].speakerNameFontUndo;
        speakerNameFontSize = undoStates[undoIndex].speakerNameFontSizeUndo;
        speakerNameFontColor = undoStates[undoIndex].speakerNameFontColorUndo;
        testSpeaker = undoStates[undoIndex].testSpeakerUndo;

        dialogueLeftPadding = undoStates[undoIndex].dialogueLeftPaddingUndo;
        dialogueRightPadding = undoStates[undoIndex].dialogueRightPaddingUndo;
        dialogueTopPadding = undoStates[undoIndex].dialogueTopPaddingUndo;
        dialogueBottomPadding = undoStates[undoIndex].dialogueBottomPaddingUndo;
        dialogueBoxMinimumWidth = undoStates[undoIndex].dialogueBoxMinimumWidthUndo;
        dialogueFont = undoStates[undoIndex].dialogueFontUndo;
        dialogueFontSize = undoStates[undoIndex].dialogueFontSizeUndo;
        dialogueFontColor = undoStates[undoIndex].dialogueFontColorUndo;
        testDialogue = undoStates[undoIndex].testDialogueUndo;

        if (cType.ToString().Contains("Input") || cType.ToString().Contains("input"))
        {
            inputIndent = undoStates[undoIndex].inputIndentUndo;
            inputInitialSpacing = undoStates[undoIndex].inputInitialSpacingUndo;
            inputOverallSpacing = undoStates[undoIndex].inputOverallSpacingUndo;
            inputFont = undoStates[undoIndex].inputFontUndo;
            inputFontSize = undoStates[undoIndex].inputFontSizeUndo;
            inputNormalColor = undoStates[undoIndex].inputNormalUndo;
            inputHighlightColor = undoStates[undoIndex].inputHighlightUndo;
        }

        undoStates.RemoveAt(undoStates.Count - 1);
        SetExternals();
    }
    #endregion  
}


[System.Serializable]
public class UndoStateDialogueController
{
    public float containerWidthUndo = 550;
    public Vector2 containerPosUndo = new Vector2(0, -220);
    public bool seeContainerWidthUndo = false;

    public bool lockImageAspectRatioUndo = true;
    public float imageWidthUndo = 100;
    public float imageHeightUndo = 100;
    public Vector2 imagePosUndo = Vector2.zero;
    public Sprite testImageUndo;

    public Vector2 namePosUndo = Vector2.zero;
    public int speakerNameLeftPaddingUndo = 10;
    public int speakerNameRightPaddingUndo = 10;
    public int speakerNameTopPaddingUndo = 5;
    public int speakerNameBottomPaddingUndo = 5;
    public Color speakerNameFontColorUndo = Color.black;
    public Font speakerNameFontUndo;
    public int speakerNameFontSizeUndo = 19;
    public string testSpeakerUndo = "Speaker";

    public int dialogueLeftPaddingUndo = 10;
    public int dialogueRightPaddingUndo = 10;
    public int dialogueTopPaddingUndo = 5;
    public int dialogueBottomPaddingUndo = 5;
    public float dialogueBoxMinimumWidthUndo = 250;
    public Color dialogueFontColorUndo = Color.black;
    public Font dialogueFontUndo;
    public int dialogueFontSizeUndo = 14;
    public string testDialogueUndo = "Dialogue Box";

    public int inputIndentUndo;
    public int inputInitialSpacingUndo;
    public float inputOverallSpacingUndo;
    public Font inputFontUndo;
    public int inputFontSizeUndo;
    public Color inputNormalUndo;
    public Color inputHighlightUndo;
}