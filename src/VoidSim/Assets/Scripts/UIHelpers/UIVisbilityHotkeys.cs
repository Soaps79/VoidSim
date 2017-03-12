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
		public KeyCode Axis;
		public GameObject Element;
	}

	/// <summary>
	/// Add this behavior to a game manager or UI controller
	/// can link KeyCodes and GameObjects in the inspector
	/// </summary>
	public class UIVisbilityHotkeys : QScript
	{
		private class HotkeyEntry
		{
			public GameObject Element;
			public bool IsAxisInUse;
		}

		public HotkeyPair[] Hotkeys;

		private Dictionary<KeyCode, HotkeyEntry> _hotkeyTable;

		void Start ()
		{
			// this is getting hit twice but I can't explain why
			// adding the null and key check stopped it
			if (!Hotkeys.Any() || _hotkeyTable != null)
				return;

			_hotkeyTable = new Dictionary<KeyCode, HotkeyEntry>();
			foreach (var hotkeyPair in Hotkeys)
			{
				if(!_hotkeyTable.ContainsKey(hotkeyPair.Axis))
					_hotkeyTable.Add(hotkeyPair.Axis, new HotkeyEntry { Element = hotkeyPair.Element});
			}
			OnEveryUpdate += CheckForKeyPress;
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
