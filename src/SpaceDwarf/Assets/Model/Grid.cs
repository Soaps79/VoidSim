namespace Assets.Model
{
    public class Grid
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly Tile[,] _tiles;

        public string Name { get { return _name; } }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public Grid(string name, int width = 50, int height = 50)
        {
            _name = name;
            _width = width;
            _height = height;

            _tiles = InitializeTiles();
        }

        private Tile[,] InitializeTiles()
        {
            var tiles = new Tile[_width, _height];
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    tiles[i, j] = new Tile(this, i, j);
                }
            }
            return tiles;
        }

        public Tile GetTileAt(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
