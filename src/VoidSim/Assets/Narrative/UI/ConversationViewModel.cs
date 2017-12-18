using System.Collections;
using System.Collections.Generic;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationViewModel : QScript
{
    [HideInInspector] public Image SpeakerImage;
    [HideInInspector] public TMP_Text SpeakerName;
    public TMP_Text BodyText;
    public RectTransform ButtonHolder;
    public GameObject SpeakerLeft;
    public GameObject SpeakerRight;

    public void EnableSpeaker(bool isLeft)
    {
        SpeakerLeft.SetActive(isLeft);
        SpeakerRight.SetActive(!isLeft);

        if (isLeft)
        {
            SpeakerName = SpeakerLeft.transform.Find("speaker_name").GetComponent<TMP_Text>();
            SpeakerImage = SpeakerLeft.transform.Find("speaker_image").GetComponent<Image>();
        }
        else
        {
            SpeakerName = SpeakerRight.transform.Find("speaker_name").GetComponent<TMP_Text>();
            SpeakerImage = SpeakerRight.transform.Find("speaker_image").GetComponent<Image>();
        }
    }
}
