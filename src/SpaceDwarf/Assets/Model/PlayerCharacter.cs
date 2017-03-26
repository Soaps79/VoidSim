using System;
using Assets.Framework;
using UnityEngine;

namespace Assets.Model
{
    public class PlayerCharacter
    {
        private Vector2 _position;
        private readonly string _name;
        private readonly string _description;
        private bool _canMove = true;

        private Action<PlayerCharacter, Vector2> _onPositionChanged;

        public string Name { get { return _name; } }

        public bool CanMove { get { return _canMove; } set { _canMove = value; } }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    var oldPosition = _position;
                    _position = value;
                    
                    // fire event
                    if (_onPositionChanged != null)
                        _onPositionChanged(this, oldPosition);
                }
            }
        }

        public string Description { get { return _description; } }

        public PlayerCharacter(Vector2 position, 
            string name = "Theo, The King",
            string description = "Regally rules robots recklessly. Twice.")
        {
            _position = position;
            _name = name;
            _description = description;
        }

        public void Move(Vector2 movement)
        {
            Position = Position.Translate(movement);
        }

        public void RegisterOnPositionChangedCallback(Action<PlayerCharacter, Vector2> callback)
        {
            _onPositionChanged += callback;
        }
    }
}
