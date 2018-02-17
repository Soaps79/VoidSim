using Assets.Scripts.Serialization;
using Assets.Station;
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

	public static class Vector3Extensions
	{
		public static Vector2 ToVector2(this Vector3 me)
		{
			return new Vector2(me.x, me.y);
		}
	}

	public static class Locator
	{
		public static ILastIdManager LastId
		{
			get { return ServiceLocator.Get<ILastIdManager>(); }
		}

		public static ISerializationHub Serialization
		{
			get { return ServiceLocator.Get<ISerializationHub>(); }
		}

		public static IMessageHub MessageHub
		{
			get { return ServiceLocator.Get<IMessageHub>(); }
		}

		public static IWorldClock WorldClock
		{
			get { return ServiceLocator.Get<IWorldClock>(); }
		}

	    public static ICanvasManager CanvasManager
	    {
	        get { return ServiceLocator.Get<ICanvasManager>(); }
	    }
    }
}