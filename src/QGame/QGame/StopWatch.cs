using System.Linq;
using UnityEngine;

namespace QGame
{
    using System;
    using System.Collections.Generic;

    // Users will have a StopWatch object, which contains StopWatchNodes.
    // Usage is exemplified in StopWatchTest object


    public class StopWatchNode
    {
        #region Members and Ctor

        float _lifetime, _elapsedLifetime;
        bool _onlyRunOnce, _hasTicked;
        public string Name;
        internal bool IsComplete;

        public StopWatchNode(string name, float lifetime, bool onlyRunOnce = false)
        {
            Name = name;
            this._lifetime = lifetime;
            _elapsedLifetime = 0;
            _onlyRunOnce = onlyRunOnce;
        }

        #endregion
        #region Interface

        /// <summary>
        /// Will return true for one cycle after _lifetime completion
        /// </summary>
        public bool HasTicked
        {
            get
            {
                return _hasTicked;
            }
        }

        public void Pause()
        {
            _isPaused = true;
        }

        private bool _isPaused;

        public bool IsPaused { get {  return _isPaused; } }

        /// <summary>
        /// Alter the occurence of ticks. Will reset elapsed timer.
        /// </summary>
        public void ChangeLifetime(int newLifetime)
        {
            _lifetime = newLifetime;
            _elapsedLifetime = 0;
        }

        /// <summary>
        /// Events hooked in here will be triggered on each tick
        /// </summary>
        public Action OnTick;

        /// <summary>
        /// Returns how many MS until next tick
        /// </summary>
        public float RemainingLifetime
        {
            get
            {
                return _lifetime - _elapsedLifetime;
            }
        }

        public float RemainingLifetimeAsZeroToOne
        {
            get
            {
                return _elapsedLifetime / _lifetime;
            }
        }

        /// <summary>
        /// Resets object state, will also unpause
        /// </summary>
        public void Reset()
        {
            _isPaused = false;
            _hasTicked = false;
            _elapsedLifetime = 0;
            IsComplete = false;
        }

        /// <summary>
        /// Changes _lifetime and resets
        /// </summary>
        public void Reset(float lifetime)
        {
            _lifetime = lifetime;
            Reset();
        }

        #endregion
        public void UpdateElapsed(float delta)
        {
            if (_hasTicked)
            {
                _hasTicked = false;
            }

            _elapsedLifetime += delta;

            if (_elapsedLifetime > _lifetime)
            {
                HandleTick();
            }
        }

        private void HandleTick()
        {
            _hasTicked = true;
            _elapsedLifetime -= _lifetime;

            OnTick?.Invoke();

            if (_onlyRunOnce)
                IsComplete = true;
        }
    }

    public class StopWatch
    {
        List<StopWatchNode> _nodes = new List<StopWatchNode>();

        /// <summary>
        /// Updates all StopWatchNodes
        /// </summary>
        public void UpdateNodes(float delta)
        {
            var needToRemove = false;
            foreach (var node in _nodes)
            {
                if (!node.IsPaused)
                    node.UpdateElapsed(delta);

                if (node.IsComplete)
                    needToRemove = true;
            }

            if (needToRemove)
                _nodes.RemoveAll(i => i.IsComplete);
        }

        /// <summary>
        /// Accessor for individual StopWatchNodes. 
        /// <summary>
        public StopWatchNode this[string name]
        {
            get
            {
                return _nodes.FirstOrDefault(i => i.Name == name);
            }
        }

        /// <summary>
        /// Returns true if there are there any actively running nodes
        /// </summary>
        public bool IsRunning()
        {
            return _nodes.Any(i => !i.IsPaused);
        }

        /// <summary>
        /// New node will be added at index name
        /// </summary>
        public StopWatchNode AddNode(string name, float lifetime, bool onlyTickOnce = false)
        {
            StopWatchNode node;
            if (_nodes.Any(i => i.Name == name))
            {
                node = _nodes.First(i => i.Name == name);
                node.Reset(lifetime);
            }
            else
            {
                node = new StopWatchNode(name, lifetime, onlyTickOnce);
                _nodes.Add(node);
            }
            return node;
        }
    }

}