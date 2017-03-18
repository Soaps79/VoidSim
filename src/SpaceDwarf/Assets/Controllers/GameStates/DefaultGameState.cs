using Assets.Model;

namespace Assets.Controllers.GameStates
{
    public class DefaultGameState : State<GameModel>
    {
        public override string Name { get { return "DefaultGameState"; } }
    }
}
