using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.WorldMaterials
{
    /// <summary>
    /// This object manages the queueing and execution of Recipes. A driver calls QueueCrafting(), 
    /// which will start constructing if none already going, otherwise queue
    /// </summary>
    public class CraftingContainer : QScript
    {
        public CraftingContainerInfo Info;

        [Inject]
        public WorldClock WorldClock;

        private readonly List<Recipe> _recipeQueue = new List<Recipe>();
        private Recipe _currentlyCrafting;
        private const string STOPWATCH_NAME = "Crafting";

        public Action<Recipe> OnCraftingComplete;
        public Action<Recipe> OnCraftingBegin;

        public float CurrentQueueCount { get { return _recipeQueue.Count; } }
        public float CurrentCraftRemainingAsZeroToOne
        {
            get
            {
                return StopWatch.IsRunning()
                ? StopWatch[STOPWATCH_NAME].RemainingLifetimeAsZeroToOne : 0f;
            }
        }

        /// <summary>
        /// Users will call this, container will decide whether or not it can start right away.
        /// </summary>
        public void QueueCrafting(Recipe recipe)
        {
            // This func can possibly take in a transaction and hold the cost.
            // will enable refunds when player cancels a build
            _recipeQueue.Add(recipe);
            CheckForBeginCrafting();
        }

        private void CheckForBeginCrafting()
        {
            // If nothing is currently being crafted and there is something in the queue, start it.
            // Doing it next cycle to give stopwatch a chance to complete, should not be made not necessary
            if (_currentlyCrafting == null && _recipeQueue.Any())
            {
                var first = _recipeQueue.First();
                OnNextUpdate += delta => BeginCrafting(first);
                _recipeQueue.RemoveAt(0);
            }
        }

        private void BeginCrafting(Recipe recipe)
        {
            // will most likely not make it here in the first place, but jic
            if (recipe.Container.Name != Info.Name)
                throw new UnityException(string.Format("{0} was given a recipe for {1}", Info.Name, recipe.Container.Name));

            // may replace with WorldTime when it is a more flexible type
            var seconds = WorldClock.GetSeconds(recipe.TimeLength);

            // TODO: Add the node just once during initialization
            StopWatch.AddNode(STOPWATCH_NAME, seconds, true).OnTick = CompleteCraft;
            _currentlyCrafting = recipe;
            if (OnCraftingBegin != null)
                OnCraftingBegin(recipe);
        }

        private void CompleteCraft()
        {
            // tell the observers it is complete, start the next if there is one
            if (OnCraftingComplete != null)
                OnCraftingComplete(_currentlyCrafting);

            _currentlyCrafting = null;
            CheckForBeginCrafting();
        }
    }
}