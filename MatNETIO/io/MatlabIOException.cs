using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatNETIO.io
{
    /// <summary>
    /// Mat-file reader/writer exception
    /// </summary>
    /// <author>
    ///     Jason Zhang, Sunnybrook Research Institute
    ///     (c) 2016 junjie.zhang@sri.utoronto.ca
    /// </author>
    class MatlabIOException : IOException
    {
        /// <summary>
        /// Construct a new <c>MatlabIOException</c>.
        /// </summary>
        /// <param name="s">A string containing the error information.</param>
        public  MatlabIOException(string s) : base(s) { }
    }
}
