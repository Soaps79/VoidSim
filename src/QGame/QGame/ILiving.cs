using System;

namespace QGame
{
    public interface ILiving
    {
        Action<ILiving> AliveChanged { get; set; }
        bool IsAlive { get; set; }
    }
}