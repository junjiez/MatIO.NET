using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatNETIO.types
{
    /// <summary>
	/// This class represents Matlab's Structure object (structure array).
	/// 
	/// Note: An array of structures can contain only structures of the same type,
	/// meaning structures that have the same field names.
	/// </summary>
    public class MLStructure: MLArray
    {
        /// <summary>
		/// A Hashtable that keeps structure field names
		/// </summary>
        private List<string> _keys;

        /// <summary>
		/// Array of structures
		/// </summary>
        private List<Dictionary<string, MLArray>> _mlStructArray;

        /// <summary>
		/// Current structure pointer for bulk insert
		/// </summary>
        private int _currentIndex = 0;

        /// <summary>
		/// Create an <c>MLStructure</c> class object.
		/// </summary>
		/// <param name="Name">The name of the <c>MLStructure</c></param>
		/// <param name="Dims">The array dimensions of the <c>MLStructure</c></param>
        public MLStructure(string Name, int[] Dims):
            this(Name, Dims, MLArray.mxSTRUCT_CLASS, 0) { }

        public MLStructure(string Name, int[] Dims, int Type, int Attributes) :
            base(Name, Dims, Type, Attributes)
        {
            if (Dims.Length != 0)
            {
                int size = Dims.Aggregate(1, (current, t) => current*t);
                _mlStructArray = new List<Dictionary<string, MLArray>>(size);
                _keys = new List<string>();
            }
            else
            {
                _mlStructArray = new List<Dictionary<string, MLArray>>();
                _keys = new List<string>();
            }
        }

        /// <summary>
		/// Public accessor to the field desribed by <c>Name</c> from the current structure.
		/// </summary>
        public MLArray this[string Name]
        {
            set { this[Name, _currentIndex] = value; }
            get { return this[Name, _currentIndex]; }
        }

        /// <summary>
		/// Public accessor to the field described by <c>Name</c> from the index'th structure
		/// in the structure array.
		/// </summary>
        public MLArray this[string Name, int Index]
        {
            set
            {
                if (!_keys.Contains(Name))
                {
                    _keys.Add(Name);
                }
                _currentIndex = Index;

                if (_mlStructArray.Count == 0 || _mlStructArray.Count <= Index)
                {
                    _mlStructArray.Insert(Index, new Dictionary<string, MLArray>());
                }
                _mlStructArray[Index].Add(Name, value);
            }
            get
            {
                return _mlStructArray[Index][Name];
            }
        }

        /// <summary>
		/// Public accessor to the field described by <c>Name</c> from the (m,n)'th structure
		/// in the structure array.
		/// </summary>
		public MLArray this[string Name, int M, int N]
        {
            set { this[Name, GetIndex(M, N)] = value; }
            get { return this[Name, GetIndex(M, N)]; }
        }

        /// <summary>
        /// Public accessor to the field described by <c>Name</c> from the structure specified by <c>subscripts</c>
        /// in the structure array. (Looks a little wired though)
        /// </summary>
        public MLArray this[string Name, int[] subscripts]
        {
            set { this[Name, GetIndex(subscripts)] = value; }
            get { return this[Name, GetIndex(subscripts)]; }
        }

        /// <summary>
		/// Gets the maximum length of field descriptor
		/// </summary>
		public int MaxFieldLength
        {
            get
            {
                int maxLen = 0;
                foreach (string s in _keys)
                {
                    maxLen = s.Length > maxLen ? s.Length : maxLen;
                }
                return maxLen + 1;
            }
        }

        public byte[] GetKeySetToByteArray()
        {
            MemoryStream memstrm = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(memstrm);
            char[] buffer = new char[MaxFieldLength];

            try
            {
                foreach (string s in _keys)
                {
                    for (int i = 0; i < buffer.Length; i++) buffer[i] = (char) 0;
                    Array.Copy(s.ToCharArray(), 0, buffer, 0, s.Length);
                    bw.Write(buffer);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Could not write Structure key set to byte array: " + e);
                return new byte[0];
            }

            return memstrm.ToArray();
        }

        /// <summary>
		/// Gets all the fields from the struct array as a flat list of fields.
		/// </summary>
		public List<MLArray> AllFields
        {
            get
            {
                List<MLArray> fields = new List<MLArray>();

                foreach (Dictionary<string, MLArray> st in _mlStructArray)
                    fields.AddRange(st.Values);

                return fields;
            }
        }

        /// <summary>
        /// Gets all the keys of the structure array
        /// </summary>
        public List<string> AllKeys
        {
            get { return _keys; }
        } 

        /// <summary>
		/// Get a string representation for the content of the array.
		/// See <see cref="MatNETIO.types.MLArray.ContentToString()"/>
		/// </summary>
		/// <returns>A string representation.</returns>
		public override string ContentToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Name + " = \n");

            if (M * N == 1)
            {
                foreach (string key in _keys)
                {
                    sb.Append("\t" + key + " : " + this[key].ContentToString() + "\n");
                }
            }
            else
            {
                sb.Append("\n");
                // sb.Append(M + "x" + N);
                sb.Append(String.Join("x", _dims.Select(d => d.ToString()).ToArray()));
                sb.Append(" struct array with fields: \n");
                foreach (string key in _keys)
                {
                    sb.Append("\t" + key + "\n");
                }
            }
            return sb.ToString();
        }
    }
}
