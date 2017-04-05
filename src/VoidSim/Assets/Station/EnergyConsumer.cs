using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;

namespace Assets.Station
{
    #region Find a better home for these
    // These will be moved and evolve alongside the Placeables system
    public static class PlaceableMessages
    {
        public const string PlaceablePlacedMessageName = "PlaceablePlaced";
    }

    public class PlaceablePlacedArgs : MessageArgs
    {
        public Placeable ObjectPlaced;
    }

    public interface IEnergyConsumer
    {
        EnergyConsumerNode EnergyConsumerNode { get; }
    }
    #endregion

    /// <summary>
    /// Allows an object in the game world to be tied into a power system. Written in a way 
    /// to allow an object to have children objects, whose energy needs are represented by the parent.
    /// 
    /// Example: A factory has attached upgrades that have energy needs. They are made children because the 
    /// energy draw should represent the whole. If there is low energy, the whole factory should be cut off.
    /// </summary>
    public class EnergyConsumerNode
    {
        // represents the needs of this object and its children
        public float TotalAmountConsumed { get { return _totalAmountConsumed; } }
        
        // called any time the total consumption changes
        public event EventHandler OnAmountConsumedChanged;

        // represents the needs of this object alone
        // Will update total if it is changed
        public float AmountConsumed
        {
            get { return _personalAmountConsumed; }
            set
            {
                if (value == _personalAmountConsumed)
                    return;

                _personalAmountConsumed = value;
                UpdateTotalAmount();
            }
        }

        private float _totalAmountConsumed;
        private float _personalAmountConsumed;
        private readonly List<EnergyConsumerNode> _children = new List<EnergyConsumerNode>();

        public EnergyConsumerNode()
        {
            UpdateTotalAmount();
        }

        // sums up the children and personal consumptions
        // updates totals if they have changed
        private void UpdateTotalAmount()
        {
            var total = _children.Sum(i => i.TotalAmountConsumed);
            total += _personalAmountConsumed;

            if (total != _totalAmountConsumed)
            {
                _totalAmountConsumed = total;
                if (OnAmountConsumedChanged != null)
                    OnAmountConsumedChanged(this, null);
            }
        }

        // children will be added to the total, which will be kept up to date wit their changes
        public void AddChild(EnergyConsumerNode child)
        {
            if (child == null || _children.Contains(child))
                return;

            child.OnAmountConsumedChanged += OnChildAmountChanged;
            _children.Add(child);
            UpdateTotalAmount();
        }

        private void OnChildAmountChanged(object sender, EventArgs e)
        {
            UpdateTotalAmount();
        }
    }
}