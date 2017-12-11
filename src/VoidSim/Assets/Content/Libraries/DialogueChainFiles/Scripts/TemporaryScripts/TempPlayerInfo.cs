using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerInfo : MonoBehaviour
{
    //Everything here is used with dialogue chains. Customize the script DialogueChainPreferences to reflect your own variables.

    public string playerName;
    public bool isMale;
    public int experience;

    public int meleeStat;
    public int rangedStat;

    public List<Sprite> playerDialogueAvatars;
}
