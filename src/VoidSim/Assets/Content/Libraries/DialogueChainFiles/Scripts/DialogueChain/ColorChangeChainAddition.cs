using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeChainAddition : ChainAdditions
{
    public override void ChainMessage(bool messageBool)
    {
        base.ChainMessage(messageBool);

        if (messageBool)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public override void OnChainEnd()
    {
        base.OnChainEnd();

        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
