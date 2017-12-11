using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// This is where the chain paused states and trigger values are saved and loaded for the Dialogue Chains. The file name to be saved or loaded is passed to
/// each function. Different file names would be used for different save states. It should be easy to fit this into your own saving method by putting the
/// variables from the ChainSavableData class below into your own class with your own variables that need serialization. Then add the marked sections
/// from the Save and Load functions into your own functions.
/// </summary>

[Serializable]
class ChainSavableData
{
    //Add these variables to your own serializable class for saving data
    public List<string> savedTriggerNames = new List<string>();
    public List<bool> savedTriggerBools = new List<bool>();
    public List<string> savedChainNames = new List<string>();
    public List<int> currentEventID = new List<int>();
}

public class SaveChainData
{
    public static void Save(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate))
        {
            ChainSavableData data = new ChainSavableData();

            //Add this section to your save function with "data" being your own serializable class
            ChainTrigger[] chainTriggers = Resources.LoadAll<ChainTrigger>(DialogueChainPreferences.triggerAssetPathway);
            foreach (ChainTrigger trigger in chainTriggers)
            {
                data.savedTriggerNames.Add(trigger.name);
                data.savedTriggerBools.Add(trigger.triggered);
            }

            DialogueChain[] chains = Resources.LoadAll<DialogueChain>(DialogueChainPreferences.chainAssetPathway);
            foreach (DialogueChain chain in chains)
            {
                data.savedChainNames.Add(chain.name);
                if (chain.paused == true && chain.currentEvent != null)
                {
                    data.currentEventID.Add(chain.currentEvent.eventID);
                }
                else
                {
                    data.currentEventID.Add(-1);
                }
            }
            //End of section

            bf.Serialize(file, data);
            Debug.Log("Saved to " + Application.persistentDataPath + "/" + fileName);
        }
    }

    public static void Load(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open))
            {
                ChainSavableData data = (ChainSavableData)bf.Deserialize(file);

                //Add this section to your load function with "data" being your own serializable class
                ChainTrigger[] chainTriggers = Resources.LoadAll<ChainTrigger>(DialogueChainPreferences.triggerAssetPathway);
                for (int i = 0; i < data.savedTriggerNames.Count; i++)
                {
                    foreach (ChainTrigger trigger in chainTriggers)
                    {
                        if (trigger.name == data.savedTriggerNames[i])
                        {
                            trigger.triggered = data.savedTriggerBools[i];
                            break;
                        }
                    }
                    //ChainTrigger loadedTrigger = (ChainTrigger)Resources.Load(DialogueChainPreferences.triggerAssetPathway + "/" + data.savedTriggerNames[i]);
                    //loadedTrigger.triggered = data.savedTriggerBools[i];
                }
                DialogueChain[] chains = Resources.LoadAll<DialogueChain>(DialogueChainPreferences.chainAssetPathway);
                for (int i = 0; i < data.savedChainNames.Count; i++)
                {
                    foreach (DialogueChain chain in chains)
                    {
                        if (chain.name == data.savedChainNames[i])
                        {
                            //DialogueChain loadedChain = (DialogueChain)Resources.Load(DialogueChainPreferences.chainAssetPathway + "/" + data.savedChainNames[i]);
                            if (data.currentEventID[i] > -1)
                            {

                                foreach (ChainEvent cEvent in chain.chainEvents)
                                {
                                    if (cEvent.eventID == data.currentEventID[i])
                                    {
                                        chain.paused = true;
                                        chain.currentEvent = cEvent;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                chain.paused = false;
                                chain.currentEvent = chain.startEvent;
                            }
                            break;
                        }
                    }
                }
                //End of section
                Debug.Log("Loaded " + Application.persistentDataPath + "/" + fileName);
            }
        }
    }
}
