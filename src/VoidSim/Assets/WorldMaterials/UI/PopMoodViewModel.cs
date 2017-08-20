using Assets.Scripts;
using Assets.Scripts.UI;
using Assets.Station;
using Assets.Station.Efficiency;
using QGame;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.WorldMaterials.UI
{
	public class PopMoodViewModel : QScript
	{
		private PopulationControl _popControl;
		private BoundTooltipTrigger _tooltip;

		[SerializeField] private Sprite _iconLow;
		[SerializeField] private Sprite _iconMedium;
		[SerializeField] private Sprite _iconHigh;
		[SerializeField] private Sprite _iconVeryHigh;
		[SerializeField] private Image _iconImage;
		[SerializeField] private GameColors _colors;

		public void Bind(PopulationControl popControl)
		{
			_popControl = popControl;
			_popControl.MoodManager.EfficiencyModule.OnValueChanged += HandleMoodChange;
			HandleMoodChange(_popControl.MoodManager.EfficiencyModule);

			_tooltip = _iconImage.GetComponent<BoundTooltipTrigger>();
			_tooltip.OnHoverActivate += GenerateDisplayString;
		}

		private void GenerateDisplayString(BoundTooltipTrigger trigger)
		{
			trigger.text = TooltipStringGenerator.Generate(_popControl.MoodManager.EfficiencyModule, _colors);
		}

		private void HandleMoodChange(EfficiencyModule module)
		{
			var range = 1.0f - module.MinimumAmount;
			var current = module.CurrentAmount - module.MinimumAmount;

			if (current > range)
				_iconImage.sprite = _iconVeryHigh;
			else if (Mathf.Abs(current - range) < .01f)
				_iconImage.sprite = _iconHigh;
			else if (current >= range / 2)
				_iconImage.sprite = _iconMedium;
			else
				_iconImage.sprite = _iconLow;
		}
	}
}