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
    /// Represents a layer or object group
    /// </summary>
    public class TiledGroup
    {
        /// <summary>
        /// The group's id
        /// </summary>
        public int id;

        /// <summary>
        /// The group's name
        /// </summary>
        public string name;

        /// <summary>
        /// The group's visibility
        /// </summary>
        public bool visible;

        /// <summary>
        /// The group's locked state
        /// </summary>
        public bool locked;

        /// <summary>
        /// The group's user properties
        /// </summary>
        public TiledProperty[] properties;

        /// <summary>
        /// The group's layers
        /// </summary>
        public TiledLayer[] layers;

        /// <summary>
        /// The group's objects
        /// </summary>
        public TiledObject[] objects;

        /// <summary>
        /// The group's subgroups
        /// </summary>
        public TiledGroup[] groups;
    }
}
