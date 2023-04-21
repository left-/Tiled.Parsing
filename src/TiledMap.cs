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

        /// <summary>
        /// Returns an empty instance of TiledMap
        /// </summary>
        public TiledMap()
        {
        }

        /* HELPER METHODS */
        /// <summary>
        /// Locates the right TiledMapTileset object for you within the Tilesets array
        /// </summary>
        /// <param name="gid">A value from the TiledLayer.data array</param>
        /// <returns>An element within the Tilesets array or null if no match was found</returns>
        public TiledMapTileset GetTiledMapTileset(int gid)
        {
            if (Tilesets == null) return null;

            for (var i = 0; i < Tilesets.Length; i++)
                if (i < Tilesets.Length - 1)
                {
                    var gid1 = Tilesets[i + 0].FirstGid;
                    var gid2 = Tilesets[i + 1].FirstGid;

                    if (gid >= gid1 && gid < gid2) return Tilesets[i];
                }
                else
                {
                    return Tilesets[i];
                }

            return new TiledMapTileset();
        }

        /// <summary>
        /// Loads external tilesets and matches them to firstGids from elements within the Tilesets array
        /// </summary>
        /// <param name="src">The folder where the TiledMap file is located</param>
        /// <returns>A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset object</returns>
        public Dictionary<int, TiledTileset> GetTiledTilesets(string src)
        {
            var tilesets = new Dictionary<int, TiledTileset>();
            var info = new FileInfo(src);
            var srcFolder = info.Directory;

            if (Tilesets == null) return tilesets;

            foreach (var mapTileset in Tilesets)
            {
                var path = $"{srcFolder}/{mapTileset.Source}";

                if (mapTileset.Source == null) continue;

                if (File.Exists(path))
                    tilesets.Add(mapTileset.FirstGid, new TiledTileset(path));
                else
                    throw new TiledException("Cannot locate tileset '" + path +
                                             "'. Please make sure the source folder is correct and it ends with a slash.");
            }

            return tilesets;
        }

        /// <summary>
        /// Locates a specific TiledTile object
        /// </summary>
        /// <param name="mapTileset">An element within the Tilesets array</param>
        /// <param name="tileset">An instance of the TiledTileset class</param>
        /// <param name="gid">An element from within a TiledLayer.data array</param>
        /// <returns>An entry of the TiledTileset.tiles array or null if none of the tile id's matches the gid</returns>
        /// <remarks>Tip: Use the GetTiledMapTileset and GetTiledTilesets methods for retrieving the correct TiledMapTileset and TiledTileset objects</remarks>
        public TiledTile GetTiledTile(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Id == gid - mapTileset.FirstGid) return tile;
            }

            return null;
        }

        /// <summary>
        /// This method can be used to figure out the source rect on a Tileset image for rendering tiles.
        /// </summary>
        /// <param name="mapTileset"></param>
        /// <param name="tileset"></param>
        /// <param name="gid"></param>
        /// <returns>An instance of the class TiledSourceRect that represents a rectangle. Returns null if the provided gid was not found within the tileset.</returns>
        public TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.FirstGid)
                {
                    var result = new TiledSourceRect();
                    result.X = tileHor * tileset.TileWidth;
                    result.Y = tileVert * tileset.TileHeight;
                    result.Width = tileset.TileWidth;
                    result.Height = tileset.TileHeight;

                    return result;
                }

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.Width / tileset.TileWidth)
                {
                    tileHor = 0;
                    tileVert++;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return IsTileFlippedHorizontal(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (FlippedHorizontallyFlag >> ShiftFlipFlagToByte)) > 0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped horizontally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (FlippedHorizontallyFlag >> ShiftFlipFlagToByte)) > 0;
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return IsTileFlippedVertical(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (FlippedVerticallyFlag >> ShiftFlipFlagToByte)) > 0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedVertical(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (FlippedVerticallyFlag >> ShiftFlipFlagToByte)) > 0;
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return IsTileFlippedDiagonal(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (FlippedDiagonallyFlag >> ShiftFlipFlagToByte)) > 0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped diagonally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (FlippedDiagonallyFlag >> ShiftFlipFlagToByte)) > 0;
        }
    }
}
