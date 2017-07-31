using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
    public class SystemPanelMessageArgs : MessageArgs
    {
        public SystemPanel SystemPanel;
    }

    public class SystemPanel : QScript
    {
        public const string MessageName = "SystemPanelCreated";

        public Sprite Icon;
        public string DisplayName;
        public string InputAxis;
        public GameObject Panel { get; private set; }

        public void Register(GameObject panel)
        {
            if(panel == null)
                throw new UnityException(string.Format("SystemPanel {0} given null panel", DisplayName));

            Panel = panel;
            Locator.MessageHub.QueueMessage(MessageName, new SystemPanelMessageArgs { SystemPanel = this });
        }
    }
}