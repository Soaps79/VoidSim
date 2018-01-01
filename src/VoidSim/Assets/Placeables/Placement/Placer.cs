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
            
            var placeable = CreatePlaceable(scriptable, position, data.InstanceName);
			placeable.HardPointName = data.HardPointName;
			placeable.InitializeNodes(data);
			QueueMessage(scriptable, placeable);
		}

		// Used when object placed during gameplay
		public static void PlaceObject(PlaceableScriptable scriptable, Vector3 position, string hardPointName)
		{
		    var objectName = scriptable.name;
		    if (objectName.ToLower().StartsWith("plc_"))
		        objectName = objectName.Remove(0, 4);
            objectName += "_" + Locator.LastId.GetNext(objectName);

            var placeable = CreatePlaceable(scriptable, position, objectName);
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

		private static Placeable CreatePlaceable(PlaceableScriptable scriptable, Vector3 position, string itsName)
		{
			var placeable = GameObject.Instantiate(scriptable.Prefab);
			placeable.transform.position = new Vector3(position.x, position.y, 1);
		    placeable.name = itsName;
			placeable.BindToScriptable(scriptable);
			return placeable;
		}
	}
}