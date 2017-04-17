using QGame;

namespace Assets.Scripts.Testing
{
    public class ConcreteScript : QScript
    {
        public void MockUpdate(float delta)
        {
            OnUpdate(delta);
        }
    }
}

