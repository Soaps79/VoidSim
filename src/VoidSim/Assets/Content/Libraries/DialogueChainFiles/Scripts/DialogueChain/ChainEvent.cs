using System.Collections;
using System.Collections.Generic;
using Assets.Narrative.Missions;
using UnityEngine;

[System.Serializable]
public class ChainEvent
{
    public ChainEventType cEventType;
    public int eventID;

    public List<int> nextEventIDs = new List<int>();
    public List<int> previousEventIDs = new List<int>();
    public List<int> lateralConnections = new List<int>();

    [HideInInspector]
    public Rect windowRect;

    public int rank;

    #region Dialogue
    [TextArea(3, 3)]
    public string dialogue;
    public ContainerType dialogueContainer;
    public Sprite speakerImage;
    public int playerImageIndex;
    public bool useCustomPlayerImage;
    public bool flipImage;
    public bool leftSide = true;
    public bool showImage;
    public string speaker;
    public bool noSpeaker;
    public float textDelay = 0.02f;
    public float dialogueWaitTime = 0;
    public float dialoguefadeTime = 0;
    #endregion

    #region UserInput
    public List<DialogueEventInputButton> inputButtons = new List<DialogueEventInputButton>();
    #endregion

    #region TriggerSet
    public List<ChainTrigger> triggers = new List<ChainTrigger>();
    public List<bool> triggerBools = new List<bool>();
    #endregion

    #region Item/Experience
    public List<Item> itemsGiven = new List<Item>();
    public List<string> itemsGivenString = new List<string>();
    public List<Item> itemsTaken = new List<Item>();
    public List<string> itemsTakenString = new List<string>();
    public int experienceGiven;
    public List<MissionSO> missions = new List<MissionSO>();
    #endregion

    #region Sub Dialogue
    public DialogueChain subDialogue;
    #endregion

    #region Audio
    public AudioClip audio;
    public bool loop = false;
    public bool overlay = true;
    public float fadeTime;
    public bool playOriginalAfter = true;
    public float originalFadeTime = 1.25f;
    public float audioVolume = 1;
    #endregion

    #region IntegerAdjustment
    public List<IntAdjustment> chainIntAdjustments = new List<IntAdjustment>();
    #endregion

    #region TriggerCheck
    public List<ChainTrigger> triggerChecks = new List<ChainTrigger>();
    public List<bool> triggerCheckBools = new List<bool>();
    #endregion

    #region ItemCheck
    public List<Item> itemChecks = new List<Item>();
    public List<string> itemChecksString = new List<string>();
    #endregion
    
    #region IntegerCheck
    public List<IntCheck> chainIntChecks = new List<IntCheck>();
    #endregion

    #region SecondaryInput
    public List<DialogueEventInputButton> secondaryInputButtons = new List<DialogueEventInputButton>();
    #endregion

    #region Message
    public bool[] sendMessage = new bool[3];
    public float messageFloat;
    public string messageString;
    public bool messageBool;
    #endregion
}

#region Integers for adjustment or check nodes
[System.Serializable]
public class IntCheck
{
    public ChainIntType intNeeded;
    [Range(0,2)]
    public int equator;
    public int value = 1;
}
[System.Serializable]
public class IntAdjustment
{
    public ChainIntType intAdjusted;
    public int value;
}
#endregion

[System.Serializable]
public class DialogueEventInputButton
{
    public string buttonText;
    public List<int> nextEventIDsForInputs = new List<int>();
}

public enum ChainEventType
{
    Start,
    Dialogue,
    SetTrigger,
    UserInput,
    ItemManagement,
    Pause,
    Audio,
    IntAdjustment,
    SubDialogue,
    Check,
    Message,
    SecondaryInput,
    Mission
}