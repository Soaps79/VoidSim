using Messaging;
using QGame;
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

    //public static class LocatorExtensions
    //{
    //    private static IMessageHub _messageHub;
    //    public static IMessageHub Messages(this Locator loc)
    //    {
    //        return _messageHub ?? (_messageHub = Locator.Get<IMessageHub>());
    //    }

    //    private static IKeyValueDisplay _valueDisplay;
    //    public static IKeyValueDisplay ValueDisplay(this Locator loc)
    //    {
    //        return _valueDisplay ?? (_valueDisplay = Locator.Get<IKeyValueDisplay>());
    //    }
    //}
}