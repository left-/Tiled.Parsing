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

        private TiledProperty[] ParseProperties(XmlNodeList nodeList)
        {
            var result = new List<TiledProperty>();

            foreach (XmlNode node in nodeList)
            {
                var attrType = node.Attributes["type"];

                var property = new TiledProperty();
                property.name = node.Attributes["name"].Value;
                property.value = node.Attributes["value"]?.Value;
                property.type = TiledPropertyType.String;

                if (attrType != null)
                {
                    if (attrType.Value == "bool") property.type = TiledPropertyType.Bool;
                    if (attrType.Value == "color") property.type = TiledPropertyType.Color;
                    if (attrType.Value == "file") property.type = TiledPropertyType.File;
                    if (attrType.Value == "float") property.type = TiledPropertyType.Float;
                    if (attrType.Value == "int") property.type = TiledPropertyType.Int;
                    if (attrType.Value == "object") property.type = TiledPropertyType.Object;
                }

                if (property.value == null) property.value = node.InnerText;

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
                tileset.firstgid = int.Parse(node.Attributes["firstgid"].Value);
                tileset.source = node.Attributes["source"]?.Value;

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
                tiledGroup.id = int.Parse(node.Attributes["id"].Value);
                tiledGroup.name = node.Attributes["name"].Value;

                if (attrVisible != null) tiledGroup.visible = attrVisible.Value == "1";
                if (attrLocked != null) tiledGroup.locked = attrLocked.Value == "1";
                if (nodesProperty != null) tiledGroup.properties = ParseProperties(nodesProperty);
                if (nodesGroup != null) tiledGroup.groups = ParseGroups(map, nodesGroup);
                if (nodesLayer != null)
                    tiledGroup.layers = ParseLayers(map, nodesLayer, nodesObjectGroup, nodesImageLayer);

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

            var tiledLayer = new TiledLayer();
            tiledLayer.id = int.Parse(node.Attributes["id"].Value);
            tiledLayer.type = type;
            tiledLayer.name = node.Attributes["name"].Value;
            tiledLayer.visible = true;
            tiledLayer.opacity = 1.0f;
            tiledLayer.parallaxX = 1.0f;
            tiledLayer.parallaxY = 1.0f;

            if (attrWidth != null) tiledLayer.width = int.Parse(attrWidth.Value);
            if (attrHeight != null) tiledLayer.height = int.Parse(attrHeight.Value);
            if (attrVisible != null) tiledLayer.visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.tintcolor = attrTint.Value;
            if (attrClass != null) tiledLayer.@class = attrClass.Value;
            if (attrOpacity != null) tiledLayer.opacity = float.Parse(attrOpacity.Value, CultureInfo.InvariantCulture);
            if (attrOffsetX != null) tiledLayer.offsetX = float.Parse(attrOffsetX.Value, CultureInfo.InvariantCulture);
            if (attrOffsetY != null) tiledLayer.offsetY = float.Parse(attrOffsetY.Value, CultureInfo.InvariantCulture);
            if (attrParallaxX != null)
                tiledLayer.parallaxX = float.Parse(attrParallaxX.Value, CultureInfo.InvariantCulture);
            if (attrParallaxY != null)
                tiledLayer.parallaxY = float.Parse(attrParallaxY.Value, CultureInfo.InvariantCulture);
            if (nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);

            if (type == TiledLayerType.TileLayer)
            {
                var nodeData = node.SelectSingleNode("data");

                ParseTileLayerData(map, nodeData, ref tiledLayer);
            }

            if (type == TiledLayerType.ObjectLayer)
            {
                var nodesObject = node.SelectNodes("object");

                tiledLayer.objects = ParseObjects(nodesObject);
            }

            if (type == TiledLayerType.ImageLayer)
            {
                var nodeImage = node.SelectSingleNode("image");

                if (nodeImage != null) tiledLayer.image = ParseImage(nodeImage);
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
                    chunk.x = int.Parse(nodeChunk.Attributes["x"].Value);
                    chunk.y = int.Parse(nodeChunk.Attributes["y"].Value);
                    chunk.width = int.Parse(nodeChunk.Attributes["width"].Value);
                    chunk.height = int.Parse(nodeChunk.Attributes["height"].Value);

                    if (encoding == "csv")
                        ParseTileLayerDataAsCSV(nodeChunk.InnerText, ref chunk.data, ref chunk.dataRotationFlags);
                    if (encoding == "base64")
                        ParseTileLayerDataAsBase64(nodeChunk.InnerText, compression, ref chunk.data,
                            ref chunk.dataRotationFlags);

                    chunks.Add(chunk);
                }

                tiledLayer.chunks = chunks.ToArray();
            }
            else
            {
                if (encoding == "csv")
                    ParseTileLayerDataAsCSV(nodeData.InnerText, ref tiledLayer.data, ref tiledLayer.dataRotationFlags);
                if (encoding == "base64")
                    ParseTileLayerDataAsBase64(nodeData.InnerText, compression, ref tiledLayer.data,
                        ref tiledLayer.dataRotationFlags);
            }
        }

        private void ParseTileLayerDataAsBase64(string input, string compression, ref int[] data,
            ref byte[] dataRotationFlags)
        {
            using (var base64DataStream = new MemoryStream(Convert.FromBase64String(input)))
            {
                if (compression == null)
                {
                    // Parse the decoded bytes and update the inner data as well as the data rotation flags
                    var rawBytes = new byte[4];
                    data = new int[base64DataStream.Length];
                    dataRotationFlags = new byte[base64DataStream.Length];

                    for (var i = 0; i < base64DataStream.Length; i++)
                    {
                        base64DataStream.Read(rawBytes, 0, rawBytes.Length);
                        var rawID = BitConverter.ToUInt32(rawBytes, 0);
                        var hor = rawID & TiledMap.FlippedHorizontallyFlag;
                        var ver = rawID & TiledMap.FlippedVerticallyFlag;
                        var dia = rawID & TiledMap.FlippedDiagonallyFlag;
                        dataRotationFlags[i] = (byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);

                        // assign data to rawID with the rotation flags cleared
                        data[i] = (int) (rawID & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag |
                                                   TiledMap.FlippedDiagonallyFlag));
                    }
                }
                else if (compression == "zlib")
                {
                    // .NET doesn't play well with the headered zlib data that Tiled produces,
                    // so we have to manually skip the 2-byte header to get what DeflateStream's looking for
                    // Should an external library be used instead of this hack?
                    base64DataStream.ReadByte();
                    base64DataStream.ReadByte();

                    using var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress);

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
                else if (compression == "gzip")
                {
                    using (var decompressionStream = new GZipStream(base64DataStream, CompressionMode.Decompress))
                    {
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
                }
                else
                {
                    throw new TiledException("Zstandard compression is currently not supported");
                }
            }
        }

        private void ParseTileLayerDataAsCSV(string input, ref int[] data, ref byte[] dataRotationFlags)
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

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.source = node.Attributes["source"].Value;
            tiledImage.width = int.Parse(node.Attributes["width"].Value);
            tiledImage.height = int.Parse(node.Attributes["height"].Value);

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
                obj.id = int.Parse(node.Attributes["id"].Value);
                obj.name = node.Attributes["name"]?.Value;
                obj.@class = node.Attributes["class"]?.Value;
                obj.type = node.Attributes["type"]?.Value;
                obj.x = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
                obj.y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (attrGid != null) ParseObjectGid(ref obj, attrGid.Value);

                if (nodesProperty != null) obj.properties = ParseProperties(nodesProperty);

                if (nodePolygon != null)
                {
                    var points = nodePolygon.Attributes["points"].Value;
                    var vertices = points.Split(' ');

                    var polygon = new TiledPolygon();
                    polygon.points = new float[vertices.Length * 2];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        polygon.points[i * 2 + 0] =
                            float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
                        polygon.points[i * 2 + 1] =
                            float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
                    }

                    obj.polygon = polygon;
                }

                if (nodeEllipse != null) obj.ellipse = new TiledEllipse();

                if (nodePoint != null) obj.point = new TiledPoint();

                if (node.Attributes["width"] != null)
                    obj.width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["height"] != null)
                    obj.height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["rotation"] != null)
                    obj.rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);

                result.Add(obj);
            }

            return result.ToArray();
        }

        private void ParseObjectGid(ref TiledObject tiledObject, string gid)
        {
            var rawId = uint.Parse(gid);
            var hor = rawId & TiledMap.FlippedHorizontallyFlag;
            var ver = rawId & TiledMap.FlippedVerticallyFlag;
            var dia = rawId & TiledMap.FlippedDiagonallyFlag;

            tiledObject.dataRotationFlag = (byte) ((hor | ver | dia) >> TiledMap.ShiftFlipFlagToByte);
            tiledObject.gid = (int) (rawId & ~(TiledMap.FlippedHorizontallyFlag | TiledMap.FlippedVerticallyFlag |
                                               TiledMap.FlippedDiagonallyFlag));
        }
    }
}
