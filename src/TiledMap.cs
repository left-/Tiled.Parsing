using Tiled.Parsing.Models;

namespace Tiled.Parsing
{
    public class TiledMap
    {
        public const uint FlippedHorizontallyFlag = 0b10000000000000000000000000000000;
        public const uint FlippedVerticallyFlag = 0b01000000000000000000000000000000;
        public const uint FlippedDiagonallyFlag = 0b00100000000000000000000000000000;
        public const int ShiftFlipFlagToByte = 29;

        public string TiledVersion { get; set; }
        public TiledProperty[] Properties { get; set; }
        public TiledMapTileset[] Tilesets { get; set; }
        public TiledLayer[] Layers { get; set; }
        public TiledGroup[] Groups { get; set; }
        public string Orientation { get; set; }
        public string RenderOrder { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public float ParallaxOriginX { get; set; }
        public float ParallaxOriginY { get; set; }
        public bool Infinite { get; set; }
        public string BackgroundColor { get; set; }
    }
}
