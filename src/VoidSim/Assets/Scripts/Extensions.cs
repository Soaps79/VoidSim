using UnityEngine;

namespace Assets.Scripts
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Modelled after <see cref="Component"/> GetOrAddComponent{T}
        /// 
        /// Will return one of the default (and unlisted in editor)
        /// game components on the game object.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var result = go.GetComponent<T>() ?? go.AddComponent<T>();
            return result;
        }
    }
}