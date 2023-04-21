using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Tiled.Parsing.Models;

namespace Tiled.Parsing
{
    /// <summary>
    /// Represents a Tiled map
    /// </summary>
    public class TiledMap
    {
        public const uint FlippedHorizontallyFlag = 0b10000000000000000000000000000000;
        public const uint FlippedVerticallyFlag = 0b01000000000000000000000000000000;
        public const uint FlippedDiagonallyFlag = 0b00100000000000000000000000000000;

        /// <summary>
        /// How many times we shift the FLIPPED flags to the right in order to store it in a byte.
        /// For example: 0b10100000000000000000000000000000 >> SHIFT_FLIP_FLAG_TO_BYTE = 0b00000101
        /// </summary>
        public const int ShiftFlipFlagToByte = 29;

        /// <summary>
        /// Returns the Tiled version used to create this map
        /// </summary>
        public string TiledVersion { get; set; }

        /// <summary>
        /// Returns an array of properties defined in the map
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        /// Returns an array of tileset definitions in the map
        /// </summary>
        public TiledMapTileset[] Tilesets { get; set; }

        /// <summary>
        /// Returns an array of layers or null if none were defined
        /// </summary>
        public TiledLayer[] Layers { get; set; }

        /// <summary>
        /// Returns an array of groups or null if none were defined
        /// </summary>
        public TiledGroup[] Groups { get; set; }

        /// <summary>
        /// Returns the defined map orientation as a string
        /// </summary>
        public string Orientation { get; set; }

        /// <summary>
        /// Returns the render order as a string
        /// </summary>
        public string RenderOrder { get; set; }

        /// <summary>
        /// The amount of horizontal tiles
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The amount of vertical tiles
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The tile width in pixels
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        /// The tile height in pixels
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        /// The parallax origin x
        /// </summary>
        public float ParallaxOriginX { get; set; }

        /// <summary>
        /// The parallax origin y
        /// </summary>
        public float ParallaxOriginY { get; set; }

        /// <summary>
        /// Returns true if the map is configured as infinite
        /// </summary>
        public bool Infinite { get; set; }

        /// <summary>
        /// Returns the defined map background color as a hex string
        /// </summary>
        public string BackgroundColor { get; set; }
    }
}
