using Assets.Scripts;
using UnityEngine;

namespace Assets.Placeables.Placement
{
	public static class Placer
	{
		// Used on game load
		public static void PlaceObject(PlaceableScriptable scriptable, Vector3 position, PlaceableData data)
		{
			if (data == null)
				throw new UnityException("Placer asked to place object with no data");

			var placeable = CreatePlaceable(scriptable, position);
			placeable.HardPointName = data.HardPointName;
			placeable.InitializeNodes(data);
			QueueMessage(scriptable, placeable);
		}

		// Used when object placed during gameplay
		public static void PlaceObject(PlaceableScriptable scriptable, Vector3 position, string hardPointName)
		{
			var placeable = CreatePlaceable(scriptable, position);
			placeable.HardPointName = hardPointName;
			placeable.InitializeNodes();
			QueueMessage(scriptable, placeable);
		}

		private static void QueueMessage(PlaceableScriptable scriptable, Placeable placeable)
		{
			Locator.MessageHub.QueueMessage(
				PlaceableMessages.PlaceablePlaced,
				new PlaceableUpdateArgs
				{
					State = PlaceablePlacementState.Placed,
					Placeable = placeable,
					Layer = scriptable.Layer
				});
		}

		private static Placeable CreatePlaceable(PlaceableScriptable scriptable, Vector3 position)
		{
			var placeable = GameObject.Instantiate(scriptable.Prefab);
			placeable.transform.position = position;
			placeable.BindToScriptable(scriptable);
			return placeable;
		}
	}
}