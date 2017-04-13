using QGame;

namespace Assets.Placeables
{
    public abstract class PlaceableNode : QScript
    {
        public abstract void BroadcastPlacement();
        public abstract void Initialize();
    }

    // how to handle interactions with other nodes?
    // ie: A factory placeable also has an energy consumer node
    // but when a module is added, its energy consumer should be a child of the factory's
}