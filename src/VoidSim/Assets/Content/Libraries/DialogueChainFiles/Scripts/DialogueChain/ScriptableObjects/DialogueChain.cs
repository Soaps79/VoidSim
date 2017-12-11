using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueChain", menuName = "Dialogue Chains/Chain")]
public class DialogueChain : ScriptableObject
{
    #region Declarations
    [HideInInspector] public int nodeIDCount = 0;
    [HideInInspector] public bool paused = false;
    [HideInInspector] public bool hasEnded = false;

    private bool originalHaltMovement;

    public bool haltMovement = true;
    public bool defaultShowImages = DialogueChainPreferences.defaultShowSpeakerImage;
    public bool defaultShowNames = DialogueChainPreferences.defaultShowSpeakerNameBox;
    public Sprite defaultSprite;
    public string defaultSpeaker;
    public ContainerType defaultContainerType = DialogueChainPreferences.defaultContainerType;
    public float defaultTextDelay = 0.02f;

    private bool waitForConfirm = false;

    [HideInInspector] public DialogueChain beforeSubDialogue;
    [HideInInspector] public bool isSubDialogueChain;

    [HideInInspector] public List<string> speakers = new List<string>();

    [HideInInspector] public ChainEvent startEvent = null;
    [HideInInspector] public ChainEvent currentEvent;
    [HideInInspector] private ChainEvent nextEvent;

    [HideInInspector] public List<ChainEvent> chainEvents = new List<ChainEvent>();
    [HideInInspector] public ChainAdditions additions;
    #endregion

    private void Awake()
    {
        speakers.AddRange(DialogueChainPreferences.defaultSpeakerList);
        waitForConfirm = false;
    }

    private void OnEnable()
    {
        paused = false;
    }

    #region StartChain
    public void StartChain()
    {
        if (!isSubDialogueChain)
        {
            originalHaltMovement = DialogueChainPreferences.GetHaltMovement();
            DialogueChainPreferences.SetHaltMovement(haltMovement);
        }

        DialogueController.instance.isRunning = true;
        hasEnded = false;

        DialogueController.instance.currentDialogueChain = this;

        if (!paused)
        {
            RunEvent(startEvent);
        }
        else
        {
            paused = false;
            GetNextEvent();
        }
    }
    #endregion

    #region RunEvent
    public void RunEvent(ChainEvent cEvent)
    {
        DialogueController.instance.isRunning = true;
        DialogueController.instance.CloseDialogue();
        if (hasEnded)
        {
            ChainEnded();
            return;
        }

        currentEvent = cEvent;

        if (cEvent.cEventType == ChainEventType.Dialogue)
        {
            DialogueEvent();
        }
        else if (cEvent.cEventType == ChainEventType.ItemManagement)
        {
            ItemManagement();
        }
        else if (cEvent.cEventType == ChainEventType.Pause)
        {
            Pause();
        }
        else if (cEvent.cEventType == ChainEventType.SetTrigger)
        {
            SetTrigger();
        }
        else if (cEvent.cEventType == ChainEventType.UserInput)
        {
            currentEvent.nextEventIDs.Clear();
            UserInput();
        }
        else if (cEvent.cEventType == ChainEventType.SubDialogue)
        {
            SubDialogue();
        }
        else if (cEvent.cEventType == ChainEventType.Audio)
        {
            Audio();
        }
        else if (cEvent.cEventType == ChainEventType.IntAdjustment)
        {
            StatAdjust();
        }
        else if (cEvent.cEventType == ChainEventType.Message)
        {
            Message();
        }
        else if (cEvent.cEventType == ChainEventType.Start)
        {
            StartNode();
        }
        else if (cEvent.cEventType == ChainEventType.Check)
        {
            Check();
        }
    }
    #endregion

    #region EventCode
    private void StartNode()
    {
        GetNextEvent();
    }
    private void DialogueEvent()
    {
        waitForConfirm = true;
        DialogueController.instance.ShowDialogue(currentEvent);
        GetNextEvent();
    }
    private void ItemManagement()
    {
        if (DialogueChainPreferences.itemsAreScriptableObjects)
        {
            foreach (Item item in currentEvent.itemsGiven)
            {
                DialogueChainPreferences.AddToInventory(item);
            }
            foreach (Item item in currentEvent.itemsTaken)
            {
                DialogueChainPreferences.RemoveFromInventory(item);
            }
        }
        else
        {
            foreach (string itemString in currentEvent.itemsGivenString)
            {
                DialogueChainPreferences.AddToInventory(itemString);
            }
            foreach (string itemString in currentEvent.itemsTakenString)
            {
                DialogueChainPreferences.RemoveFromInventory(itemString);
            }
        }


        DialogueChainPreferences.AddToPlayerExperience(currentEvent.experienceGiven);

        GetNextEvent();
    }
    private void Pause()
    {
        paused = true;
        DialogueChainPreferences.SetHaltMovement(originalHaltMovement);
        DialogueController.instance.isRunning = false;
    }
    private void SetTrigger()
    {
        for (int i = 0; i < currentEvent.triggers.Count; i++)
        {
            currentEvent.triggers[i].triggered = currentEvent.triggerBools[i];
        }

        GetNextEvent();
    }
    private void UserInput()
    {
        currentEvent.secondaryInputButtons.Clear();

        for (int i = 0; i < currentEvent.lateralConnections.Count; i++)
        {
            foreach (ChainEvent dEvent2 in chainEvents)
            {
                if (dEvent2.eventID == currentEvent.lateralConnections[i])
                {
                    if (EventPassesCheck(dEvent2))
                    {
                        foreach (DialogueEventInputButton inputButton in dEvent2.inputButtons)
                        {
                            currentEvent.secondaryInputButtons.Add(inputButton);
                        }
                    }
                    break;
                }
            }
        }

        DialogueController.instance.ShowDialogue(currentEvent);
    }
    private void SubDialogue()
    {
        paused = true;

        currentEvent.subDialogue.isSubDialogueChain = true;
        currentEvent.subDialogue.beforeSubDialogue = this;
        currentEvent.subDialogue.StartChain();
    }
    private void Audio()
    {
        if (currentEvent.overlay)
        {
            ChainAudioController.instance.AddTempSource(currentEvent.audio, currentEvent.fadeTime, currentEvent.audioVolume);
        }
        else
        {
            ChainAudioController.instance.CrossFade(currentEvent.audio, currentEvent.fadeTime, currentEvent.loop, currentEvent.playOriginalAfter, currentEvent.originalFadeTime, currentEvent.audioVolume);
        }
        GetNextEvent();
    }
    private void StatAdjust()
    {
        for (int i = 0; i < currentEvent.chainIntAdjustments.Count; i++)
        {
            DialogueChainPreferences.SetChainInt(currentEvent.chainIntAdjustments[i].intAdjusted, currentEvent.chainIntAdjustments[i].value);           
        }

        GetNextEvent();
    }
    private void Message()
    {
        if (currentEvent.sendMessage[0])
        {
            additions.ChainMessage(currentEvent.messageFloat);
        }
        if (currentEvent.sendMessage[1])
        {
            additions.ChainMessage(currentEvent.messageString);
        }
        if (currentEvent.sendMessage[2])
        {
            additions.ChainMessage(currentEvent.messageBool);
        }

        GetNextEvent();
    }
    private void Check()
    {
        GetNextEvent();
    }
    #endregion
    
    #region GettingNextEvent
    public void GetNextEvent()
    {
        if (currentEvent.nextEventIDs.Count == 1)
        {
            nextEvent = NextEventOneOption(currentEvent);
        }
        else if (currentEvent.nextEventIDs.Count == 0)
        {
            hasEnded = true;
        }
        else
        {
            nextEvent = NextEventMoreOptions(currentEvent);
        }
        if (nextEvent == null)
        {
            hasEnded = true;
        }

        if (!paused)
        {
            if (waitForConfirm)
            {
                waitForConfirm = false;
                DialogueController.instance.StartCoroutine(DialogueController.instance.RunNextEventAfterUserConfirms(this, nextEvent));
            }
            else
            {
                DialogueController.instance.CloseDialogue();
                RunEvent(nextEvent);
            }
        }
    }

    ChainEvent NextEventOneOption(ChainEvent dEvent)
    {
        foreach (ChainEvent dEvent2 in chainEvents)
        {
            if (dEvent2.eventID == dEvent.nextEventIDs[0])
            {
                if (EventPassesCheck(dEvent2))
                {
                    return dEvent2;
                }
            }
        }

        Debug.Log("Couldn't Get next Quest event " + dEvent.eventID);
        return null;
    }

    ChainEvent NextEventMoreOptions(ChainEvent dEvent)
    {
        //Gets all next events
        List<ChainEvent> possibleEvents = new List<ChainEvent>();
        for (int i = 0; i < dEvent.nextEventIDs.Count; i++)
        {
            foreach (ChainEvent dEvent2 in chainEvents)
            {
                if (dEvent2.eventID == dEvent.nextEventIDs[i])
                {
                    possibleEvents.Add(dEvent2);
                    break;
                }
            }
        }
        //Orders next events by rank
        List<ChainEvent> eventsOrdered = new List<ChainEvent>();
        int checkRank = possibleEvents.Count - 1;
        int whileCheck = 0;
        while (eventsOrdered.Count < possibleEvents.Count || whileCheck < 400)
        {
            int sameRankCount = 0;
            for (int i = 0; i < possibleEvents.Count; i++)
            {
                if (possibleEvents[i].rank >= checkRank && !eventsOrdered.Contains(possibleEvents[i]))
                {
                    eventsOrdered.Insert(sameRankCount, possibleEvents[i]);
                    sameRankCount++;
                }
            }
            checkRank--;
            whileCheck++;
        }
        //Finds first event on list that passes requirements
        for (int i = 0; i < eventsOrdered.Count; i++)
        {
            if (EventPassesCheck(eventsOrdered[i]))
            {
                return eventsOrdered[i];
            }
        }

        Debug.Log("Couldn't Get next Quest event " + dEvent.eventID);
        return null;
    }

    bool EventPassesCheck(ChainEvent dEvent)
    {
        if (dEvent.cEventType == ChainEventType.Check || dEvent.cEventType == ChainEventType.SecondaryInput)
        {
            for (int i = 0; i < dEvent.triggerChecks.Count; i++)
            {
                if (dEvent.triggerChecks[i].triggered != dEvent.triggerCheckBools[i])
                {
                    return false;
                }
            }
            for (int i = 0; i < dEvent.itemChecks.Count; i++)
            {
                if (DialogueChainPreferences.itemsAreScriptableObjects)
                {
                    if (!DialogueChainPreferences.InventoryContains(dEvent.itemChecks[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!DialogueChainPreferences.InventoryContainsString(dEvent.itemChecksString[i]))
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < dEvent.chainIntChecks.Count; i++)
            {
                if (dEvent.chainIntChecks[i].equator == 0)
                {
                    if (DialogueChainPreferences.GetChainInt(dEvent.chainIntChecks[i].intNeeded) >= dEvent.chainIntChecks[i].value)
                    {
                        return false;
                    }
                }
                else if (dEvent.chainIntChecks[i].equator == 1)
                {
                    if (DialogueChainPreferences.GetChainInt(dEvent.chainIntChecks[i].intNeeded) <= dEvent.chainIntChecks[i].value)
                    {
                        return false;
                    }
                }
                else
                {
                    if (DialogueChainPreferences.GetChainInt(dEvent.chainIntChecks[i].intNeeded) != dEvent.chainIntChecks[i].value)
                    {
                        return false;
                    }
                }              
            }
        }
        return true;
    }

    public void GetNextEventFromInput(int inputIndex)
    {
        List<int> nextEventIDs;

        if (inputIndex <= currentEvent.inputButtons.Count - 1)
        {
            nextEventIDs = currentEvent.inputButtons[inputIndex].nextEventIDsForInputs;
        }
        else
        {
            nextEventIDs = currentEvent.secondaryInputButtons[inputIndex - (currentEvent.inputButtons.Count)].nextEventIDsForInputs;
        }

        if (nextEventIDs.Count == 1)
        {
            nextEvent = NextEventOneOptionUserInput(nextEventIDs[0]);
        }
        else if (nextEventIDs.Count == 0)
        {
            hasEnded = true;
        }
        else
        {
            nextEvent = NextEventMoreOptionsUserInput(nextEventIDs);
        }
        if (nextEvent == null)
        {
            hasEnded = true;
        }

        if (!paused)
        {
            if (waitForConfirm)
            {
                waitForConfirm = false;
                DialogueController.instance.StartCoroutine(DialogueController.instance.RunNextEventAfterUserConfirms(this, nextEvent));
            }
            else
            {
                DialogueController.instance.CloseDialogue();
                RunEvent(nextEvent);
            }
        }
    }

    ChainEvent NextEventOneOptionUserInput(int eventID)
    {
        foreach (ChainEvent dEvent2 in chainEvents)
        {
            if (dEvent2.eventID == eventID)
            {
                if (EventPassesCheck(dEvent2))
                {
                    return dEvent2;
                }
            }
        }

        Debug.Log("Couldn't Get next Quest event from user input");
        return null;
    }

    ChainEvent NextEventMoreOptionsUserInput(List<int> nextEventIDs)
    {
        //Gets all next events
        List<ChainEvent> possibleEvents = new List<ChainEvent>();
        for (int i = 0; i < nextEventIDs.Count; i++)
        {
            foreach (ChainEvent dEvent2 in chainEvents)
            {
                if (dEvent2.eventID == nextEventIDs[i])
                {
                    possibleEvents.Add(dEvent2);
                    break;
                }
            }
        }
        //Orders next events by rank
        List<ChainEvent> eventsOrdered = new List<ChainEvent>();
        int checkRank = possibleEvents.Count - 1;
        int whileCheck = 0;
        while (eventsOrdered.Count < possibleEvents.Count || whileCheck < 400)
        {
            int sameRankCount = 0;
            for (int i = 0; i < possibleEvents.Count; i++)
            {
                if (possibleEvents[i].rank >= checkRank && !eventsOrdered.Contains(possibleEvents[i]))
                {
                    eventsOrdered.Insert(sameRankCount, possibleEvents[i]);
                    sameRankCount++;
                }
            }
            checkRank--;
            whileCheck++;
        }
        //Finds first event on list that passes requirements
        for (int i = 0; i < eventsOrdered.Count; i++)
        {
            if (EventPassesCheck(eventsOrdered[i]))
            {
                return eventsOrdered[i];
            }
        }

        Debug.Log("Couldn't Get next Quest event from user input");
        return null;
    }    
    #endregion  
    

    void ChainEnded()
    {
        if (!isSubDialogueChain)
        {
            DialogueChainPreferences.SetHaltMovement(originalHaltMovement);
            DialogueController.instance.isRunning = false;
            if (additions != null)
            {
                additions.OnChainEnd();
            }
        }
        else
        {
            beforeSubDialogue.paused = false;
            beforeSubDialogue.GetNextEvent();
        }
    } 
}
