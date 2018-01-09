using System.Collections.Generic;
using System.Linq;
using Assets.Controllers.GUI;
using Assets.Placeables.Placement;
using Assets.Placeables.UI;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Station;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Placeables
{

	// These will be moved and evolve alongside the Placeables system
	public static class PlaceableMessages
	{
		public const string PlaceablePlaced = "PlaceablePlaced";
	}

	public enum PlaceablePlacementState { BeginPlacement, Placed, Cancelled, Removed }

	public class PlaceableUpdateArgs : MessageArgs
	{
		public PlaceablePlacementState State;
		public Placeable Placeable;
		public LayerType Layer;
	}

	public class PlaceableData
	{
		public string PlaceableName;
		public string InstanceName;
		public string HardPointName;
		public Vector3Data Position;
		public List<PlaceableNodeData> Nodes;
	}

	public class PlaceableNodeData
	{
		public string NodeName;
		public string InstanceName;
	}

	/// <summary>
	/// Represents any structure or module or any other object placed into the game world.
	/// </summary>
	[RequireComponent(typeof(PolygonCollider2D))]
	public class Placeable : QScript, ISerializeData<PlaceableData>, IPointerClickHandler
	{
		[HideInInspector] public LayerType Layer;
		public string PlaceableName { get { return _scriptable.ProductName; } }
		public string HardPointName { get; set; }
        
		private PlaceableScriptable _scriptable;
		private List<IPlaceableNode> _nodes;
		private PlaceableViewModel _viewModelInstance;

		public void BindToScriptable(PlaceableScriptable scriptable)
		{
			_scriptable = scriptable;
			Layer = scriptable.Layer;
		    
			gameObject.TrimCloneFromName();
			var rend = gameObject.GetComponent<SpriteRenderer>();
			rend.sortingLayerName = Layer.ToString();
			rend.sortingOrder = 1;
		}

		public void InitializeNodes(PlaceableData data = null)
		{
			_nodes = GetComponents<IPlaceableNode>().ToList();
			foreach (var node in _nodes)
			{
				node.BroadcastPlacement();
			}
		}

		public PlaceableData GetData()
		{
			return new PlaceableData
			{
				InstanceName = name,
				PlaceableName = PlaceableName,
				Position = transform.position,
				HardPointName = HardPointName,
				Nodes = _nodes.Select(i => new PlaceableNodeData
				{
					NodeName = i.NodeName, InstanceName = i.Name
				}).ToList()
			};
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (UserPlacement.RemoveIsActive)
			{
				Locator.MessageHub.QueueMessage(
					PlaceableMessages.PlaceablePlaced, 
					new PlaceableUpdateArgs
					{
						Layer = Layer,
						Placeable = this,
						State = PlaceablePlacementState.Removed
					});

				_nodes.ForEach(i => i.HandleRemove());
			}
			else
			{
				PlaceableUIFactory.ToggleUI(this);
			}
		}
	}
}