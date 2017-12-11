using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChainOnInteraction : MonoBehaviour
{
    public DialogueChain dialogueChain;
    public ChainAdditions additions;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(CheckForPress());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(CheckForPress());
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            StopAllCoroutines();
        }
    }

    IEnumerator CheckForPress()
    {
        do
        {
            if (!DialogueController.IsRunning() && !DialogueChainPreferences.GetHaltMovement())
            {
                bool buttonPressed = false;
                string buttonString = "";

                do
                {
                    yield return new WaitForEndOfFrame();
                    if (DialogueController.AdvanceButtonPressed() != "" && !DialogueChainPreferences.GetHaltMovement())
                    {
                        buttonPressed = true;
                        buttonString = DialogueController.AdvanceButtonPressed();
                    }
                } while (!buttonPressed);
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (Input.GetAxis(buttonString) != 0);

                if (additions == null)
                {
                    dialogueChain.StartChain();
                }
                else
                {
                    if (additions.dialogueChain == null)
                    {
                        additions.dialogueChain = dialogueChain;
                    }
                    additions.RunChain();
                }
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
