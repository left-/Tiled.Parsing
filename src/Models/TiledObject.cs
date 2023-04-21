//
// Copyright (c) 2023 Luiz Tenorio (https://github.com/left-)
// Copyright (c) 2012-2021 Scott Chacon and others
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
