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
        
        // Hooks into the ui control and keypress
        public void Bind(Toggle toggle, string axisName)
        {
            if(toggle == null || string.IsNullOrEmpty(axisName))
                throw new UnityException("ToggleButtonPressBinder given bad init data");

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
                Toggle.isOn = !Toggle.isOn;
            }
        }

        private void BroadcastToggle(bool isOn)
        {
            if (OnToggle != null)
                OnToggle(Toggle.isOn);
        }
    }
}