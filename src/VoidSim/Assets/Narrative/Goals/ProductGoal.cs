using System;
using Assets.Narrative.Missions;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;

namespace Assets.Narrative.Goals
{
	[Serializable]
	public enum GoalType
	{
		CraftProduct,
		AccumulateProduct,
		SellProduct
	}

	/// <summary>
	/// Content for the mission is handled in the ProductGoalInfo class.
	/// This one holds the progress of the goal, and enough data to match it back up with the content after load
	/// </summary>
	public class ProductGoalProgressData
	{
		public GoalType Type;
		public bool IsComplete;
		public bool IsActive;
		public int ElapsedAmount;
	}

	// currently, all goals deal with products, write new code knowing this probably won't always be true
	[Serializable]
	public class ProductGoal : ISerializeData<ProductGoalProgressData>
	{
		public GoalType Type;
		public bool IsComplete;
		public bool IsActive;
		public Action<ProductGoal> OnCompleteChange;

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

		public ProductGoal() { }

		public ProductGoal(ProductGoalInfo info)
		{
			ProductName = info.ProductName;
			ProductId = info.ProductId;
			TotalAmount = info.TotalAmount;
			Type = info.Type;
		}

		public void SetFromData(ProductGoalProgressData progressData)
		{
			IsActive = progressData.IsActive;
			ElapsedAmount = progressData.ElapsedAmount;

			if (progressData.IsComplete)
				TriggerComplete(true); 
		}

		public ProductGoalProgressData GetData()
		{
			return new ProductGoalProgressData
			{
				Type = Type,
				IsComplete = IsComplete,
				IsActive = IsActive,
				ElapsedAmount = ElapsedAmount
			};
		}
	}
}