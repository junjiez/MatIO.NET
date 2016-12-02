using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatNETIO.types
{
    /// <summary>
	/// An Empty array class
	/// </summary>
    public class MLEmptyArray : MLArray
    {
        /// <summary>
		/// Create an basic empty array
		/// </summary>
		public MLEmptyArray() :
            base(null, new int[] { 0, 0 }, mxDOUBLE_CLASS, 0)
        { }

        /// <summary>
        /// Ceate an basic empty array
        /// </summary>
        /// <param name="name">The name of the array</param>
        public MLEmptyArray(string name) :
            base(name, new int[] { 0, 0 }, mxDOUBLE_CLASS, 0)
        {
        }

        /// <summary>
        /// Construct an MLEmptyArray object.
        /// </summary>
        /// <param name="name">The name of the array</param>
        /// <param name="dims">The array dimensions</param>
        /// <param name="type">The type of array</param>
        /// <param name="attributes">Any attributes for this array</param>
        public MLEmptyArray(string name, int[] dims, int type, int attributes) :
            base(name, dims, type, attributes)
        {
        }

        public override string ContentToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Name + " = \n");

            sb.Append("\tEmpty\n");

            return sb.ToString();
        }
    }
}
