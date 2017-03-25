using Assets.Model;
using Assets.View;
using UnityEngine;

namespace Assets.Controllers
{
    public abstract class PlayerControl : ScriptableObject
    {
        public abstract void UpdateCharacter(PlayerCharacter character, PlayerView view, float delta);
    }
}