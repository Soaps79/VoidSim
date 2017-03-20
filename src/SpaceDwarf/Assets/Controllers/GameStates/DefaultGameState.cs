using Assets.Model;
using UnityEngine;

namespace Assets.Controllers.GameStates
{
    public class DefaultGameState : State<GameModel>
    {
        public override string Name { get { return "DefaultGameState"; } }

        private readonly State<GameModel> _selectionState;

        public DefaultGameState(SelectionGameState selectionState)
        {
            _selectionState = selectionState;
        }

        public override void Execute(GameModel owner, float timeDelta)
        {
            base.Execute(owner, timeDelta);

            if (Input.GetButtonDown("SelectionMode"))
            {
                Machine.ChangeState(_selectionState);
            }
        }
    }
}
