using Assets.Model.Units;
using Assets.Views.Units;
using QGame;
using UnityEngine;

namespace Assets.Controllers.Units
{
    public class UnitBehavior : OrderedEventBehavior
    {
        public UnitDetails Details;
        public UnitViewDetails ViewDetails;

        private UnitMovement _movement;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        protected override void OnStart()
        {
            base.OnStart();

            // movement control
            _movement = GetComponent<UnitMovement>();

            // animation from view
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = ViewDetails.Animator;

            // renderer and sprite from view
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = ViewDetails.Sprite;

            // update bounding box from sprite
            var box = GetComponent<BoxCollider2D>();
            var size = _spriteRenderer.sprite.bounds.size;
            box.size = size;
        }
    }
}
