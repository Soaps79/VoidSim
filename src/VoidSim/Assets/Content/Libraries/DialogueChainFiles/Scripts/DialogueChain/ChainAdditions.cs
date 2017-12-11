using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAdditions : MonoBehaviour
{
    public DialogueChain dialogueChain;
    public bool interactable = true;
    public bool oneShot = false;

    public void RunChain()
    {
        if (interactable)
        {
            dialogueChain.additions = this;
            dialogueChain.StartChain();
        }
    }

    public virtual void ChainMessage(float messageFloat)
    { }
    public virtual void ChainMessage(string messageString)
    { }
    public virtual void ChainMessage(bool messageBool)
    { }

    public virtual void OnChainEnd()
    {
        if (oneShot)
        {
            interactable = false;
        }
    }
}
