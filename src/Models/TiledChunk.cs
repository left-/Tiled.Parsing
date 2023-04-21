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
