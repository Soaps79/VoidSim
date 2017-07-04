using Assets.Placeables.Nodes;
using QGame;

namespace Assets.Placeables
{
	public class PlaceableViewModel : QScript
	{
		private Placeable _placeable;

		public void Bind(Placeable placeable)
		{
			_placeable = placeable;
			HandleSubsystems();
		}

		private void HandleSubsystems()
		{
			TryPower();
		}

		private void TryPower()
		{
			var consumer = _placeable.GetComponent<EnergyConsumer>();
			if (consumer == null)
				return;
		}
	}
}