using System;

namespace Zenject.SpaceFighter
{
    public class GameEvents
    {
        public Action PlayerDied = delegate {};
        public Action EnemyKilled = delegate {};
    }
}
