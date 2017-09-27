using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Station;
using Messaging;

namespace Assets.Placeables.HardPoints
{
	/// <summary>
	/// This class exists to manage any number of HardPointGroups
	/// </summary>
	public class HardPointMonitor : IMessageListener
	{
		private readonly Dictionary<LayerType, IHardPointGroup> _groups 
			= new Dictionary<LayerType, IHardPointGroup>();

		public HardPointMonitor()
		{
			Locator.MessageHub.AddListener(this, HardPointGroup.MessageName);
			Locator.MessageHub.AddListener(this, PlaceableMessages.PlaceablePlaced);
			
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == HardPointGroup.MessageName && args != null)
				HandleGroupUpdate(args as HardPointGroupUpdateMessage);
			else if (type == PlaceableMessages.PlaceablePlaced && args != null)
				HandlePlaceable(args as PlaceableUpdateArgs);

		}

		private void HandlePlaceable(PlaceableUpdateArgs args)
		{
			if (!_groups.ContainsKey(args.Layer))
				return;

			if (args.State == PlaceablePlacementState.BeginPlacement)
			{
				_groups[args.Layer].ActivateHardpoints();
				return;
			}
			else if (args.State == PlaceablePlacementState.Placed)
				_groups[args.Layer].HandlePlacement(args.Placeable);

			_groups[args.Layer].DeactivateHardpoints();
		}

		private void HandleGroupUpdate(HardPointGroupUpdateMessage args)
		{
			if (!_groups.ContainsKey(args.Layer))
			{
				_groups.Add(args.Layer, args.Group);	
			}
				
			// else,update the relations between group and placeables
		}

		public string Name { get { return "HardPointMonitor"; } }

		public IEnumerable<HardPoint> GetHardPoints(LayerType layer)
		{
			return _groups.ContainsKey(layer) ? _groups[layer].GetAvailableHardPoints() : Enumerable.Empty<HardPoint>();
		}
	}
}