using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.WorldMaterials
{
    /// <summary>
    /// This object manages the queueing and crafting of Recipes. It was written alongside
    /// PlayerCraftingViewModel, which binds it to a player-facing UI. Any other actor can drive
    /// the Container using a similar interface:
    /// 
    /// QueueCrafting(Recipe recipe) - queues a build, returns its ID
    /// public Action<Recipe> OnCraftingComplete - consume the result
    /// 
    /// CancelCrafting(int recipeId)
    /// public Action<Recipe> OnCraftingCancelled - refunds goods
    /// 
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
        
        // best external usage
        public Action<Recipe> OnCraftingComplete;
        public Action<Recipe> OnCraftingCancelled;

        // could probably be ignored by an outside actor, aimed at internal UI representation
        // also encapsulates Recipe ID system
        public Action<Recipe, int> OnCraftingBegin;
        public Action<Recipe, int> OnCraftingQueued;
        public Action<Recipe, int> OnCraftingCompleteUI;
	    private string _lastIdName;

        public float CurrentQueueCount { get { return _recipeQueue.Count; } }
        public float CurrentCraftRemainingAsZeroToOne
        {
            get
            {
                return StopWatch.IsRunning()
                ? StopWatch[STOPWATCH_NAME].RemainingLifetimeAsZeroToOne : 0f;
            }
        }

	    void Awake()
	    {
		    _lastIdName = "crafting_container_" + name;
	    }

        // This, CancelCrafting() and the Recipe callbacks are the typical in-game usage
        public int QueueCrafting(Recipe recipe)
        {
            var queued = new QueuedRecipe {ID = LastIdManager.Instance.GetNext(_lastIdName), Recipe = recipe};
            _recipeQueue.Add(queued);

            if (OnCraftingQueued != null)
                OnCraftingQueued(queued.Recipe, queued.ID);

            CheckForBeginCrafting();
            return queued.ID;
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
            var seconds = WorldClock.Instance.GetSeconds(recipe.TimeLength);

            StopWatch.AddNode(STOPWATCH_NAME, seconds, true).OnTick = CompleteCraft;
            _currentlyCrafting = queuedRecipe;
            if (OnCraftingBegin != null)
                OnCraftingBegin(queuedRecipe.Recipe, queuedRecipe.ID);
        }

        private void CompleteCraft()
        {
            // tell the observers it is complete, start the next if there is one
            if (OnCraftingComplete != null)
                OnCraftingComplete(_currentlyCrafting.Recipe);

            if (OnCraftingCompleteUI != null)
                OnCraftingCompleteUI(_currentlyCrafting.Recipe, _currentlyCrafting.ID);

            _currentlyCrafting = null;
            CheckForBeginCrafting();
        }

        public void CancelCrafting(int recipeId)
        {
            Recipe recipe = null;
            if (_currentlyCrafting == null)
            {
                CheckForBeginCrafting();
                return;
            }
            
            // check whether to cancel current build or one from the queue
            if (_currentlyCrafting.ID == recipeId)
            {
                recipe = _currentlyCrafting.Recipe;
                CancelCurrentCraft();
            }
            else
            {
                var queuedRecipe = _recipeQueue.FirstOrDefault(i => i.ID == recipeId);
                if (queuedRecipe != null)
                    recipe = queuedRecipe.Recipe;

                _recipeQueue.RemoveAll(i => i.ID == recipeId);
            }

            // only broadcast if a build was actually cancelled
            if (recipe != null && OnCraftingCancelled != null)
                OnCraftingCancelled(recipe);

            CheckForBeginCrafting();
        }

        private void CancelCurrentCraft()
        {
            _currentlyCrafting = null;
            StopWatch[STOPWATCH_NAME].Reset(0);
            StopWatch[STOPWATCH_NAME].Pause();
        }

	    public int ResumeCrafting(Recipe recipe, float remaining)
	    {
		    var seconds = WorldClock.Instance.GetSeconds(recipe.TimeLength);
		    var node = StopWatch.AddNode(STOPWATCH_NAME, seconds, true);
			node.OnTick = CompleteCraft;
			node.UpdateElapsed(remaining * seconds);
		    _currentlyCrafting = new QueuedRecipe { ID = LastIdManager.Instance.GetNext(_lastIdName), Recipe = recipe };
		    if (OnCraftingBegin != null)
			    OnCraftingBegin(_currentlyCrafting.Recipe, _currentlyCrafting.ID);

		    return _currentlyCrafting.ID;
	    }
    }
}