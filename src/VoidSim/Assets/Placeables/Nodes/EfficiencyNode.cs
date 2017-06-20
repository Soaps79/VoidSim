using Assets.Station.Efficiency;
using UnityEngine;

namespace Assets.Placeables.Nodes
{
	/// <summary>
	/// Node to tie together all providers and consumers on same placeable
	/// </summary>
	public class EfficiencyNode : PlaceableNode
	{
		[SerializeField] private float _currentValue;

		public EfficiencyModule Module { get; private set; }

		void Awake()
		{
			Module = new EfficiencyModule();
			Module.OnValueChanged += UpdateModule;
		}

		private void UpdateModule(EfficiencyModule module)
		{
			_currentValue = module.CurrentAmount;
		}

		public override void BroadcastPlacement() { }

		public override string NodeName { get { return "efficency"; } }
	}
}