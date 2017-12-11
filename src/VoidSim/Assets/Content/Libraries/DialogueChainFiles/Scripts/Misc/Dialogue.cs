using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// You can customize this script as you please. The Dialogue Controller will call the method DialogueSyntaxFix and pass to it the dialogue from the 
/// dialogue chains. Any shorthand you want to type into the dialogue chain that will return a specific string can be placed here.
/// Feel free to delete everything in the method DialogueSyntaxFix as long as you can still call it and pass through a string. 
/// If you don't want to change your text at all, have the method return the string it was passed. "return text" is all you'd need to write in the method.
/// </summary>

public static class Dialogue
{
    public static string DialogueSyntaxFix(string text)
    {
        //Makes a temporary string and sets it equal to the original text.
        string newText = text;

        //Replaces all instances of "/playerName" in the text to that of your player's name. Make sure to customize this in the DialogueChainPreferences.
        newText = newText.Replace("/playerName", DialogueChainPreferences.GetPlayerName());

        //If the player's gender is male change the dialogue accordingly, otherwise change it to accurately speak with females.
        if (DialogueChainPreferences.IsPlayerGenderMale())
        {
            newText = newText.Replace("/he", "he");
            newText = newText.Replace("/he's", "he's");
            newText = newText.Replace("/his", "his");
            newText = newText.Replace("/hiss", "his");
            newText = newText.Replace("/him", "him");
            newText = newText.Replace("/guy", "guy");
            newText = newText.Replace("/man", "man");

            newText = newText.Replace("/He", "He");
            newText = newText.Replace("/He's", "He's");
            newText = newText.Replace("/His", "His");
            newText = newText.Replace("/Hiss", "His");
            newText = newText.Replace("/Him", "Him");
            newText = newText.Replace("/Guy", "Guy");
            newText = newText.Replace("/Man", "Man");
        }
        else
        {
            newText = newText.Replace("/he", "She");
            newText = newText.Replace("/he's", "she's");
            newText = newText.Replace("/his", "her");
            newText = newText.Replace("/hiss", "hers");
            newText = newText.Replace("/him", "her");
            newText = newText.Replace("/guy", "gal");
            newText = newText.Replace("/man", "woman");

            newText = newText.Replace("/He", "She");
            newText = newText.Replace("/He's", "She's");
            newText = newText.Replace("/His", "Her");
            newText = newText.Replace("/Hiss", "Hers");
            newText = newText.Replace("/Him", "Her");
            newText = newText.Replace("/Guy", "Gal");
            newText = newText.Replace("/Man", "Woman");
        }

        return newText;

        //You must type the the first part of each Replace() statement into your dialogue boxes, and in the game it will be replaced with the correct word.
        //Example: Your player has chosen the name "Jen" and the female gender. In a dialogue box you've typed:
        //         "I don't think /playerName can kill that Ogre. /He's not /man enough."
        //         In game, the player will see the dialogue:
        //         "I don't think Jen can kill that Ogre. She's not woman enough."

        //Add as many of your own statements as you'd like. Anything the player will choose for themselves that will change the words that the dialogue uses
            //should be placed here.
    }
}
