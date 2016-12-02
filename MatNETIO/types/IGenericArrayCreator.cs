using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatNETIO.types
{
    /// <summary>
	/// Interface used to create a generic array GenericArrayCreator.
	/// </summary>
    public interface IGenericArrayCreator<T>
    {
        /// <summary>
		/// Creates a generic array.
		/// </summary>
		/// <param name="m">The number of columns in the array</param>
		/// <param name="n">The number of rows in the array</param>
		/// <returns>A generic array.</returns>
        T[] CreateArray(int m, int n);

        /// <summary>
        /// Creates a generic array.
        /// </summary>
        /// <param name="dims">The dimension of the array to create</param>
        /// <returns>A generic array.</returns>
        T[] CreateArray(int[] dims);
    }
}
