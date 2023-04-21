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

namespace Tiled.Parsing.Models
{
    /// <summary>
    /// Represents a tile within a tileset
    /// </summary>
    /// <remarks>These are not defined for all tiles within a tileset, only the ones with properties, terrains and animations.</remarks>
    public class TiledTile
    {
        /// <summary>
        /// The tile id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The custom tile type, set by the user
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The custom tile class, set by the user
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// The terrain definitions as int array. These are indices indicating what part of a terrain and which terrain this tile represents.
        /// </summary>
        /// <remarks>In the map file empty space is used to indicate null or no value. However, since it is an int array I needed something so I decided to replace empty values with -1.</remarks>
        public int[] Terrain { get; set; }

        /// <summary>
        /// An array of properties. Is null if none were defined.
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        /// An array of tile animations. Is null if none were defined. 
        /// </summary>
        public TiledTileAnimation[] Animation { get; set; }

        /// <summary>
        /// An array of tile objects created using the tile collision editor
        /// </summary>
        public TiledObject[] Objects { get; set; }

        /// <summary>
        /// The individual tile image
        /// </summary>
        public TiledImage Image { get; set; }
    }
}
