using System;
using Assets.Framework;
using UnityEngine;

namespace Assets.Model
{
    public class PlayerCharacter
    {
        private Vector2 _position;
        private readonly string _name;
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

        public PlayerCharacter(Vector2 position, string name = "PlayerCharacter")
        {
            _position = position;
            _name = name;
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
