using System;
using Assets.Narrative.Goals;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Narrative.UI
{
	public class GoalViewModel : QScript
	{
		[SerializeField] private TMP_Text _nameDisplay;
		[SerializeField] private Toggle _toggle;
		private ProductGoal _goal;

		public void Initialize(ProductGoal goal)
		{
			_goal = goal;
			OnEveryUpdate += Refresh;
		}

		private void Refresh(float obj)
		{
			_nameDisplay.text = GetDisplayName(_goal);
			_toggle.isOn = _goal.IsComplete;
		}

		private string GetDisplayName(ProductGoal goal)
		{
			switch (goal.Type)
			{
				case GoalType.CraftProduct:
					return string.Format("Produce {0}     {1} / {2}", goal.ProductName, goal.ElapsedAmount, goal.TotalAmount);
				case GoalType.AccumulateProduct:
					return string.Format("Accumulate {0}     {1} / {2}", goal.ProductName, goal.ElapsedAmount, goal.TotalAmount);
				case GoalType.SellProduct:
					return string.Format("Sell {0}     {1} / {2}", goal.ProductName, goal.ElapsedAmount, goal.TotalAmount);
				case GoalType.PlacePlaceable:
					return string.Format("Place a {0}     {1} / {2}", goal.ProductName, goal.ElapsedAmount, goal.TotalAmount);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}