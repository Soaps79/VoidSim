using Assets.Placeables.Nodes;
using Assets.Placeables.UI;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Placeables
{
	public class PlaceableViewModel : QScript
	{
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private RectTransform _statsHolder;

		[SerializeField] private EnergyConsumerViewModel _energyPrefab;
		[SerializeField] private Color _linecolor;
		[SerializeField] private float _lineWidth;
		private LineRenderer _line;

		private Placeable _placeable;

		public void Bind(Placeable placeable)
		{
			_placeable = placeable;
			HandleSubsystems();
			//GenerateLine();
			//UpdateLine();
		}

		private void GenerateLine()
		{
			//var go = new GameObject();
			//go.transform.SetParent(transform);
			_line = GetComponent<LineRenderer>();
			_line.sortingLayerName = "GameUI";
			//_line.endColor = _linecolor;
			//_line.startColor = _linecolor;
			_line.startWidth = _lineWidth;
			_line.endWidth = _lineWidth;
		}

		private void UpdateLine()
		{
			var position = Camera.main.ScreenToWorldPoint(transform.position);
			_line.SetPositions(new Vector3[]{ position, _placeable.transform.position});
		}

		private void HandleSubsystems()
		{
			_nameText.text = _placeable.PlaceableName;
			TryPower();
		}

		private void TryPower()
		{
			var consumer = _placeable.GetComponent<EnergyConsumer>();
			if (consumer == null)
				return;

			var energy = Instantiate(_energyPrefab, _statsHolder);
			energy.Bind(consumer);
		}
	}
}