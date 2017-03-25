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
        // for use internally and for UI callbacks
        public class QueuedRecipe
        {
            public int ID;
            public Recipe Recipe;
        }

        public CraftingContainerInfo Info;
        public WorldClock WorldClock;

        private readonly List<QueuedRecipe> _recipeQueue = new List<QueuedRecipe>();
        private QueuedRecipe _currentlyCrafting;
        private const string STOPWATCH_NAME = "Crafting";
        private int _lastId;

        public Action<Recipe> OnCraftingComplete;
        public Action<QueuedRecipe> OnCraftingBegin;
        public Action<QueuedRecipe> OnCraftingQueued;
        public Action<Recipe> OnCraftingCancelled;
        // written so game-side consumers don't have to know what a QueuedRecipe is
        public Action<QueuedRecipe> OnCraftingCompleteUI;

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
            _lastId++;
            var queued = new QueuedRecipe {ID = _lastId, Recipe = recipe};
            _recipeQueue.Add(queued);

            if (OnCraftingQueued != null)
                OnCraftingQueued(queued);

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

        private void BeginCrafting(QueuedRecipe queuedRecipe)
        {
            var recipe = queuedRecipe.Recipe;

            // will most likely not make it here in the first place, but jic
            if (recipe.Container.Name != Info.Name)
                throw new UnityException(string.Format("{0} was given a recipe for {1}", Info.Name, recipe.Container.Name));

            // may replace with WorldTime when it is a more flexible type
            var seconds = WorldClock.GetSeconds(recipe.TimeLength);

            // TODO: Add the node just once during initialization
            StopWatch.AddNode(STOPWATCH_NAME, seconds, true).OnTick = CompleteCraft;
            _currentlyCrafting = queuedRecipe;
            if (OnCraftingBegin != null)
                OnCraftingBegin(queuedRecipe);
        }

        private void CompleteCraft()
        {
            // tell the observers it is complete, start the next if there is one
            if (OnCraftingComplete != null)
                OnCraftingComplete(_currentlyCrafting.Recipe);

            if (OnCraftingCompleteUI != null)
                OnCraftingCompleteUI(_currentlyCrafting);

            _currentlyCrafting = null;
            CheckForBeginCrafting();
        }

        public void CancelCrafting(QueuedRecipe recipe)
        {
            // cancels craft, tells observers, see if a new craft should start
            if (_currentlyCrafting.ID == recipe.ID)
            {
                CancelCurrentCraft();
            }
            else
            {
                _recipeQueue.RemoveAll(i => i.ID == recipe.ID);
            }

            if (OnCraftingCancelled != null)
                OnCraftingCancelled(recipe.Recipe);

            CheckForBeginCrafting();
        }

        private void CancelCurrentCraft()
        {
            _currentlyCrafting = null;
            StopWatch[STOPWATCH_NAME].Reset(0);
            StopWatch[STOPWATCH_NAME].Pause();
        }
    }
}