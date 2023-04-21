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
using System.IO.Compression;
using System.Xml;
using Tiled.Parsing.Models;

namespace Tiled.Parsing
{
    public class TiledMapParser
    {
        /// <summary>
        /// Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="stream">Stream of opened tmx file</param>
        /// <exception cref="TiledException">Thrown when the map could not be loaded</exception>
        public TiledMap Parse(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            return ParseXml(content);
        }

        /// <summary>
        /// Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="path">The path to the tmx file</param>
        /// <exception cref="ArgumentException">Thrown when the map could not be loaded</exception>
        /// <exception cref="TiledException">Thrown when the map could not be loaded or is not in a correct format</exception>
        public TiledMap Parse(string path)
        {
            if (!File.Exists(path)) throw new ArgumentException($"{path} not found");

            var content = File.ReadAllText(path);
            return path.EndsWith(".tmx")
                ? ParseXml(content)
                : throw new TiledException("Unsupported file format");
        }


        /// <summary>
        /// Can be used to parse the content of a TMX map manually instead of loading it using the constructor
        /// </summary>
        /// <param name="xml">The tmx file content as string</param>
        /// <exception cref="TiledException"></exception>
        public TiledMap ParseXml(string xml)
        {
            var map = new TiledMap();

            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);

                var nodeMap = document.SelectSingleNode("map");
                var nodesProperty = nodeMap.SelectNodes("properties/property");
                var nodesLayer = nodeMap.SelectNodes("layer");
                var nodesImageLayer = nodeMap.SelectNodes("imagelayer");
                var nodesObjectGroup = nodeMap.SelectNodes("objectgroup");
                var nodesTileset = nodeMap.SelectNodes("tileset");
                var nodesGroup = nodeMap.SelectNodes("group");
                var attrParallaxOriginX = nodeMap.Attributes["parallaxoriginx"];
                var attrParallaxOriginY = nodeMap.Attributes["parallaxoriginy"];

                map.TiledVersion = nodeMap.Attributes["tiledversion"].Value;
                map.Orientation = nodeMap.Attributes["orientation"].Value;
                map.RenderOrder = nodeMap.Attributes["renderorder"].Value;
                map.BackgroundColor = nodeMap.Attributes["backgroundcolor"]?.Value;
                map.Infinite = nodeMap.Attributes["infinite"].Value == "1";

                map.Width = int.Parse(nodeMap.Attributes["width"].Value);
                map.Height = int.Parse(nodeMap.Attributes["height"].Value);
                map.TileWidth = int.Parse(nodeMap.Attributes["tilewidth"].Value);
                map.TileHeight = int.Parse(nodeMap.Attributes["tileheight"].Value);

                if (nodesProperty != null) map.Properties = ParseProperties(nodesProperty);
                if (nodesTileset != null) map.Tilesets = ParseTilesets(nodesTileset);
                if (nodesLayer != null) map.Layers = ParseLayers(map, nodesLayer, nodesObjectGroup, nodesImageLayer);
                if (nodesGroup != null) map.Groups = ParseGroups(map, nodesGroup);
                if (attrParallaxOriginX != null)
                    map.ParallaxOriginX = float.Parse(attrParallaxOriginX.Value, CultureInfo.InvariantCulture);
                if (attrParallaxOriginY != null)
                    map.ParallaxOriginY = float.Parse(attrParallaxOriginY.Value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new TiledException("An error occurred while trying to parse the Tiled map file", ex);
            }

            return map;
        }

        private static TiledProperty[] ParseProperties(XmlNodeList nodeList)
        {
            var result = new List<TiledProperty>();

            foreach (XmlNode node in nodeList)
            {
                var attrType = node.Attributes["type"];

                var property = new TiledProperty
                {
                    Name = node.Attributes["name"].Value,
                    Value = node.Attributes["value"]?.Value,
                    Type = TiledPropertyType.String
                };

                if (attrType != null)
                    property.Type = attrType.Value switch
                    {
                        "bool" => TiledPropertyType.Bool,
                        "color" => TiledPropertyType.Color,
                        "file" => TiledPropertyType.File,
                        "float" => TiledPropertyType.Float,
                        "int" => TiledPropertyType.Int,
                        "object" => TiledPropertyType.Object,
                        _ => property.Type
                    };

                property.Value ??= node.InnerText;

                result.Add(property);
            }

            return result.ToArray();
        }

        private TiledMapTileset[] ParseTilesets(XmlNodeList nodeList)
        {
            var result = new List<TiledMapTileset>();

            foreach (XmlNode node in nodeList)
            {
                var tileset = new TiledMapTileset();
                tileset.FirstGid = int.Parse(node.Attributes["firstgid"].Value);
                tileset.Source = node.Attributes["source"]?.Value;

                result.Add(tileset);
            }

            return result.ToArray();
        }

        private TiledGroup[] ParseGroups(TiledMap map, XmlNodeList nodeListGroups)
        {
            var result = new List<TiledGroup>();

            foreach (XmlNode node in nodeListGroups)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodesGroup = node.SelectNodes("group");
                var nodesLayer = node.SelectNodes("layer");
                var nodesObjectGroup = node.SelectNodes("objectgroup");
                var nodesImageLayer = node.SelectNodes("imagelayer");
                var attrVisible = node.Attributes["visible"];
                var attrLocked = node.Attributes["locked"];

                var tiledGroup = new TiledGroup();
                tiledGroup.Id = int.Parse(node.Attributes["id"].Value);
                tiledGroup.Name = node.Attributes["name"].Value;

                if (attrVisible != null) tiledGroup.Visible = attrVisible.Value == "1";
                if (attrLocked != null) tiledGroup.Locked = attrLocked.Value == "1";
                if (nodesProperty != null) tiledGroup.Properties = ParseProperties(nodesProperty);
                if (nodesGroup != null) tiledGroup.Groups = ParseGroups(map, nodesGroup);
                if (nodesLayer != null)
                    tiledGroup.Layers = ParseLayers(map, nodesLayer, nodesObjectGroup, nodesImageLayer);

                result.Add(tiledGroup);
            }

            return result.ToArray();
        }

        private TiledLayer[] ParseLayers(TiledMap map, XmlNodeList nodesLayer, XmlNodeList nodesObjectGroup,
            XmlNodeList nodesImageLayer)
        {
            var result = new List<TiledLayer>();

            foreach (XmlNode node in nodesLayer)
            {
                result.Add(ParseLayer(map, node, TiledLayerType.TileLayer));
            }

            foreach (XmlNode node in nodesObjectGroup)
            {
                result.Add(ParseLayer(map, node, TiledLayerType.ObjectLayer));
            }

            foreach (XmlNode node in nodesImageLayer)
            {
                result.Add(ParseLayer(map, node, TiledLayerType.ImageLayer));
            }

            return result.ToArray();
        }

        private TiledLayer ParseLayer(TiledMap map, XmlNode node, TiledLayerType type)
        {
            var nodesProperty = node.SelectNodes("properties/property");
            var attrVisible = node.Attributes["visible"];
            var attrLocked = node.Attributes["locked"];
            var attrTint = node.Attributes["tintcolor"];
            var attrOffsetX = node.Attributes["offsetx"];
            var attrOffsetY = node.Attributes["offsety"];
            var attrParallaxX = node.Attributes["parallaxx"];
            var attrParallaxY = node.Attributes["parallaxy"];
            var attrOpacity = node.Attributes["opacity"];
            var attrClass = node.Attributes["class"];
            var attrWidth = node.Attributes["width"];
            var attrHeight = node.Attributes["height"];

            var tiledLayer = new TiledLayer
            {
                Id = int.Parse(node.Attributes["id"].Value),
                Type = type,
                Name = node.Attributes["name"].Value,
                Visible = true,
                Opacity = 1.0f,
                ParallaxX = 1.0f,
                ParallaxY = 1.0f
            };

            if (attrWidth != null) tiledLayer.Width = int.Parse(attrWidth.Value);
            if (attrHeight != null) tiledLayer.Height = int.Parse(attrHeight.Value);
            if (attrVisible != null) tiledLayer.Visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.Locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.Tintcolor = attrTint.Value;
            if (attrClass != null) tiledLayer.Class = attrClass.Value;
            if (attrOpacity != null) tiledLayer.Opacity = float.Parse(attrOpacity.Value, CultureInfo.InvariantCulture);
            if (attrOffsetX != null) tiledLayer.OffsetX = float.Parse(attrOffsetX.Value, CultureInfo.InvariantCulture);
            if (attrOffsetY != null) tiledLayer.OffsetY = float.Parse(attrOffsetY.Value, CultureInfo.InvariantCulture);
            if (attrParallaxX != null)
                tiledLayer.ParallaxX = float.Parse(attrParallaxX.Value, CultureInfo.InvariantCulture);
            if (attrParallaxY != null)
                tiledLayer.ParallaxY = float.Parse(attrParallaxY.Value, CultureInfo.InvariantCulture);
            if (nodesProperty != null) tiledLayer.Properties = ParseProperties(nodesProperty);

            switch (type)
            {
                case TiledLayerType.TileLayer:
                {
                    var nodeData = node.SelectSingleNode("data");

                    ParseTileLayerData(map, nodeData, ref tiledLayer);
                    break;
                }
                case TiledLayerType.ObjectLayer:
                {
                    var nodesObject = node.SelectNodes("object");

                    tiledLayer.Objects = ParseObjects(nodesObject);
                    break;
                }
                case TiledLayerType.ImageLayer:
                {
                    var nodeImage = node.SelectSingleNode("image");

                    if (nodeImage != null) tiledLayer.Image = ParseImage(nodeImage);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return tiledLayer;
        }

        private void ParseTileLayerData(TiledMap map, XmlNode nodeData, ref TiledLayer tiledLayer)
        {
            var encoding = nodeData.Attributes["encoding"].Value;
            var compression = nodeData.Attributes["compression"]?.Value;

            if (encoding != "csv" && encoding != "base64")
                throw new TiledException("Only CSV and Base64 encodings are currently supported");

            if (map.Infinite)
            {
                var nodesChunk = nodeData.SelectNodes("chunk");
                var chunks = new List<TiledChunk>();

                foreach (XmlNode nodeChunk in nodesChunk)
                {
                    var chunk = new TiledChunk();
                    chunk.X = int.Parse(nodeChunk.Attributes["x"].Value);
                    chunk.Y = int.Parse(nodeChunk.Attributes["y"].Value);
                    chunk.Width = int.Parse(nodeChunk.Attributes["width"].Value);
                    chunk.Height = int.Parse(nodeChunk.Attributes["height"].Value);

                    switch (encoding)
                    {
                        case "csv":
                            ParseTileLayerDataAsCsv(nodeChunk.InnerText, out var data, out var dataRotationFlags);
                            chunk.Data = data;
                            chunk.DataRotationFlags = dataRotationFlags;
                            break;
                        case "base64":
                            ParseTileLayerDataAsBase64(nodeChunk.InnerText, compression, out data,
                                out dataRotationFlags);
                            chunk.Data = data;
                            chunk.DataRotationFlags = dataRotationFlags;
                            break;
                    }

                    chunks.Add(chunk);
                }

                tiledLayer.Chunks = chunks.ToArray();
            }
            else
            {
                switch (encoding)
                {
                    case "csv":
                        ParseTileLayerDataAsCsv(nodeData.InnerText, out var data, out var dataRotationFlags);
                        tiledLayer.Data = data;
                        tiledLayer.DataRotationFlags = dataRotationFlags;
                        break;
                    case "base64":
                        ParseTileLayerDataAsBase64(nodeData.InnerText, compression, out data, out dataRotationFlags);
                        tiledLayer.Data = data;
                        tiledLayer.DataRotationFlags = dataRotationFlags;
                        break;
                }
            }
        }

        private static void ParseTileLayerDataAsBase64(string input, string compression, out int[] data,
            out byte[] dataRotationFlags)
        {
            using var base64DataStream = new MemoryStream(Convert.FromBase64String(input));
            switch (compression)
            {
                case null:
                    ParseTileLayerData(out data, out dataRotationFlags, base64DataStream);
                    break;
                case "zlib":
                    ParseTileLayerZlibData(out data, out dataRotationFlags, base64DataStream);
                    break;
                case "gzip":
                    ParseTiledLayerGzipData(out data, out dataRotationFlags, base64DataStream);
                    break;
                default:
                    throw new TiledException("Zstandard compression is currently not supported");
            }
        }

        private static void ParseTiledLayerGzipData(out int[] data, out byte[] dataRotationFlags,
            MemoryStream base64DataStream)
        {
            using var decompressionStream = new GZipStream(base64DataStream, CompressionMode.Decompress);
            // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
            var decompressedDataBuffer = new byte[4]; // size of each tile
            var dataRotationFlagsList = new List<byte>();
            var layerDataList = new List<int>();

            while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) ==
                   decompressedDataBuffer.Length)
            {
                var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                var hor = rawId & TiledMap.FlippedHorizontallyFlag;
                var ver = rawId & TiledMap.FlippedVerticallyFlag;
                var dia = rawId & TiledMap.FlippedDiagonallyFlag;

                dataRotationFlagsList.Add((byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte));

                // assign data to rawID with the rotation flags cleared
                layerDataList.Add((int) (rawId & ~(TiledMap.FlippedHorizontallyFlag |
                                                   TiledMap.FlippedVerticallyFlag |
                                                   TiledMap.FlippedDiagonallyFlag)));
            }

            data = layerDataList.ToArray();
            dataRotationFlags = dataRotationFlagsList.ToArray();
        }

        private static void ParseTileLayerZlibData(out int[] data, out byte[] dataRotationFlags,
            MemoryStream base64DataStream)
        {
            // .NET doesn't play well with the headered zlib data that Tiled produces,
            // so we have to manually skip the 2-byte header to get what DeflateStream's looking for
            // Should an external library be used instead of this hack?
            base64DataStream.ReadByte();
            base64DataStream.ReadByte();

            using var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress);

            var decompressedDataBuffer = new byte[4];
            var dataRotationFlagsList = new List<byte>();
            var layerDataList = new List<int>();

            while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) ==
                   decompressedDataBuffer.Length)
            {
                var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                var hor = rawId & TiledMap.FlippedHorizontallyFlag;
                var ver = rawId & TiledMap.FlippedVerticallyFlag;
                var dia = rawId & TiledMap.FlippedDiagonallyFlag;
                dataRotationFlagsList.Add((byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte));

                layerDataList.Add((int) (rawId & ~(TiledMap.FlippedHorizontallyFlag |
                                                   TiledMap.FlippedVerticallyFlag |
                                                   TiledMap.FlippedDiagonallyFlag)));
            }

            data = layerDataList.ToArray();
            dataRotationFlags = dataRotationFlagsList.ToArray();
        }

        private static void ParseTileLayerData(out int[] data, out byte[] dataRotationFlags,
            MemoryStream base64DataStream)
        {
            var rawBytes = new byte[4];
            data = new int[base64DataStream.Length];
            dataRotationFlags = new byte[base64DataStream.Length];

            for (var i = 0; i < base64DataStream.Length; i++)
            {
                base64DataStream.Read(rawBytes, 0, rawBytes.Length);
                var rawId = BitConverter.ToUInt32(rawBytes, 0);
                var hor = rawId & TiledMap.FlippedHorizontallyFlag;
                var ver = rawId & TiledMap.FlippedVerticallyFlag;
                var dia = rawId & TiledMap.FlippedDiagonallyFlag;
                dataRotationFlags[i] = (byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);

                data[i] = (int) (rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag |
                                           TiledMap.FlippedDiagonallyFlag));
            }
        }

        private static void ParseTileLayerDataAsCsv(string input, out int[] data, out byte[] dataRotationFlags)
        {
            var csvs = input.Split(',');

            data = new int[csvs.Length];
            dataRotationFlags = new byte[csvs.Length];

            // Parse the comma separated csv string and update the inner data as well as the data rotation flags
            for (var i = 0; i < csvs.Length; i++)
            {
                var rawId = uint.Parse(csvs[i]);
                var hor = rawId & TiledMap.FlippedHorizontallyFlag;
                var ver = rawId & TiledMap.FlippedVerticallyFlag;
                var dia = rawId & TiledMap.FlippedDiagonallyFlag;
                dataRotationFlags[i] = (byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);

                // assign data to rawID with the rotation flags cleared
                data[i] = (int) (rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag |
                                           TiledMap.FlippedDiagonallyFlag));
            }
        }

        private static TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.Source = node.Attributes["source"].Value;
            tiledImage.Width = int.Parse(node.Attributes["width"].Value);
            tiledImage.Height = int.Parse(node.Attributes["height"].Value);

            return tiledImage;
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
                var attrGid = node.Attributes["gid"];

                var obj = new TiledObject();
                obj.Id = int.Parse(node.Attributes["id"].Value);
                obj.Name = node.Attributes["name"]?.Value;
                obj.Class = node.Attributes["class"]?.Value;
                obj.Type = node.Attributes["type"]?.Value;
                obj.X = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
                obj.Y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (attrGid != null) ParseObjectGid(ref obj, attrGid.Value);

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

        private static void ParseObjectGid(ref TiledObject tiledObject, string gid)
        {
            var rawId = uint.Parse(gid);
            var hor = rawId & TiledMap.FlippedHorizontallyFlag;
            var ver = rawId & TiledMap.FlippedVerticallyFlag;
            var dia = rawId & TiledMap.FlippedDiagonallyFlag;

            tiledObject.DataRotationFlag = (byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);
            tiledObject.Gid = (int) (rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag |
                                               TiledMap.FlippedDiagonallyFlag));
        }
    }
}
