using QGame;

namespace Assets.Scripts.Testing
{
    public class ConcreteScript : QScript
    {
        private int _onUpdateCount = 0;
        public int OnUpdateCount { get { return _onUpdateCount; } }

        protected override void OnAwake()
        {
            OnEveryUpdate += (delta) => _onUpdateCount++;
        }
    }
}

