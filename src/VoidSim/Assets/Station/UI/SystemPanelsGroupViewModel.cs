using System.Collections.Generic;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station.UI
{
    public class SystemPanelsGroupViewModel : QScript, IMessageListener
    {
        private class SystemPanelEntry
        {
            public Toggle Toggle;
            public SystemPanel Panel;
        }

        public RectTransform PanelPrefab;
        public Toggle ButtonPrefab;

        private RectTransform _panelInstance;
        private RectTransform _contentHolder;
        private readonly List<SystemPanelEntry> _entries = new List<SystemPanelEntry>();

        void Start()
        {
            var canvas = GameObject.Find("InfoCanvas");
            _panelInstance = Instantiate(PanelPrefab, canvas.transform, false);
            _contentHolder = _panelInstance.FindChild("content_holder").GetComponent<RectTransform>();
            //_panelInstance.gameObject.SetActive(false);

            MessageHub.Instance.AddListener(this, SystemPanel.MessageName);
            OnEveryUpdate += CheckForVisibilityToggleKeypress;
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == SystemPanel.MessageName && args != null)
                HandleSystemAdd(args as SystemPanelMessageArgs);
        }

        private void HandleSystemAdd(SystemPanelMessageArgs args)
        {
            if(args == null || args.SystemPanel == null)
                throw new UnityException("SystemPanelsViewModel recieved bad message data");

            // binds system panel to a toggle button, setting toggle isOn to current state
            var toggle = Instantiate(ButtonPrefab, _contentHolder, false);
            toggle.isOn = args.SystemPanel.Panel.activeSelf;

            // set the icon and add local tracking
            var icon = toggle.transform.FindChild("icon").GetComponent<Image>();
            icon.sprite = args.SystemPanel.Icon;

            var entry = new SystemPanelEntry {Panel = args.SystemPanel, Toggle = toggle};
            _entries.Add(entry);
            toggle.onValueChanged.AddListener(isOn => SetVisibility(isOn, entry));

            // turn main panel visible if it is not already
            if (!_panelInstance.gameObject.activeSelf)
                _panelInstance.gameObject.SetActive(true);
        }

        // Complicated relationship warning!
        // This object subscribes to a toggle's value changed event, and shows/hides based on that.
        // So, if a keypress comes in, this object flips the toggle, to call that same event
        private void CheckForVisibilityToggleKeypress(float obj)
        {
            foreach (var entry in _entries)
            {
                if (Input.GetButtonDown(entry.Panel.InputAxis))
                    ToggleVisibility(entry);
            }
        }

        private void ToggleVisibility(SystemPanelEntry entry)
        {
            entry.Toggle.isOn = !entry.Toggle.isOn;
            //SetVisibility(!entry.Panel.gameObject.activeSelf, entry);
        }

        private void SetVisibility(bool isOn, SystemPanelEntry entry)
        {
            entry.Panel.Panel.gameObject.SetActive(isOn);
        }

        public string Name { get; private set; }
    }
}