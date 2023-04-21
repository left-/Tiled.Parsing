// 
// Copyright (c) 2023-2023 Luiz Tenório (left@overttime.com)
// Copyright (c) 2023-2023 Overttime Studio Co.
// All rights reserved.
// 
// This software contains confidential and proprietary information
// of Luiz Tenório (left@overttime.com)
// The user of this software agrees not to disclose, disseminate or copy such
// Confidential Information and shall use the software only in accordance with
// the terms of the license agreement the user entered into with
// Luiz Tenório (left@overttime.com).
// 

using System.Collections.Generic;
using System.IO;
using Tiled.Parsing.Models;

namespace Tiled.Parsing
{
    public static class TiledMapExtensions
    {
        /// <summary>
        /// Locates the right TiledMapTileset object for you within the Tilesets array
        /// </summary>
        /// <param name="gid">A value from the TiledLayer.data array</param>
        /// <returns>An element within the Tilesets array or null if no match was found</returns>
        public static TiledMapTileset GetTiledMapTileset(this TiledMap map, int gid)
        {
            if (map.Tilesets == null) return null;

            for (var i = 0; i < map.Tilesets.Length; i++)
                if (i < map.Tilesets.Length - 1)
                {
                    var gid1 = map.Tilesets[i + 0].FirstGid;
                    var gid2 = map.Tilesets[i + 1].FirstGid;

                    if (gid >= gid1 && gid < gid2) return map.Tilesets[i];
                }
                else
                {
                    return map.Tilesets[i];
                }

            return new TiledMapTileset();
        }

        /// <summary>
        /// Loads external tilesets and matches them to firstGids from elements within the Tilesets array
        /// </summary>
        /// <param name="src">The folder where the TiledMap file is located</param>
        /// <returns>A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset object</returns>
        public static Dictionary<int, TiledTileset> GetTiledTilesets(this TiledMap map, string src)
        {
            var tilesets = new Dictionary<int, TiledTileset>();
            var info = new FileInfo(src);
            var srcFolder = info.Directory;

            if (map.Tilesets == null) return tilesets;

            foreach (var mapTileset in map.Tilesets)
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
        public static TiledTile GetTiledTile(this TiledMap map, TiledMapTileset mapTileset, TiledTileset tileset,
            int gid)
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
        public static TiledSourceRect GetSourceRect(this TiledMap map, TiledMapTileset mapTileset, TiledTileset tileset,
            int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.FirstGid)
                {
                    var result = new TiledSourceRect
                    {
                        X = tileHor * tileset.TileWidth,
                        Y = tileVert * tileset.TileHeight,
                        Width = tileset.TileWidth,
                        Height = tileset.TileHeight
                    };

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
        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedHorizontal(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] &
                (TiledMap.FlippedHorizontallyFlag >> TiledMap.ShiftFlipFlagToByte)) > 0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped horizontally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedHorizontallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public static bool IsTileFlippedVertical(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedVertical(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public static bool IsTileFlippedVertical(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (TiledMap.FlippedVerticallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
               0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public static bool IsTileFlippedVertical(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedVerticallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedDiagonal(layer, tileHor + tileVert * layer.Width);
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (TiledMap.FlippedDiagonallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
               0;

        /// <summary>
        /// Checks is a tile linked to an object is flipped diagonally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedDiagonallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }
    }
}
