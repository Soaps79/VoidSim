using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Station.Efficiency;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class EnergyConsumerMessageArgs : MessageArgs
    {
        public EnergyConsumer EnergyConsumer;
    }

    /// <summary>
    /// Allows an object in the game world to be tied into a power system. Written in a way 
    /// to allow an object to have children objects, whose energy needs are represented by the parent.
    /// 
    /// Example: A factory has attached upgrades that have energy needs. They are made children because the 
    /// energy draw should represent the whole. If there is low energy, the whole factory should be cut off.
    /// </summary>
    [RequireComponent(typeof(Placeable))]
    [RequireComponent(typeof(EfficiencyNode))]
	public class EnergyConsumer : PlaceableNode<EnergyConsumer>
    {
	    public override string NodeName { get { return "EnergyConsumer"; } }
	    protected override EnergyConsumer GetThis() { return this; }

	    public const string MessageName = "EnergyConsumerCreated";

        [SerializeField] private float _initialValue;
	    [SerializeField] private float _weight = 1.0f;
		private EfficiencyAffector _affector;
		
		// this amount / total amount should be re-implemented once sub-modules come into play
		// at a glance, it won't work correctly, but it suits the current needs
        // represents the needs of this object and its children
        public float TotalAmountConsumed { get { return _totalAmountConsumed; } }

	    public float CurrentFulfillment
	    {
		    get { return _affector.Efficiency; }
	    }

	    // called any time the total consumption changes
        public event EventHandler OnAmountConsumedChanged;

		protected override void OnStart()
        {
			// hook into efficiency system
	        _affector = new EfficiencyAffector("Energy");
			var efficiency = GetComponent<EfficiencyNode>();
			efficiency.Module.RegisterAffector(_affector);

			// init values
            _personalAmountConsumed = _initialValue;
            UpdateTotalAmount();
        }

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

		// will be useful if consumption from sub-modules is passed on through this object
		// if not, remove this
        private float _totalAmountConsumed;
        private float _personalAmountConsumed;
        private readonly List<EnergyConsumer> _children = new List<EnergyConsumer>();

        // sums up the children and personal consumptions
        // updates totals if they have changed
        private void UpdateTotalAmount()
        {
            var total = _children.Sum(i => i.TotalAmountConsumed);
            total += _personalAmountConsumed;

	        if (!(Math.Abs(total - _totalAmountConsumed) > .01))
				return;

	        _totalAmountConsumed = total;
	        if (OnAmountConsumedChanged != null)
		        OnAmountConsumedChanged(this, null);
        }

	    public void SetCurrentEfficiency(float efficiency)
	    {
		    _affector.Efficiency = efficiency;
	    }

        // children will be added to the total, which will be kept up to date wit their changes
        public void AddChild(EnergyConsumer child)
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

        public override void BroadcastPlacement()
        {
            Locator.MessageHub.QueueMessage(MessageName, new EnergyConsumerMessageArgs { EnergyConsumer = this } );
        }
    }
}