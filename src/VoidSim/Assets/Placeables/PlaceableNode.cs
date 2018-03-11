using System;
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

    public abstract class PlaceableNode<T> : QScript, IPlaceableNode where T: PlaceableNode<T>
	{
		public Action<T> OnRemove { get; set; }
	    protected abstract T GetThis();

        protected virtual void OnHandleRemove() { }

		public void HandleRemove()
		{
			OnHandleRemove();
			if (OnRemove != null)
				OnRemove(GetThis());
		}

	    public abstract void BroadcastPlacement();
	    public abstract string NodeName { get; }

	    public string Name
	    {
	        get { return name; }
	        set { name = value; }
	    }
	}
}