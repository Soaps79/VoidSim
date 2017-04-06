// ReSharper disable InconsistentNaming
namespace Assets.HexGrid.Scripts
{
    public enum HexDirection
    {
        NE = 0,
        E = 1,
        SE = 2,
        SW = 3,
        W = 4,
        NW = 5
    }

    public static class HexDirectionExtensions
    {
        /// <summary>
        /// Returns the opposite direction
        /// </summary>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }

        /// <summary>
        /// Returns the counter-clockwise direction.
        /// </summary>
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        /// <summary>
        /// Returns the clockwise direction
        /// </summary>
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }

        public static HexDirection Previous2(this HexDirection direction)
        {
            direction -= 2;
            return direction >= HexDirection.NE ? direction : (direction + 6);
        }

        public static HexDirection Next2(this HexDirection direction)
        {
            direction += 2;
            return direction <= HexDirection.NW ? direction : (direction - 6);
        }
    }
}