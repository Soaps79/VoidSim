using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework
{
    public static class Vector2Extensions
    {
        public const float DefaultZAxisCoord = 0;

        public static Vector2 Translate(this Vector2 u, Vector2 translation)
        {
            return new Vector2(u.x + translation.x, u.y + translation.y);
        }

        public static Vector3 ToVector3(this Vector2 u)
        {
            return new Vector3(u.x, u.y, DefaultZAxisCoord);
        }
    }

    public static class Vector3Extensions
    {
        public static Vector2 ToVector2(this Vector3 u)
        {
            return new Vector2(u.x, u.y);
        }
    }

    public static class MonoBehaviourExtentions
    {
        /// <summary>
        /// from: http://wiki.unity3d.com/index.php/GetOrAddComponent
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            var result = child.GetComponent<T>() ?? child.gameObject.AddComponent<T>();
            return result;
        }
    }
    
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

    public static class DictionaryExtensions
    {
        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }

}
