using Assets.Framework;
using Assets.Model;
using Assets.View;
using UnityEngine;
// ReSharper disable PossibleInvalidOperationException
// Reason: Always escaping early.

namespace Assets.Controllers
{
    [CreateAssetMenu(menuName = "Player/Control/Click to Move")]
    public class ClickToMoveControl : PlayerControl
    {
        public int MouseButton = 0;
        public float ArriveRadius = 0.5f;

        private Vector3? _destination = null;
        public override void UpdateCharacter(PlayerCharacter character, PlayerView view, float delta)
        {
            if (!character.CanMove)
            {
                view.Animator.SetFloat("MoveX", 0);
                view.Animator.SetFloat("MoveY", 0);
                return;
            }

            // check for input
            if (Input.GetMouseButton(MouseButton) || Input.GetMouseButtonDown(MouseButton))
            {
                //mouse down, figure out where and set the destination
                var worldPosition = CameraController.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
                _destination = worldPosition;
            }

            if (_destination == null)
            {
                return;
            }

            // have we arrived?
            if (HasArrived(character.Position, _destination, ArriveRadius))
            {
                _destination = null;
                return;
            }

            // determine how far to move this frame and apply
            var direction = _destination.Value.ToVector2() - character.Position;
            var movement = direction.normalized * view.MoveSpeed * delta;
            
            // update animation
            view.Animator.SetFloat("MoveX", movement.x);
            view.Animator.SetFloat("MoveY", movement.y);

            // update character
            character.Move(movement);
        }

        private static bool HasArrived(Vector2 position, Vector3? dest, float arriveRadius)
        {
            var distance = dest.Value.ToVector2() - position;
            var hasArrived = distance.magnitude <= arriveRadius;
            return hasArrived;
        }
    }
}