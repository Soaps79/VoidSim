using QGame;

namespace QGameTests
{
	public class QScriptConcrete : QScript
	{

	}

	public class LivingConcrete : ILiving
	{
		public event VoidILivingCallback AliveChanged;
		public bool IsAlive { get; set; }
	}
}