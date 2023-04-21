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

        public static TiledTile GetTiledTile(this TiledMap map, TiledMapTileset mapTileset, TiledTileset tileset,
            int gid)
        {
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Id == gid - mapTileset.FirstGid) return tile;
            }

            return null;
        }

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

        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedHorizontal(layer, tileHor + tileVert * layer.Width);
        }

        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] &
                (TiledMap.FlippedHorizontallyFlag >> TiledMap.ShiftFlipFlagToByte)) > 0;

        public static bool IsTileFlippedHorizontal(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedHorizontallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }

        public static bool IsTileFlippedVertical(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedVertical(layer, tileHor + tileVert * layer.Width);
        }

        public static bool IsTileFlippedVertical(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (TiledMap.FlippedVerticallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
               0;

        public static bool IsTileFlippedVertical(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedVerticallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }

        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");

            return map.IsTileFlippedDiagonal(layer, tileHor + tileVert * layer.Width);
        }

        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledLayer layer, int dataIndex)
            => (layer.DataRotationFlags[dataIndex] & (TiledMap.FlippedDiagonallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
               0;

        public static bool IsTileFlippedDiagonal(this TiledMap map, TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0) throw new TiledException("Tiled object not linked to a tile");

            return (tiledObject.DataRotationFlag & (TiledMap.FlippedDiagonallyFlag >> TiledMap.ShiftFlipFlagToByte)) >
                   0;
        }
    }
}
