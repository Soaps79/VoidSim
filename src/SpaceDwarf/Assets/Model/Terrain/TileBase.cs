namespace Assets.Model.Terrain
{
    public class TileBase
    {
        protected readonly int _x;
        protected readonly int _y;

        public virtual string Name { get { return string.Format("Tile ({0}, {1})", _x, _y); } }

        public TileBase(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}