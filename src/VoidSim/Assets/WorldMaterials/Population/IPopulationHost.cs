namespace Assets.WorldMaterials.Population
{
	public interface IPopulationHost
	{
		float CurrentQualityOfLife { get; }
		bool PopulationWillMigrateTo(IPopulationHost otherHost);
	}
}