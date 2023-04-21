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
    /// Represents an image
    /// </summary>
    public class TiledImage
    {
        /// <summary>
        /// The image width
        /// </summary>
        public int width;

        /// <summary>
        /// The image height
        /// </summary>
        public int height;

        /// <summary>
        /// The image source path
        /// </summary>
        public string source;
    }
}
