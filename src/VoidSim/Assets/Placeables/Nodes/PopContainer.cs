﻿using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using Messaging;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
    public class PopHolderMessageArgs : MessageArgs
    {
        public PopContainer PopContainer;
    }

    public interface IPopContainer
    {
        void TakePeople(IEnumerable<Person> people);
    }


    /// <summary>
    /// Provides a container for a Person's physical presence
    /// </summary>
    public class PopContainer : PlaceableNode<PopContainer>, IPopContainer
    {
        [SerializeField]
        private List<Person> _occupants;

        protected override PopContainer GetThis() { return this; }
        public const string MessageName = "PopContainerCreated";

        private int _maxCapacity;

        public void TakePeople(IEnumerable<Person> people)
        {
            _occupants = new List<Person>();
            _occupants.AddRange(people);
        }

        public void SetCapacity(int capacity)
        {
            _maxCapacity = capacity;
        }

        public override void BroadcastPlacement()
        {
            Locator.MessageHub.QueueMessage(MessageName, new PopHolderMessageArgs{ PopContainer = this });
        }

        public override string NodeName { get { return "PopContainer"; } }
    }
}