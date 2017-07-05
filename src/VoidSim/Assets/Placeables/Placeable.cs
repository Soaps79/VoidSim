using System.Collections.Generic;
using System.Linq;
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

	public class PlaceableUpdateArgs : MessageArgs
	{
		public Placeable ObjectPlaced;
		public LayerType Layer;
		public bool WasRemoved;
	}

	public class PlaceableData
	{
		public string PlaceableName;
		public string InstanceName;
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
	public class Placeable : QScript, ISerializeData<PlaceableData>, IPointerClickHandler
	{
		[HideInInspector] public LayerType Layer;
		public string PlaceableName { get { return _scriptable.ProductName; } }
		private PlaceableScriptable _scriptable;
		private List<PlaceableNode> _nodes;

		public void BindToScriptable(PlaceableScriptable scriptable)
		{
			_scriptable = scriptable;
			Layer = scriptable.Layer;

			gameObject.TrimCloneFromName();
			var rend = gameObject.GetComponent<SpriteRenderer>();
			//rend.enabled = true;
			//rend.sprite = scriptable.PlacedSprite;
			rend.sortingLayerName = Layer.ToString();
			rend.sortingOrder = 1;
		}

		public void InitializeNodes(PlaceableData data = null)
		{
			_nodes = GetComponents<PlaceableNode>().ToList();
			foreach (var node in _nodes)
			{
				if (data != null && data.Nodes != null && data.Nodes.Any(i => i.NodeName == node.NodeName))
				{
					node.name = data.Nodes.First(i => i.NodeName == node.NodeName).InstanceName;
				}
				else
				{
					node.name = PlaceableNode.DefaultName;
				}
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
				Nodes = _nodes.Select(i => new PlaceableNodeData
				{
					NodeName = i.NodeName, InstanceName = i.name
				}).ToList()
			};
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			// create view model
			var canvas = GameObject.Find("InfoCanvas");
			var viewmodel = Instantiate(_scriptable.ViewModel, canvas.transform, false);
			viewmodel.Bind(this);
		}
	}
}