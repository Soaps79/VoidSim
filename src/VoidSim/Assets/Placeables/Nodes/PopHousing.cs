using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopHousingMessageArgs : MessageArgs
    {
        public PopHousing PopHousing;
    }

    /// <summary>
    /// Placeholder
    /// Written as a hook for population when it comes time
    /// </summary>
    public class PopHousing : PlaceableNode
    {
        public const string MessageName = "PopHousingCreated";
        [SerializeField] private int _initialValue;

        public int Capacity { get; private set; }

        void Awake()
        {
            Capacity = _initialValue;
        }

        public override void BroadcastPlacement()
        {
            MessageHub.Instance.QueueMessage(MessageName, new PopHousingMessageArgs { PopHousing = this });
        }
    }
}