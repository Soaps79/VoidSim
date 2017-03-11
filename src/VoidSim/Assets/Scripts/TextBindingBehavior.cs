using System;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	/// <summary>
	/// Drop onto any GameObject with a Text component, 
	/// serves as a hook into its text field
	/// </summary>
	public class TextBindingBehavior : QScript
	{
		private Text _text;
		private Func<object> _binding;

		void Awake ()
		{
			var text = GetComponent<Text>();

			if (text == null)
			{
				throw new UnityException(string.Format("{0} has no Text Object", GetType()));
			}
			_text = text;
		}

		// can either set the text periodically (or just once)
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
