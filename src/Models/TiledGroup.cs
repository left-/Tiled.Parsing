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
    /// Represents a layer or object group
    /// </summary>
    public class TiledGroup
    {
        /// <summary>
        /// The group's id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The group's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The group's visibility
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The group's locked state
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// The group's user properties
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        /// The group's layers
        /// </summary>
        public TiledLayer[] Layers { get; set; }

        /// <summary>
        /// The group's objects
        /// </summary>
        public TiledObject[] Objects { get; set; }

        /// <summary>
        /// The group's subgroups
        /// </summary>
        public TiledGroup[] Groups { get; set; }
    }
}
