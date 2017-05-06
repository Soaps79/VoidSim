using System.Collections.Generic;
using Assets.Controllers.GUI;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station.UI
{
    /// <summary>
    /// Manages the UI panels for main game systems.  Listens for SystemPanel messages.
    /// Will add the panel, and handle binding it to a visiblity toggle button and keypress
    /// </summary>
    public class SystemPanelsGroupViewModel : QScript, IMessageListener
    {
        private class SystemPanelEntry
        {
            public ToggleButtonPressBinder Binder;
            public SystemPanel Panel;
        }

        [SerializeField] private RectTransform _panelPrefab;
        [SerializeField] private Toggle _buttonPrefab;

        private RectTransform _panelInstance;
        private RectTransform _contentHolder;
        private readonly List<SystemPanelEntry> _entries = new List<SystemPanelEntry>();

        void Start()
        {
            var canvas = GameObject.Find("InfoCanvas");
            _panelInstance = Instantiate(_panelPrefab, canvas.transform, false);
            _contentHolder = _panelInstance.FindChild("content_holder").GetComponent<RectTransform>();

            MessageHub.Instance.AddListener(this, SystemPanel.MessageName);
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
            var toggle = Instantiate(_buttonPrefab, _contentHolder, false);
            toggle.isOn = args.SystemPanel.Panel.activeSelf;

            // set the icon
            var icon = toggle.transform.FindChild("icon").GetComponent<Image>();
            icon.sprite = args.SystemPanel.Icon;

            // bind the UI Toggle with the keypress, set it to trigger panel visiblity
            var binder = new GameObject().AddComponent<ToggleButtonPressBinder>();
            binder.Bind(toggle, args.SystemPanel.InputAxis);
            binder.transform.SetParent(transform);
            binder.OnToggle += (isOn) => SetVisibility(isOn, args.SystemPanel.Panel.gameObject);

            // save the reference
            var entry = new SystemPanelEntry {Panel = args.SystemPanel, Binder = binder};
            _entries.Add(entry);
        }

        private void SetVisibility(bool isOn, GameObject panel)
        {
            panel.SetActive(isOn);
        }

        public string Name { get; private set; }
    }
}