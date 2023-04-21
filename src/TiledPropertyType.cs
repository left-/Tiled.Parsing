namespace Tiled.Parsing
{
    /// <summary>
    /// Represents property's value data type
    /// </summary>
    public enum TiledPropertyType
    {
        /// <summary>
        /// A string value
        /// </summary>
        String,
        
        /// <summary>
        /// A bool value
        /// </summary>
        Bool,
        
        /// <summary>
        /// A color value in hex format
        /// </summary>
        Color,
        
        /// <summary>
        /// A file path as string
        /// </summary>
        File,
        
        /// <summary>
        /// A float value
        /// </summary>
        Float,
        
        /// <summary>
        /// An int value
        /// </summary>
        Int,
        
        /// <summary>
        /// An object value which is the id of an object in the map
        /// </summary>
        Object
    }
}
