using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatNETIO.common;

namespace MatNETIO.io
{
    /// <summary>
    /// Used to create a Mat-file style tag
    /// </summary>
    /// <author>
    ///     Jason Zhang, Sunnybrook Research Institue
    ///     (c) 2016 junjie.zhang@sri.utoronto.ca
    /// </author>
    public class MatTag
    {
        /// <summary>
        /// The array type for this tag.
        /// </summary>
        protected int _type;
        /// <summary>
        /// The size of the tag.
        /// </summary>
        protected int _size;

        public MatTag(int Type, int Size)
        {
            _type = Type;
            _size = Size;
        }

        /// <summary>
		/// Calculate the padding for the element.
		/// </summary>
		/// <param name="size">The size of the element.</param>
		/// <param name="compressed">Is the tag compressed?</param>
		/// <returns>The number of padding bytes</returns>
        protected int GetPadding(int size, bool compressed)
        {
            int padding;
            // data not packed in the tag
            if (!compressed)
            {
                int b;
                padding = (b = (((size / SizeOf()) % (8 / SizeOf())) * SizeOf())) != 0 ? 8 - b : 0;
            }
            else
            {
                int b;
                padding = (b = (((size / SizeOf()) % (4 / SizeOf())) * SizeOf())) != 0 ? 4 - b : 0;
            }
            return padding;
        }

        /// <summary>
		/// Get size of single data in this tag.
		/// </summary>
		/// <returns>The number of bytes for single data</returns>
        public int SizeOf()
        {
            return MatDataTypes.SizeOf(_type);
        }

        /// <summary>
        /// Get the type of the MatTag
        /// </summary>
        public int Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Get the number of bytes for the MAT-Data object
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
		/// <see cref="Object.ToString()"/>
		/// </summary>
		public override string ToString()
        {
            string s;
            s = "[tag: " + MatDataTypes.TypeToString(_type) + " size: " + _size + "]";
            return s;
        }
    }
}
