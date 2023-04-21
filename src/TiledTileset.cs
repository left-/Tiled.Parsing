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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Tiled.Parsing.Commons;
using Tiled.Parsing.Models;

namespace Tiled.Parsing
{
    public class TiledTileset
    {
        public string TiledVersion { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TileCount { get; set; }
        public int Columns { get; set; }
        public TiledImage Image { get; set; }
        public int Spacing { get; set; }
        public int Margin { get; set; }
        public TiledTile[] Tiles { get; set; }
        public TiledProperty[] Properties { get; set; }
        public TiledOffset Offset { get; set; }

        public TiledTileset(string path)
        {
            if (!File.Exists(path)) throw new TiledException($"{path} not found");

            var content = File.ReadAllText(path);

            if (path.EndsWith(".tsx"))
                ParseXml(content);
            else
                throw new TiledException("Unsupported file format");
        }

        public TiledTileset(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            ParseXml(content);
        }

        public void ParseXml(string xml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);

                var nodeTileset = document.SelectSingleNode("tileset");
                var nodeImage = nodeTileset.SelectSingleNode("image");
                var nodeOffset = nodeTileset.SelectSingleNode("tileoffset");
                var nodesTile = nodeTileset.SelectNodes("tile");
                var nodesProperty = nodeTileset.SelectNodes("properties/property");

                var attrMargin = nodeTileset.Attributes["margin"];
                var attrSpacing = nodeTileset.Attributes["spacing"];
                var attrClass = nodeTileset.Attributes["class"];

                TiledVersion = nodeTileset.Attributes["tiledversion"].Value;
                Name = nodeTileset.Attributes["name"]?.Value;
                TileWidth = int.Parse(nodeTileset.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeTileset.Attributes["tileheight"].Value);
                TileCount = int.Parse(nodeTileset.Attributes["tilecount"].Value);
                Columns = int.Parse(nodeTileset.Attributes["columns"].Value);

                if (attrMargin != null) Margin = int.Parse(nodeTileset.Attributes["margin"].Value);
                if (attrSpacing != null) Spacing = int.Parse(nodeTileset.Attributes["spacing"].Value);
                if (attrClass != null) Class = attrClass.Value;
                if (nodeImage != null) Image = ParseImage(nodeImage);
                if (nodeOffset != null) Offset = ParseOffset(nodeOffset);

                Tiles = ParseTiles(nodesTile);
                Properties = ParseProperties(nodesProperty);
            }
            catch (Exception ex)
            {
                throw new TiledException("An error occurred while trying to parse the Tiled tileset file", ex);
            }
        }

        private TiledOffset ParseOffset(XmlNode node)
        {
            var tiledOffset = new TiledOffset();
            tiledOffset.X = int.Parse(node.Attributes["x"].Value);
            tiledOffset.Y = int.Parse(node.Attributes["y"].Value);

            return tiledOffset;
        }

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.Source = node.Attributes["source"].Value;
            tiledImage.Width = int.Parse(node.Attributes["width"].Value);
            tiledImage.Height = int.Parse(node.Attributes["height"].Value);

            return tiledImage;
        }

        private TiledTileAnimation[] ParseAnimations(XmlNodeList nodeList)
        {
            var result = new List<TiledTileAnimation>();

            foreach (XmlNode node in nodeList)
            {
                var animation = new TiledTileAnimation();
                animation.TileId = int.Parse(node.Attributes["tileid"].Value);
                animation.Duration = int.Parse(node.Attributes["duration"].Value);

                result.Add(animation);
            }

            return result.ToArray();
        }

        private TiledProperty[] ParseProperties(XmlNodeList nodeList)
        {
            var result = new List<TiledProperty>();

            foreach (XmlNode node in nodeList)
            {
                var attrType = node.Attributes["type"];

                var property = new TiledProperty();
                property.Name = node.Attributes["name"].Value;
                property.Value = node.Attributes["value"]?.Value;
                property.Type = TiledPropertyType.String;

                if (attrType != null)
                {
                    if (attrType.Value == "bool") property.Type = TiledPropertyType.Bool;
                    if (attrType.Value == "color") property.Type = TiledPropertyType.Color;
                    if (attrType.Value == "file") property.Type = TiledPropertyType.File;
                    if (attrType.Value == "float") property.Type = TiledPropertyType.Float;
                    if (attrType.Value == "int") property.Type = TiledPropertyType.Int;
                    if (attrType.Value == "object") property.Type = TiledPropertyType.Object;
                }

                if (property.Value == null) property.Value = node.InnerText;

                result.Add(property);
            }

            return result.ToArray();
        }

        private TiledTile[] ParseTiles(XmlNodeList nodeList)
        {
            var result = new List<TiledTile>();

            foreach (XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodesObject = node.SelectNodes("objectgroup/object");
                var nodesAnimation = node.SelectNodes("animation/frame");
                var nodeImage = node.SelectSingleNode("image");

                var tile = new TiledTile();
                tile.Id = int.Parse(node.Attributes["id"].Value);
                tile.Class = node.Attributes["class"]?.Value;
                tile.Type = node.Attributes["type"]?.Value;
                tile.Terrain = node.Attributes["terrain"]?.Value.Split(',').AsIntArray();
                tile.Properties = ParseProperties(nodesProperty);
                tile.Animation = ParseAnimations(nodesAnimation);
                tile.Objects = ParseObjects(nodesObject);

                if (nodeImage != null)
                {
                    var tileImage = new TiledImage();
                    tileImage.Width = int.Parse(nodeImage.Attributes["width"].Value);
                    tileImage.Height = int.Parse(nodeImage.Attributes["height"].Value);
                    tileImage.Source = nodeImage.Attributes["source"].Value;

                    tile.Image = tileImage;
                }

                result.Add(tile);
            }

            return result.ToArray();
        }

        private TiledObject[] ParseObjects(XmlNodeList nodeList)
        {
            var result = new List<TiledObject>();

            foreach (XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodePolygon = node.SelectSingleNode("polygon");
                var nodePoint = node.SelectSingleNode("point");
                var nodeEllipse = node.SelectSingleNode("ellipse");

                var obj = new TiledObject();
                obj.Id = int.Parse(node.Attributes["id"].Value);
                obj.Name = node.Attributes["name"]?.Value;
                obj.Class = node.Attributes["class"]?.Value;
                obj.Type = node.Attributes["type"]?.Value;
                obj.Gid = int.Parse(node.Attributes["gid"]?.Value ?? "0");
                obj.X = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
                obj.Y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (nodesProperty != null) obj.Properties = ParseProperties(nodesProperty);

                if (nodePolygon != null)
                {
                    var points = nodePolygon.Attributes["points"].Value;
                    var vertices = points.Split(' ');

                    var polygon = new TiledPolygon();
                    polygon.Points = new float[vertices.Length * 2];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        polygon.Points[i * 2 + 0] =
                            float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
                        polygon.Points[i * 2 + 1] =
                            float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
                    }

                    obj.Polygon = polygon;
                }

                if (nodeEllipse != null) obj.Ellipse = new TiledEllipse();

                if (nodePoint != null) obj.Point = new TiledPoint();

                if (node.Attributes["width"] != null)
                    obj.Width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["height"] != null)
                    obj.Height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["rotation"] != null)
                    obj.Rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);

                result.Add(obj);
            }

            return result.ToArray();
        }
    }
}
