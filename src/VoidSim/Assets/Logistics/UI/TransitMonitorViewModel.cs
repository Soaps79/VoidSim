using System.Collections.Generic;
using Assets.Logistics.Ships;
using Assets.Logistics.Transit;
using Assets.Scripts;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Logistics.UI
{
	public class TransitMonitorViewModel : QScript
	{
		[SerializeField] private GameObject _entryPrefab;
		[SerializeField] private Image _panelPrefab;
		private TransitMonitor _monitor;
		
		private Dictionary<string, TransitEntryViewModel> _entries = new Dictionary<string, TransitEntryViewModel>();
		private Image _panel;
		private Image _panelContent;

		public Color TransitColor;
		public Color HoldColor;

		public void Initialize(TransitMonitor monitor)
		{
			if(monitor == null)
				throw new UnityException("TransitMonitorViewModel got bad init data");
			_monitor = monitor;

			_monitor.OnShipAdded += AddEntry;
			CreatePanel();
			gameObject.RegisterSystemPanel(_panel.gameObject);
		}

		private void CreatePanel()
		{
		    var canvas = Locator.CanvasManager.GetCanvas(CanvasType.ConstantUpdate);
			_panel = Instantiate(_panelPrefab, canvas.transform, false);
			_panelContent = _panel.transform.Find("content_holder/scroll_view/view_port/content").GetComponent<Image>();
		}

		private void AddEntry(Ship ship)
		{
			var go = Instantiate(_entryPrefab);
			go.transform.SetParent(_panelContent.transform, false);
			var entry = go.GetComponent<TransitEntryViewModel>();
			entry.HoldColor = HoldColor;
			entry.TransitColor = TransitColor;
			entry.Bind(ship);
			_entries.Add(ship.Name, entry);
		}
	}
}
