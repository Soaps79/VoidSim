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

    public ConversationViewModel ViewModelPrefab;
    public Button ButtonPrefab;
    private ConversationViewModel _viewModel;


    Vector3 originalImageScale;
    Vector3 originalImagePos;
    Vector3 originalSpeakerPos;
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
        InstantiateViewModel();

        if (!currentEvent.noSpeaker)
        {
            _viewModel.EnableSpeaker(currentEvent.leftSide);
            _viewModel.SpeakerImage.sprite = currentEvent.speakerImage;
            _viewModel.SpeakerName.text = currentEvent.speaker;
        }

        if (!string.IsNullOrEmpty(currentEvent.dialogue))
        {
            _viewModel.BodyText.text = currentEvent.dialogue;
        }

        if (currentEvent.cEventType == ChainEventType.UserInput)
        {
            AddButtonsToViewModel(currentEvent);
        }
    }

    private void InstantiateViewModel()
    {
        loadedCanvas = Instantiate(dialogueCanvas);
        _viewModel = Instantiate(ViewModelPrefab, loadedCanvas.transform, false);
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

    private void AddButtonsToViewModel(ChainEvent dEvent)
    {
        for (int i = 0; i < dEvent.inputButtons.Count; i++)
        {
            var button = Instantiate(ButtonPrefab, _viewModel.ButtonHolder, false);
            button.onClick.AddListener(delegate { currentDialogueChain.GetNextEventFromInput(button.transform.GetSiblingIndex()); });
        }

        // secondary input?
        //for (int i = 0; i < dEvent.secondaryInputButtons.Count; i++)
        //{
        //    GameObject dialogueButton = Instantiate(Resources.Load("ChainResources/Prefabs/UI/CanvasAndButton/DialogueButton")) as GameObject;
        //    dialogueButton.transform.SetParent(container.inputPanel.transform);
        //    dialogueButton.transform.localScale = Vector3.one;
        //    int loopBack = i + dEvent.inputButtons.Count >= DialogueChainPreferences.characterBeforeInputText.Length ? 0 : i + dEvent.inputButtons.Count;
        //    dialogueButton.transform.GetChild(0).GetComponent<Text>().text = DialogueChainPreferences.characterBeforeInputText[loopBack] + dEvent.secondaryInputButtons[i].buttonText;
        //    dialogueButton.GetComponent<Button>().onClick.AddListener(delegate { currentDialogueChain.GetNextEventFromInput(dialogueButton.transform.GetSiblingIndex()); });
        //}
    }

    //Destroys the dialogue container instance.
    public void CloseDialogue()
    {
        StopCoroutine("FadeDialogue");
        if (_viewModel != null)
        {
            Destroy(_viewModel.gameObject);
            _viewModel = null;
        }
        if (loadedCanvas != null)
        {
            Destroy(loadedCanvas.gameObject);
            loadedCanvas = null;
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