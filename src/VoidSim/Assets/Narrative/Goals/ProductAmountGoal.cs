using System;

namespace Assets.Narrative.Goals
{
	[Serializable]
	public enum GoalType
	{
		CraftProduct, AccumulateProduct, SellProduct
	}

	// currently, all goals deal with products, write new code knowing this probably won't always be true
	[Serializable]
	public class ProductAmountGoal
	{
		public GoalType Type;
		public bool IsComplete;
		public bool IsActive;
		public Action<ProductAmountGoal> OnCompleteChange;

		public string ProductName;
		public int ProductId;
		public int TotalAmount;
		public int ElapsedAmount;

		// if change is true, set IsComplete to it and tell listeners
		public void TriggerComplete(bool isComplete)
		{
			if (IsComplete == isComplete)
				return;

			IsComplete = isComplete;
			if (OnCompleteChange != null)
				OnCompleteChange(this);
		}
	}
}