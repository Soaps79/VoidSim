using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DialogueController : MonoBehaviour
{
    #region Declarations
    public static DialogueController instance;

    //User Customizable options
    public Canvas dialogueCanvas;
    public List<GameObject> containers = new List<GameObject>();
    public List<Sprite> boxImages = new List<Sprite>();
    public List<Sprite> speakerBoxImages = new List<Sprite>();
    public bool chainDataReset = true;

    public DialogueChain currentDialogueChain;

    Vector3 originalImageScale;
    Vector3 originalImagePos;
    Vector3 originalSpeakerPos;
    DialogueContainer currentContainer = null;
    GameObject loadedContainer = null;
    Canvas loadedCanvas;
    ChainEvent currentEvent;

    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isWriting;
    [HideInInspector] public bool isFading;
    [HideInInspector] public bool isWaiting;
    [HideInInspector] public bool finishWriting;
    [HideInInspector] public float tempTextDelay;
    #endregion

    private void Awake()
    {
        //Singleton
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            isRunning = false;
        }
    }

    //Putting the loading and saving methods here get around the fact that unity assets keep their changes after leaving playmode.
    private void OnDisable()
    {
        if (chainDataReset)
        {
            SaveChainData.Load("tempChainData.dat");
        }
    }
    private void Start()
    {
        if (chainDataReset)
        {
            SaveChainData.Save("tempChainData.dat");
        }
    }

    #region Handling Button Presses
    //Waits for the dialogue advancement button to be pressed
    public IEnumerator RunNextEventAfterUserConfirms(DialogueChain chain, ChainEvent cEvent)
    {
        bool buttonPressed = false;
        string buttonString = "";

        while (isWaiting || isFading)
        {
            yield return new WaitForEndOfFrame();
        }

        if (isWriting)
        {
            do
            {
                yield return new WaitForEndOfFrame();
                if (FinishTextButtonPressed() != null && FinishTextButtonPressed() != "")
                {
                    buttonPressed = true;
                    buttonString = FinishTextButtonPressed();
                }
            } while (!buttonPressed && isWriting);
            if (isWriting)
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (Input.GetAxis(buttonString) != 0 && isWriting);

                finishWriting = true;
            }
        }

        buttonPressed = false;
        buttonString = "";
        yield return new WaitForEndOfFrame();

        do
        {
            yield return new WaitForEndOfFrame();
            if (AdvanceButtonPressed() != null && AdvanceButtonPressed() != "")
            {
                buttonPressed = true;
                buttonString = AdvanceButtonPressed();
            }
        } while (!buttonPressed);
        do
        {
            yield return new WaitForEndOfFrame();
        } while (Input.GetAxis(buttonString) != 0);


        CloseDialogue();
        chain.RunEvent(cEvent);
    }

    public static string AdvanceButtonPressed()
    {
        for (int i = 0; i < DialogueChainPreferences.inputsToAdvanceDialogue.Length; i++)
        {
            if (Input.GetAxis(DialogueChainPreferences.inputsToAdvanceDialogue[i]) > 0)
            {
                return DialogueChainPreferences.inputsToAdvanceDialogue[i];
            }
        }

        return "";
    }
    public static string FinishTextButtonPressed()
    {
        for (int i = 0; i < DialogueChainPreferences.inputsToAdvanceDialogueQuickly.Length; i++)
        {
            if (Input.GetAxis(DialogueChainPreferences.inputsToAdvanceDialogueQuickly[i]) > 0)
            {
                return DialogueChainPreferences.inputsToAdvanceDialogueQuickly[i];
            }
        }

        return "";
    }
    #endregion

    #region Handling Dialogue Boxes
    //The method called when a chain event is a dialogue or user input. It will call the appropriate methods to instantiate dialogue boxes and write text on them.
    public void ShowDialogue(ChainEvent cEvent)
    {
        isRunning = true;
        finishWriting = false;
        isWriting = false;
        isFading = false;
        isWaiting = true;
        currentEvent = cEvent;


        Invoke("ContinueDialogue", cEvent.dialogueWaitTime);     
    }

    void ContinueDialogue()
    {
        isWaiting = false;
        currentContainer = new DialogueContainer();
        GetContainer(currentEvent.dialogueContainer);
        currentContainer.container.SetActive(true);

        float speakerTextToBoxDiff = currentContainer.speakerNameBox.GetComponent<RectTransform>().rect.width - currentContainer.speakerNameText.GetComponent<RectTransform>().rect.width;

        if (currentEvent.dialoguefadeTime > 0)
        {
            StartCoroutine("FadeDialogue", currentEvent.dialoguefadeTime);
        }

        if (currentEvent.showImage)
        {
            currentContainer.speakerImage.enabled = true;

            if (currentEvent.useCustomPlayerImage)
            {
                currentEvent.speakerImage = DialogueChainPreferences.GetPlayerDialogueAvatars()[currentEvent.playerImageIndex];
            }

            currentContainer.speakerImage.GetComponent<Image>().sprite = currentEvent.speakerImage;
           
            originalImageScale = currentContainer.speakerImage.GetComponent<RectTransform>().localScale;

            if (currentEvent.flipImage)
            {
                currentContainer.speakerImage.rectTransform.localScale = new Vector3(originalImageScale.x * -1, originalImageScale.y, originalImageScale.z);

                if (currentContainer.speakerImage.GetComponent<LayoutElement>().ignoreLayout)
                {
                    Vector3 tempPos = currentContainer.speakerImage.rectTransform.anchoredPosition;
                    tempPos.x += currentContainer.speakerImage.rectTransform.rect.width;
                    currentContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition = tempPos;
                }
                else
                {
                    currentContainer.speakerImage.transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left += (int)currentContainer.speakerImage.rectTransform.rect.width;
                }
            }
        }
        else
        {
            currentContainer.speakerImage.enabled = false;
        }

        if (currentEvent.noSpeaker || currentEvent.speaker == "")
        {
            currentContainer.speakerNameBox.SetActive(false);
        }
        else
        {
            currentContainer.speakerNameBox.SetActive(true);
            if (currentEvent.speaker == "Player")
            {
                currentContainer.speakerNameText.text = DialogueChainPreferences.GetPlayerName();
            }
            else
            {
                currentContainer.speakerNameText.text = currentEvent.speaker;
            }

            originalSpeakerPos = currentContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition;

            if (!currentEvent.leftSide)
            {
                Vector2 containerPos = currentContainer.container.GetComponent<RectTransform>().anchoredPosition;
                containerPos.x = loadedCanvas.GetComponent<RectTransform>().rect.width - currentContainer.container.GetComponent<LayoutElement>().preferredWidth - containerPos.x;
                currentContainer.container.GetComponent<RectTransform>().anchoredPosition = containerPos;

                currentContainer.dialogueBox.transform.parent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerRight;
                currentContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentContainer.dialogueBox.transform.parent.GetComponent<RectTransform>().rect.width - (currentContainer.speakerNameText.GetComponent<RectTransform>().rect.width + speakerTextToBoxDiff) + originalSpeakerPos.x, originalSpeakerPos.y, originalSpeakerPos.z);

                if (currentEvent.showImage)
                {
                    Vector2 pos = currentContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition;
                    int mod = -1;
                    if (currentEvent.flipImage)
                    {
                        mod = 1;
                    }
                    pos.x = (currentContainer.container.GetComponent<LayoutElement>().preferredWidth - currentContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition.x + currentEvent.speakerImage.rect.width * mod);
                    currentContainer.speakerImage.GetComponent<RectTransform>().anchoredPosition = pos;
                }
                if (!currentEvent.noSpeaker)
                {
                    Vector2 pos = currentContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition;
                    pos.x = (currentContainer.container.GetComponent<LayoutElement>().preferredWidth - currentContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition.x - currentContainer.speakerNameText.GetComponent<Text>().preferredWidth - pos.x);
                    currentContainer.speakerNameBox.GetComponent<RectTransform>().anchoredPosition = pos;
                }
            }
            else
            {
                currentContainer.dialogueBox.transform.parent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerLeft;
            }
        }

        if (currentEvent.dialogue != "")
        {
            currentContainer.dialogueBox.SetActive(true);
        }
        else
        {
            currentContainer.container.SetActive(true);
            if (currentEvent.cEventType == ChainEventType.UserInput)
            {
                currentContainer.dialogueBox.SetActive(true);
                currentContainer.dialogueText.enabled = false;
            }
            else
            {
                currentContainer.dialogueBox.SetActive(false);
            }
        }

        if (currentDialogueChain == null)
        {
            StartCoroutine(TypeOutDialogue(Dialogue.DialogueSyntaxFix(currentEvent.dialogue), tempTextDelay, currentEvent));
        }
        else
        {
            StartCoroutine(TypeOutDialogue(Dialogue.DialogueSyntaxFix(currentEvent.dialogue), currentEvent.textDelay, currentEvent));
        }

        if (currentEvent.cEventType == ChainEventType.UserInput)
        {
            StartCoroutine(ShowInputs(currentEvent, currentContainer));
        }
        else
        {
            if (currentContainer.inputPanel != null)
            {
                currentContainer.inputPanel.SetActive(false);
            }
        }
    }

    //Instantiates the correct dialogue container prefab chosen by the dialogue chain event
    void GetContainer(ContainerType cType)
    {
        loadedCanvas = Instantiate(dialogueCanvas) as Canvas;
        loadedCanvas.worldCamera = Camera.main;
        loadedContainer = Instantiate(containers[(int)cType], loadedCanvas.transform, false) as GameObject;
        SetupContainer();
    }

    //Gets the sprite associated with the boximage enum
    public Sprite GetBoxSprite(BoxImage bType)
    {
        return boxImages[(int)bType];
    }
    public Sprite GetSpeakerBoxSprite(BoxImage bType)
    {
        return speakerBoxImages[(int)bType];
    }

    //Sets these variables equal to their appropriate gameobjects
    void SetupContainer()
    {
        currentContainer.container = loadedContainer;
        currentContainer.speakerImage = loadedContainer.transform.Find("SpeakerImage").GetComponent<Image>();
        currentContainer.speakerNameBox = loadedContainer.transform.Find("SpeakerNameBox").gameObject;
        currentContainer.speakerNameText = loadedContainer.transform.Find("SpeakerNameBox").transform.Find("SpeakerName").GetComponent<Text>();
        currentContainer.dialogueBox = loadedContainer.transform.Find("DialogueBox").gameObject;
        currentContainer.dialogueText = loadedContainer.transform.Find("DialogueBox").transform.Find("Dialogue").GetComponent<Text>();
        currentContainer.inputPanel = currentContainer.dialogueBox.transform.Find("InputArea").gameObject;

        currentContainer.dialogueText.text = "";
        currentContainer.speakerNameText.text = "";
    }

    //Types out the dialogue one character at a time depending on a textDelay variable taken from the current dialogue chain
    public IEnumerator TypeOutDialogue(string text, float delay, ChainEvent dEvent)
    {
        if (delay == 0)
        {
            //If the delay is 0, the text immediately fills the box
            currentContainer.dialogueText.text = text;
        }
        else
        {
            //Set the dialogue box text to the final text to break it into lines. This stops the end of the line from starting to type a word and then moving it to the next line.
            isWriting = true;
            currentContainer.dialogueText.text = text;
            if (dEvent.cEventType == ChainEventType.UserInput)
            {
                UserInput(dEvent, currentContainer);
            }

            Canvas.ForceUpdateCanvases();

            string[] lines = new string[currentContainer.dialogueText.cachedTextGenerator.lines.Count];
            for (int i = 0; i < currentContainer.dialogueText.cachedTextGenerator.lines.Count; i++)
            {
                int startIndex = currentContainer.dialogueText.cachedTextGenerator.lines[i].startCharIdx;
                int endIndex = (i == currentContainer.dialogueText.cachedTextGenerator.lines.Count - 1) ? currentContainer.dialogueText.text.Length
                    : currentContainer.dialogueText.cachedTextGenerator.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;
                lines[i] = (currentContainer.dialogueText.text.Substring(startIndex, length));
            }

            //Set the preferred box size to that of its final size so it doesn't change size while typing.
            float width = currentContainer.dialogueBox.GetComponent<RectTransform>().sizeDelta.x;
            currentContainer.dialogueBox.GetComponent<LayoutElement>().preferredWidth = width;
            float height = currentContainer.dialogueBox.GetComponent<RectTransform>().sizeDelta.y;
            currentContainer.dialogueBox.GetComponent<LayoutElement>().preferredHeight = height;

            //Reset the box to have no text or buttons
            if (dEvent.cEventType == ChainEventType.UserInput)
            {
                foreach (Transform child in currentContainer.inputPanel.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            currentContainer.dialogueText.text = "";
            Canvas.ForceUpdateCanvases();

            //Write the text to the box one line at a time and one character at a time
            for (int i = 0; i < lines.Length; i++)
            {
                int characterCount = 0;
                while (characterCount < lines[i].Length)
                {
                    if (finishWriting)
                    {
                        currentContainer.dialogueText.text = text;
                        isWriting = false;
                        finishWriting = false;
                        yield break;
                    }

                    currentContainer.dialogueText.text += lines[i][characterCount++];

                    yield return new WaitForSeconds(delay);
                }

                //Enter the line breaks at the end of each line so that a word doesn't start typing on one line and drop to another.
                if (i < lines.Length - 1)
                {
                    currentContainer.dialogueText.text += "\n";
                }
            }

            isWriting = false;
        }
    }

    //Waits for the dialogue text to be finished writing before showing the possible user inputs
    public IEnumerator ShowInputs(ChainEvent dEvent, DialogueContainer container)
    {
        while (isWriting)
        {
            bool buttonPressed = false;
            string buttonString = "";

            do
            {
                yield return new WaitForEndOfFrame();
                if (FinishTextButtonPressed() != null && FinishTextButtonPressed() != "")
                {
                    buttonPressed = true;
                    buttonString = FinishTextButtonPressed();
                }
            } while (!buttonPressed && isWriting);
            while (isWriting && Input.GetAxis(buttonString) != 0)
            {
                yield return new WaitForEndOfFrame();
            }

            if (isWriting)
            {
                finishWriting = true;
                yield return new WaitForEndOfFrame();
            }
        }

        UserInput(dEvent, currentContainer);
    }

    //If the dialogue box has user inputs this adds prefab buttons for each one
    public void UserInput(ChainEvent dEvent, DialogueContainer container)
    {
        container.inputPanel.SetActive(true);

        for (int i = 0; i < dEvent.inputButtons.Count; i++)
        {
            GameObject dialogueButton = Instantiate(Resources.Load("ChainResources/Prefabs/UI/CanvasAndButton/DialogueButton")) as GameObject;
            dialogueButton.transform.SetParent(container.inputPanel.transform);
            dialogueButton.transform.localScale = Vector3.one;
            int loopBack = i >= DialogueChainPreferences.characterBeforeInputText.Length ? 0 : i;
            dialogueButton.transform.GetChild(0).GetComponent<Text>().text = DialogueChainPreferences.characterBeforeInputText[loopBack] + dEvent.inputButtons[i].buttonText;
            dialogueButton.GetComponent<Button>().onClick.AddListener(delegate { currentDialogueChain.GetNextEventFromInput(dialogueButton.transform.GetSiblingIndex()); });
        }
        for (int i = 0; i < dEvent.secondaryInputButtons.Count; i++)
        {
            GameObject dialogueButton = Instantiate(Resources.Load("ChainResources/Prefabs/UI/CanvasAndButton/DialogueButton")) as GameObject;
            dialogueButton.transform.SetParent(container.inputPanel.transform);
            dialogueButton.transform.localScale = Vector3.one;
            int loopBack = i + dEvent.inputButtons.Count >= DialogueChainPreferences.characterBeforeInputText.Length ? 0 : i + dEvent.inputButtons.Count;
            dialogueButton.transform.GetChild(0).GetComponent<Text>().text = DialogueChainPreferences.characterBeforeInputText[loopBack] + dEvent.secondaryInputButtons[i].buttonText;
            dialogueButton.GetComponent<Button>().onClick.AddListener(delegate { currentDialogueChain.GetNextEventFromInput(dialogueButton.transform.GetSiblingIndex()); });
        }
    }

    //Fades the dialogue box as directed by ChainEvent dialogueFade
    IEnumerator FadeDialogue(float fadeTime)
    {
        isFading = true;

        Color tempImage = currentContainer.speakerImage.GetComponent<Image>().color;
        float o1 = tempImage.a;
        tempImage.a = 0;
        Color tempName = currentContainer.speakerNameBox.GetComponent<Image>().color;
        float o2 = tempName.a;
        tempName.a = 0;
        Color tempDialogue = currentContainer.dialogueBox.GetComponent<Image>().color;
        float o3 = tempDialogue.a;
        tempDialogue.a = 0;
        Color tempDialogueText = currentContainer.dialogueText.GetComponent<Text>().color;
        float o4 = tempDialogueText.a;
        tempDialogueText.a = 0;
        Color tempNameText = currentContainer.speakerNameText.GetComponent<Text>().color;
        float o5 = tempNameText.a;
        tempNameText.a = 0;

        float runningTime = 0;
        while (runningTime <= fadeTime)
        {
            currentContainer.speakerImage.GetComponent<Image>().color = tempImage;
            currentContainer.speakerNameBox.GetComponent<Image>().color = tempName;
            currentContainer.dialogueBox.GetComponent<Image>().color = tempDialogue;
            currentContainer.dialogueText.GetComponent<Text>().color = tempDialogueText;
            currentContainer.speakerNameText.GetComponent<Text>().color = tempNameText;

            tempImage.a = Mathf.Lerp(0, o1, 1f - Mathf.Cos((runningTime / fadeTime) * Mathf.PI * 0.5f));
            tempName.a = Mathf.Lerp(0, o2, 1f - Mathf.Cos((runningTime / fadeTime) * Mathf.PI * 0.5f));
            tempDialogue.a = Mathf.Lerp(0, o3, 1f - Mathf.Cos((runningTime / fadeTime) * Mathf.PI * 0.5f));
            tempDialogueText.a = Mathf.Lerp(0, o4, 1f - Mathf.Cos((runningTime / fadeTime) * Mathf.PI * 0.5f));
            tempNameText.a = Mathf.Lerp(0, o5, 1f - Mathf.Cos((runningTime / fadeTime) * Mathf.PI * 0.5f));

            runningTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isFading = false;
    }


    //Destroys the dialogue container instance.
    public void CloseDialogue()
    {
        StopCoroutine("FadeDialogue");
        if (currentContainer != null)
        {
            currentContainer = null;
            if (loadedCanvas != null)
            {
                Destroy(loadedContainer.gameObject);
                Destroy(loadedCanvas.gameObject);
            }
        }
    }
    #endregion

    public static bool IsRunning()
    {
        return instance.isRunning;
    }
    public static bool IsWriting()
    {
        return instance.isWriting;
    }
    public static bool IsFading()
    {
        return instance.isFading;
    }
    public static bool IsWaiting()
    {
        return instance.isWaiting;
    }
}


//Used to assign gameobjects that will be accessed often when showing dialogue.
[System.Serializable]
public class DialogueContainer
{
    public GameObject container;
    public Image speakerImage;
    public GameObject speakerNameBox;
    public Text speakerNameText;
    public GameObject dialogueBox;
    public Text dialogueText;
    public GameObject inputPanel;
}