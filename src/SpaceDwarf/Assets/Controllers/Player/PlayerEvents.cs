using Assets.Controllers.GUI;
using Assets.Model;
using Assets.View;
using UnityEngine;

namespace Assets.Controllers.Player
{
    public class PlayerEvents
    {
        public static void SetTooltip(PlayerView view, PlayerCharacter character)
        {
            var label = character.Name;
            var flavor = character.Description;
            var thumbnail = view.CharacterPrefab.GetComponent<SpriteRenderer>().sprite;
            var position = character.Position;
            TooltipController.Instance.SetTooltip(label, flavor, thumbnail, position);
        }
    }
}
