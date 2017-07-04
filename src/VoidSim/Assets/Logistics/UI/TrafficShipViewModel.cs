using QGame;
using TMPro;
using UnityEngine;

namespace Assets.Logistics.UI
{
	public class TrafficShipViewModel : QScript
	{
		[SerializeField] private TMP_Text _nameText;
		private TrafficShip _trafficShip;

		public void Bind(TrafficShip trafficShip)
		{
			_trafficShip = trafficShip;
			_nameText.text = trafficShip.name;
		}
	}
}