using System;
using UnityEngine;

namespace Assets.Scripts.UIHelpers
{
	/// <summary>
	/// Use TextBindingBehavior and TextBindingPublisher to bridge scripts with UI Text objects
	/// Add Behavior to GameObjects with a Text object, and Publisher alongside the script 
	/// you want to access it with. Drag Behavior into Publisher in the editor, and use it in your script:
	/// GetComponent<TextBindingPublisher>().SetText(GenerateDisplayText()); - set once
	/// GetComponent<TextBindingPublisher>().SetBinding(GenerateDisplayText); - give a func to refresh every frame
	/// </summary>
	public class TextBindingPublisher : MonoBehaviour
	{
		public TextBindingReceiver DisplayReciever;

		public void SetText(string text)
		{
			DisplayReciever.SetText(text);
		}

		public void SetBinding(Func<object> func)
		{
			DisplayReciever.SetBinding(func);
		}
	}
}
