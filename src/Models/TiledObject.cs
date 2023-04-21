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
    /// Represents an tiled object defined in object layers and tiles
    /// </summary>
    public class TiledObject
    {
        /// <summary>
        /// The object id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The object's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The object type if defined. Null if none was set.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The object's class
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// The object's x position in pixels
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The object's y position in pixels
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The object's rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// The object's width in pixels
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The object's height in pixels
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The tileset gid when the object is linked to a tile
        /// </summary>
        public int Gid { get; set; }

        /// <summary>
        /// A byte which stores the rotation flags of the tile linked to the object's gid.
        /// Bit 3 is horizontal flip,
        /// bit 2 is vertical flip, and
        /// bit 1 is (anti) diagonal flip.
        /// Is null when the layer is not a tilelayer.
        /// </summary>
        public byte DataRotationFlag { get; set; }

        /// <summary>
        /// An array of properties. Is null if none were defined.
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        /// If an object was set to a polygon shape, this property will be set and can be used to access the polygon's data
        /// </summary>
        public TiledPolygon Polygon { get; set; }

        /// <summary>
        /// If an object was set to a point shape, this property will be set
        /// </summary>
        public TiledPoint Point { get; set; }

        /// <summary>
        /// If an object was set to an ellipse shape, this property will be set
        /// </summary>
        public TiledEllipse Ellipse { get; set; }
    }
}
