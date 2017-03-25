using System;
using QGame;
using UnityEngine;
using UnityEngine.UI;

//todo: Update QGame unity reference, cannot reference UnityEngine.UI with current reference
namespace Assets.Scripts.UIHelpers
{
	/// <summary>
	/// Use TextBindingBehavior and TextBindingPublisher to bridge scripts with UI Text objects
	/// Add Behavior to GameObjects with a Text object, and Publisher alongside the script 
	/// you want to access it with. Drag Behavior into Publisher in the editor, and use it in your script:
	/// GetComponent<TextBindingPublisher>().SetText(GenerateDisplayText()); - set once
	/// GetComponent<TextBindingPublisher>().SetBinding(GenerateDisplayText); - give a func to refresh every frame
	/// </summary>
	public class TextBindingReceiver : QScript
	{
		private Text _text;
		private Func<object> _binding;

		void Awake()
		{
			var text = GetComponent<Text>();

			if (text == null)
			{
				throw new UnityException(string.Format("{0} has no Text Object", GetType()));
			}
			_text = text;
		}

		// can set the text periodically (or just once)
		public void SetText(string text)
		{
			OnNextUpdate += (delta) => SetTextActual(text);
		}

		// or you can bind it to refresh every frame
		public void SetBinding(Func<object> func)
		{
			_binding = func;
			OnEveryUpdate = UpdateBoundProperty;
		}

		public void ReleaseBinding()
		{
			_binding = null;
			OnEveryUpdate = null;
		}

		private void UpdateBoundProperty(float value)
		{
			_text.text = _binding().ToString();
		}

		private void SetTextActual(string text)
		{
			_text.text = text;
		}
	}
}
