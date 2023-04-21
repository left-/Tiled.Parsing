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
    /// Represents a tile layer as well as an object layer within a tile map
    /// </summary>
    public class TiledLayer
    {
        /// <summary>
        /// The layer id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The layer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total horizontal tiles
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Total vertical tiles
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The layer type.
        /// </summary>
        public TiledLayerType Type { get; set; }

        /// <summary>
        /// The tint color set by the user in hex code
        /// </summary>
        public string Tintcolor { get; set; }

        /// <summary>
        /// Defines if the layer is visible in the editor
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Is true when the layer is locked
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// The horizontal offset
        /// </summary>
        public float OffsetX { get; set; }

        /// <summary>
        /// The vertical offset
        /// </summary>
        public float OffsetY { get; set; }

        /// <summary>
        /// The parallax x position
        /// </summary>
        public float ParallaxX { get; set; }

        /// <summary>
        /// The parallax y position
        /// </summary>
        public float ParallaxY { get; set; }

        /// <summary>
        /// The layer opacity
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// The layer class
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// An int array of gid numbers which define which tile is being used where. The length of the array equals the layer width * the layer height. Is null when the layer is not a tilelayer.
        /// </summary>
        public int[] Data { get; set; }

        /// <summary>
        /// A parallel array to data which stores the rotation flags of the tile.
        /// Bit 3 is horizontal flip,
        /// bit 2 is vertical flip, and
        /// bit 1 is (anti) diagonal flip.
        /// Is null when the layer is not a tilelayer.
        /// </summary>
        public byte[] DataRotationFlags { get; set; }

        /// <summary>
        /// The list of objects in case of an objectgroup layer. Is null when the layer has no objects.
        /// </summary>
        public TiledObject[] Objects { get; set; }

        /// <summary>
        /// The layer properties if set
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        /// The image the layer represents when the layer is an image layer
        /// </summary>
        public TiledImage Image { get; set; }

        /// <summary>
        /// The chunks of data when the map is infinite
        /// </summary>
        public TiledChunk[] Chunks { get; set; }
    }
}
