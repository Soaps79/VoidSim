using System.Runtime.InteropServices;
using Assets.Model;
using Assets.View;
using UnityEngine;

namespace Assets.Controllers
{
    [CreateAssetMenu(menuName = "Player/Control/Keyboard Movement")]
    public class KeyboardMovementControl : PlayerControl
    {
        public override void UpdateCharacter(PlayerCharacter character, PlayerView view, float delta)
        {
            // check for input, calculate movement vector
            var horizontal = Input.GetAxis("Horizontal") * view.MoveSpeed * delta;
            var vertical = Input.GetAxis("Vertical") * view.MoveSpeed * delta;
            var movement = new Vector2(horizontal, vertical);

            if (character.CanMove)
            {
                // update animation
                view.Animator.SetFloat("MoveX", horizontal);
                view.Animator.SetFloat("MoveY", vertical);

                // update player character
                character.Move(movement);
            }
            else
            {
                view.Animator.SetFloat("MoveX", 0);
                view.Animator.SetFloat("MoveY", 0);
            }
        }
    }
}