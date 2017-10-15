using System;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Controllers.GUI
{
	/// <summary>
	/// Will keep a UI Toggle control and a keypress toggle in sync.
	/// Also exposes a callback, enabling a single point of contact.
	/// Note that this cannot be placed on the object being toggled because it needs update for keypresses.
	/// </summary>
	public class ToggleButtonPressBinder : QScript
	{
		private string _axisName;
		public Toggle Toggle { get; private set; }

		public Action<bool> OnToggle;
		public bool IsOn {  get { return Toggle.isOn; } }

		// used to bind object to a key that can only turn it off
		// common use is for "cancel" to close/stop things
		private bool _keypressOnlyToggleOff;
		
		// Hooks into the ui control and keypress
		public void Bind(Toggle toggle, string axisName, bool keyPressOnlyToggleOff = false)
		{
			if(toggle == null || string.IsNullOrEmpty(axisName))
				throw new UnityException("ToggleButtonPressBinder given bad init data");

			_keypressOnlyToggleOff = keyPressOnlyToggleOff;
			Toggle = toggle;
			_axisName = axisName;
			Toggle.onValueChanged.AddListener(BroadcastToggle);

			OnEveryUpdate += CheckForVisibilityToggleKeypress;
		}

		// keypress just drives the toggle, which tells subscribers
		private void CheckForVisibilityToggleKeypress(float obj)
		{
			if (Input.GetButtonDown(_axisName))
			{
				if (Toggle.isOn)
					Toggle.isOn = false;
				else if(!_keypressOnlyToggleOff)
					Toggle.isOn = true;
			}
		}

		private void BroadcastToggle(bool isOn)
		{
			if (OnToggle != null)
				OnToggle(Toggle.isOn);
		}
	}
}