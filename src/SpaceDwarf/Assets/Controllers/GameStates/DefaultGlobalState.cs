using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class DefaultGlobalState : State<GameModel>
    {
        public override string Name { get { return "DefaultGlobalState"; } }

        private readonly State<GameModel> _pauseState;

        public DefaultGlobalState(PauseGameState pauseGameState)
        {
            _pauseState = pauseGameState;
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            if (Input.GetButtonDown("Pause"))
            {
                Machine.ChangeState(_pauseState);
            }
        }
    }
}