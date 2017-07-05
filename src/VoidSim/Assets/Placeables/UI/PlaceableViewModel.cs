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

		private Placeable _placeable;

		public void Bind(Placeable placeable)
		{
			_placeable = placeable;
			HandleSubsystems();
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