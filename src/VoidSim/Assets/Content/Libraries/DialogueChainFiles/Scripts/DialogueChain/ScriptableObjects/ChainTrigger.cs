using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewChainTrigger", menuName = "Dialogue Chains/Trigger")]
[System.Serializable]
public class ChainTrigger : ScriptableObject
{
    public bool triggered = false;

}
