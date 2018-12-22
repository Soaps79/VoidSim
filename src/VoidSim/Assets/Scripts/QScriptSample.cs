using QGame;

namespace Assets.Scripts
{
    public class QScriptSample : QScript
    {
        public void Start()
        {
            // perform an action once at the beginning of the next update
            OnNextUpdate += DoAThing;

            // perform an action every update until it is removed
            OnEveryUpdate += DoAThingForAWhile;

            // perform an action on a timer
            var node = StopWatch.AddNode("Repeater", 1.0f);
            node.OnTick += DoAThingOnATimer;

            // perform an action once
            node = StopWatch.AddNode("Stop", 1.0f);
            node.OnTick += () =>
            {
                // stop an every update action
                OnEveryUpdate -= DoAThingForAWhile;
            };
        }

        private void DoAThing() { }

        private void DoAThingForAWhile() { }

        private void DoAThingOnATimer() { }
    }
}