using Assets.Station;
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
            var result = go.GetComponent<T>();
            if (result == null) result = go.AddComponent<T>();
            return result;
        }

        public static void TrimCloneFromName(this GameObject go)
        {
            go.name = go.name.Replace("(Clone)", "");
        }

        public static void RegisterSystemPanel(this GameObject go, GameObject panel)
        {
            var systemPanel = go.GetComponent<SystemPanel>();
            if(systemPanel == null)
                throw new UnityException("Attempted to register system panel with null component");

            systemPanel.Register(panel);
        }
    }
}