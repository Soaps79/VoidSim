using System;
using System.Security;
using QGame;

namespace Assets.Placeables
{
	public interface IPlaceableNode
	{
		void BroadcastPlacement();
		string NodeName { get; }
		void HandleRemove();
		string Name { get; set; }
	}

	public abstract class PlaceableNode<T> : QScript, IDisposable, IPlaceableNode where T: PlaceableNode<T>
	{
		public Action<T> OnRemove;
		public abstract void BroadcastPlacement();
		public abstract string NodeName { get; }

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		protected abstract T GetThis();

		protected virtual void OnHandleRemove() { }

		public void HandleRemove()
		{
			OnHandleRemove();
			if (OnRemove != null)
				OnRemove(GetThis());
		}

		public void Dispose() { }
	}

	// how to handle interactions with other nodes?
	// ie: A factory placeable also has an energy consumer node
	// but when a module is added, its energy consumer should be a child of the factory's
}