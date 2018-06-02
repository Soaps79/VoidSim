using Assets.Controllers.GUI;
using Assets.WorldMaterials.UI;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Logistics.UI
{
	public class TrafficShipViewModel : QScript
	{
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private InventoryGridViewModel _inventoryGrid;
		private TrafficShip _trafficShip;

		public void Bind(TrafficShip trafficShip)
		{
			_trafficShip = trafficShip;
			_nameText.text = trafficShip.name;
			_inventoryGrid.BindToInventory(trafficShip.Inventory);


			var helper = _trafficShip.gameObject.AddComponent<SelectionHelper>();
			helper.Bind(gameObject, _trafficShip.gameObject);
		}
	}
}