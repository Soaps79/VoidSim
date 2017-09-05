using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class DebugUIHelper : QScript
	{
		public Image PositionMarker;
		public Canvas DebugCanvas;

		private static Image _positionMarker;
		private static Canvas _debugCanvas;

		public void Initialize()
		{
			_positionMarker = PositionMarker;
			_debugCanvas = DebugCanvas;
		}

		public static void PlaceMarker(Vector2 location, Color color)
		{
			var placed = Instantiate(_positionMarker, _debugCanvas.transform, false);
			placed.color = color;
			placed.transform.position = location;
			placed.gameObject.SetActive(true);
		}
	}
}