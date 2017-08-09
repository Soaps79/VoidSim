namespace Assets.WorldMaterials.Population
{
	// First pass at this interface, should evolve with the Population system
	public interface IPopulationHost
	{
		float CurrentQualityOfLife { get; }
		bool PopulationWillMigrateTo(IPopulationHost otherHost);
	}
}