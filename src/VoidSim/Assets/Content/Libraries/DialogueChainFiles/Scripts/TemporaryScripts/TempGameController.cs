using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameController : MonoBehaviour
{
    public static TempGameController instance;

    public string saveFilePath = "ExampleGameData1.dat";

    //Variables that Dialogue Chains use. Customize the script DialogueChainPreferences to reflect your own project's variables.
    public bool haltMovement;
    public GameObject player;
    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    //Only use these variables if you're not using scriptable objects for items
    public Dictionary<string, int> inventoryNotScriptable = new Dictionary<string, int>();
    [HideInInspector] public List<string> allItemNames;

    private void Awake()
    {
        instance = this;
    }
}
