using System;

namespace Tiled.Parsing
{
    public class TiledException : Exception
    {
        public TiledException(string message) : base(message)
        {
        }

        public TiledException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
