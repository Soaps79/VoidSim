using QGame;

namespace Assets.Scripts.Initialization
{
	public static class ServiceInitializer
	{
		/// <summary>
		/// Will initialize any services unknown to the Locator
		/// </summary>
		public static void Initialize()
		{
			InitializeLastIds();
		}

		private static void InitializeLastIds()
		{
			var existing = Locator.LastId;
			if(existing == null)
				ServiceLocator.Register<ILastIdManager>(new LastIdManager());
		}
	}
}