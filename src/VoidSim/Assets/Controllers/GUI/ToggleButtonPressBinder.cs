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
        private Toggle _toggle;

        public Action<bool> OnToggle;
        public bool IsOn {  get { return _toggle.isOn; } }
        
        // Hooks into the ui control and keypress
        public void Bind(Toggle toggle, string axisName)
        {
            if(toggle == null || string.IsNullOrEmpty(axisName))
                throw new UnityException("ToggleButtonPressBinder given bad init data");

            _toggle = toggle;
            _axisName = axisName;
            _toggle.onValueChanged.AddListener(BroadcastToggle);

            OnEveryUpdate += CheckForVisibilityToggleKeypress;
        }

        // keypress just drives the toggle, which tells subscribers
        private void CheckForVisibilityToggleKeypress(float obj)
        {
            if (Input.GetButtonDown(_axisName))
            {
                _toggle.isOn = !_toggle.isOn;
            }
        }

        private void BroadcastToggle(bool isOn)
        {
            if (OnToggle != null)
                OnToggle(_toggle.isOn);
        }
    }
}