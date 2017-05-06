using QGame;

namespace Assets.Placeables.Nodes
{
    public enum ShipType
    {
        Corvette, Freighter
    }

    public class ShipBerth : QScript
    {
        public ShipType ShipType;
    }
}