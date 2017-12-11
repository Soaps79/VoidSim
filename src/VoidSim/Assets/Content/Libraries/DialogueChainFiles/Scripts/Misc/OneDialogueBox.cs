using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneDialogueBox : MonoBehaviour
{
    public bool useInteractionCode = true;
    public float textDelay = 0.02f;

    public Sprite speakerImage;
    public string speaker;
    public string dialogue;

    public bool leftSide = true;
    public bool flipImage = false;

    public bool useCustomPlayerImage = false;
    public int playerImageIndex = 0;

    public ContainerType dialogueContainer = DialogueChainPreferences.defaultContainerType;
    public BoxImage boxSpriteType;
    public bool showImage = DialogueChainPreferences.defaultShowSpeakerImage;
    public bool showSpeakerName = DialogueChainPreferences.defaultShowSpeakerNameBox;

    public bool haltMovement = true;
    bool originalHaltMovement;

    public void ShowDialogue()
    {
        DialogueChainPreferences.SetHaltMovement(haltMovement);
        ChainEvent tempEvent = new ChainEvent()
        {
            speakerImage = speakerImage,
            speaker = speaker,
            dialogue = dialogue,
            leftSide = leftSide,
            flipImage = flipImage,
            useCustomPlayerImage = useCustomPlayerImage,
            playerImageIndex = playerImageIndex,
            dialogueContainer = dialogueContainer,
            showImage = showImage,
            noSpeaker = !showSpeakerName
        };

        DialogueController.instance.tempTextDelay = textDelay;
        DialogueController.instance.ShowDialogue(tempEvent);
        StartCoroutine("CancelDialogueWhenAdvanced");
    }

    public IEnumerator CancelDialogueWhenAdvanced()
    {
        bool buttonPressed = false;
        string buttonString = "";

        if (DialogueController.instance.isWriting)
        {
            do
            {
                yield return new WaitForEndOfFrame();
                if (DialogueController.FinishTextButtonPressed() != null && DialogueController.FinishTextButtonPressed() != "")
                {
                    buttonPressed = true;
                    buttonString = DialogueController.FinishTextButtonPressed();
                }
            } while (!buttonPressed && DialogueController.instance.isWriting);
            if (DialogueController.instance.isWriting)
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (Input.GetAxis(buttonString) != 0 && DialogueController.instance.isWriting);

                DialogueController.instance.finishWriting = true;
            }
        }

        buttonPressed = false;
        buttonString = "";
        yield return new WaitForEndOfFrame();

        do
        {
            yield return new WaitForEndOfFrame();
            if (DialogueController.AdvanceButtonPressed() != null && DialogueController.AdvanceButtonPressed() != "")
            {
                buttonPressed = true;
                buttonString = DialogueController.AdvanceButtonPressed();
            }
        } while (!buttonPressed);
        do
        {
            yield return new WaitForEndOfFrame();
        } while (Input.GetAxis(buttonString) != 0);

        DialogueChainPreferences.SetHaltMovement(originalHaltMovement);
        DialogueController.instance.CloseDialogue();
        DialogueController.instance.isRunning = false;
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !DialogueChainPreferences.GetHaltMovement() && useInteractionCode)
        {
            StartCoroutine(CheckForPress());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !DialogueChainPreferences.GetHaltMovement() && useInteractionCode)
        {
            StopAllCoroutines();
            if (DialogueController.IsRunning())
            {
                StartCoroutine(CancelDialogueWhenAdvanced());
            }
        }
    }

    IEnumerator CheckForPress()
    {
        do
        {
            if (!DialogueController.IsRunning())
            {
                bool buttonPressed = false;
                string buttonString = "";

                do
                {
                    yield return new WaitForEndOfFrame();
                    if (DialogueController.AdvanceButtonPressed() != "")
                    {
                        buttonPressed = true;
                        buttonString = DialogueController.AdvanceButtonPressed();
                    }
                } while (!buttonPressed);
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (Input.GetAxis(buttonString) != 0);

                originalHaltMovement = DialogueChainPreferences.GetHaltMovement();
                ShowDialogue();
                buttonPressed = false;
                buttonString = "";
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        } while (true);
    }
}
