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
    /// Represents a tile layer chunk when the map is infinite
    /// </summary>
    public class TiledChunk
    {
        /// <summary>
        /// The chunk's x position
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The chunk's y position
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The chunk's width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The chunk's height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The chunk's data is similar to the data array in the TiledLayer class
        /// </summary>
        public int[] Data { get; set; }

        /// <summary>
        /// The chunk's data rotation flags are similar to the data rotation flags array in the TiledLayer class
        /// </summary>
        public byte[] DataRotationFlags { get; set; }
    }
}
