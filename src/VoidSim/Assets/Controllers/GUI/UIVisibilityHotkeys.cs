using System;
using System.Collections.Generic;
using System.Linq;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UIHelpers
{
	[Serializable]
	public class HotkeyPair
	{
		public string Name;
		public KeyCode Key;
		public GameObject Element;
	}

	/// <summary>
	/// Add this behavior to a game manager or UI controller
	/// can link KeyCodes and GameObjects in the inspector
	/// </summary>
	public class UIVisibilityHotkeys : QScript
	{
		private class HotkeyEntry
		{
			public string Name;
			public GameObject Element;
			public bool IsAxisInUse = false;
		}

		public HotkeyPair[] Hotkeys;

		private Dictionary<KeyCode, HotkeyEntry> _hotkeyTable;

		void Start ()
		{
			// this is getting hit twice but I can't explain why
			// adding the null and key check stopped it
			if (!Hotkeys.Any())
				return;

			_hotkeyTable = new Dictionary<KeyCode, HotkeyEntry>();
			foreach (var hotkeyPair in Hotkeys)
			{
				//if(!_hotkeyTable.ContainsKey(hotkeyPair.Key))
					_hotkeyTable.Add(hotkeyPair.Key, new HotkeyEntry
					{
						Name = hotkeyPair.Name, Element = hotkeyPair.Element
					});
			}

			OnEveryUpdate += CheckForKeyPress;
		}

		private string GenerateDisplayText()
		{
			var s = _hotkeyTable.Aggregate("",
				(current, hotkey) => current + string.Format("{0}\t\t\t\t\t\t\t{1}\n", hotkey.Value.Name, hotkey.Key));

			return s;
		}

		void CheckForKeyPress(float delta)
		{
			foreach (var entry in _hotkeyTable)
			{
				if (Input.GetKeyDown(entry.Key))
				{
					// Toggle Active on the GameObject
					entry.Value.Element.SetActive(!entry.Value.Element.activeSelf);
				}
			}
		}
	}
}
