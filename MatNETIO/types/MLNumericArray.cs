using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatNETIO.common;
using System.Runtime.InteropServices;

namespace MatNETIO.types
{
    public abstract class MLNumericArray<T> : MLArray, IGenericArrayCreator<T>, IByteStorageSupport
    {
        private ByteBuffer _real;
        private ByteBuffer _imaginary;
        private byte[] _bytes;

        #region Constructors
        /// <summary>
		/// Constructs an abstract MLNumericArray class object
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Dims">The dimensions of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="Attributes">Any attributes associated with this array.</param>
		protected MLNumericArray(string Name, int[] Dims, int Type, int Attributes)
            : base(Name, Dims, Type, Attributes)
        {
            _real = new ByteBuffer(Size * GetBytesAllocated);
            if (IsComplex)
                _imaginary = new ByteBuffer(Size * GetBytesAllocated);
            _bytes = new byte[GetBytesAllocated];
        }

        /// <summary>
		/// <a href="http://math.nist.gov/javanumerics/jama/">Jama</a> [math.nist.gov] style:
		/// construct a 2D real matrix from a one-dimensional packed array.
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="Vals">One-dimensional array of doubles, packed by columns.</param>
		/// <param name="M">The number of rows.</param>
		protected MLNumericArray(string Name, int Type, T[] Vals, int M)
            : this(Name, new int[] { M, Vals.Length / M }, Type, 0)
        {
            // Fill in the array
            for (int i = 0; i < Vals.Length; i++)
                Set(Vals[i], i);
        }

        /// <summary>
		/// <a href="http://math.nist.gov/javanumerics/jama/">Jama</a> [math.nist.gov] style:
		/// construct a 2D imaginary matrix from a one-dimensional packed array.
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="RealVals">One-dimensional array of doubles for the <i>real</i> part, 
		/// packed by columns</param>
		/// <param name="ImagVals">One-dimensional array of doubles for the <i>imaginary</i> part, 
		/// packed by columns</param>
		/// <param name="M">The number of columns</param>
		protected MLNumericArray(string Name, int Type, T[] RealVals, T[] ImagVals, int M)
            : this(Name, new int[] { M, RealVals.Length / M }, Type, MLArray.mtFLAG_COMPLEX)
        {
            if (ImagVals.Length != RealVals.Length)
                throw new ArgumentException("Attempting to create an imaginary numeric array where the " +
                    "imaginary array is _not_ the same size as the real array.");

            // Fill in the imaginary array
            for (int i = 0; i < ImagVals.Length; i++)
            {
                SetReal(RealVals[i], i);
                SetImaginary(ImagVals[i], i);
            }
        }

        #endregion

        /// <summary>Gets the flags for this array.</summary>
		public override int Flags
        {
            get
            {
                return (int)((uint)(base._type & MLArray.mtFLAG_TYPE)
                    | (uint)(base._attributes & 0xFFFFFF00));
            }
        }

        /// <summary>
        /// Gets a single real array element of A(m,n).
        /// </summary>
        /// <param name="M">Row index</param>
        /// <param name="N">Column index</param>
        /// <returns>Array Element</returns>
        public virtual T GetReal(int M, int N)
        {
            return GetReal(GetIndex(M, N));
        }

        /// <summary>
        /// Gets the <c>ByteBuffer</c> for the Real Numbers.
        /// </summary>
        /// <returns>The real buffer</returns>
        public ByteBuffer GetReal()
        {
            return _real;
        }

        /// <summary>
        /// Get a single real array element
        /// </summary>
        /// <param name="Index">Column-packed vector index.</param>
        /// <returns>Array Element.</returns>
        public virtual T GetReal(int Index)
        {
            return _Get(_real, Index);
        }

        /// <summary>
        /// Get a single real array element by subscript (in Matlab convention, starts from 1~M)
        /// </summary>
        /// <param name="subscripts">Subscripts in Matlab convention.</param>
        /// <returns>Array Element.</returns>
        public virtual T GetReal(int[] subscripts)
        {
            return _Get(_real, this.GetIndex(subscripts));
        }

        /// <summary>
        /// Sets a single real array element.
        /// </summary>
        /// <param name="Val">The element value.</param>
        /// <param name="M">The row index.</param>
        /// <param name="N">The column index.</param>
        public virtual void SetReal(T Val, int M, int N)
        {
            SetReal(Val, GetIndex(M, N));
        }

        /// <summary>
        /// Sets a single real array element.
        /// </summary>
        /// <param name="Val">The element value.</param>
        /// <param name="Index">Column-packed vector index.</param>
        public virtual void SetReal(T Val, int Index)
        {
            _Set(_real, Val, Index);
        }
        /// <summary>
        /// Set a single real array element.
        /// </summary>
        /// <param name="Val">The element value.</param>
        /// <param name="subscripts">Subscripts in Matlab convention (starts from 1~M).</param>
        public virtual void SetReal(T Val, int[] subscripts)
        {
            _Set(_real, Val, GetIndex(subscripts));
        }

        ///// <summary>
        ///// Sets real part of the matrix.
        ///// </summary>
        ///// <exception cref="ArgumentException">When the <c>Vector</c> is not the
        ///// same length as the Numeric Array.</exception>
        ///// <param name="Vector">A column-packed vector of elements.</param>
        //public void SetReal( T[] Vector )
        //{
        //    if( Vector.Length != Size )
        //        throw new ArgumentException("Matrix dimensions do not match. " + Size + " not " + Vector.Length );
        //    //Array.Copy( Vector, 0, _real, 0, Vector.Length );
        //    _real.CopyFrom(Vector);
        //}

        /// <summary>
        /// Sets a single imaginary array element.
        /// </summary>
        /// <param name="Val">Element value.</param>
        /// <param name="M">Row Index.</param>
        /// <param name="N">Column Index.</param>
        public virtual void SetImaginary(T Val, int M, int N)
        {
            if (IsComplex)
                SetImaginary(Val, GetIndex(M, N));
        }

        /// <summary>
        /// Sets a single imaginary array element.
        /// </summary>
        /// <param name="Val">Element Value</param>
        /// <param name="Index">Column-packed vector index.</param>
        public virtual void SetImaginary(T Val, int Index)
        {
            if (IsComplex)
                _Set(_imaginary, Val, Index);
        }

        /// <summary>
        /// Sets a single imaginary array element.
        /// </summary>
        /// <param name="Val">Element Value</param>
        /// <param name="Subscripts">Subscripts in Matlab convention (starts from 1~N).</param>
        public virtual void SetImaginary(T Val, int[] Subscripts)
        {
            if(IsComplex)
                _Set(_imaginary, Val, GetIndex(Subscripts));
        }

        /// <summary>
        /// Gets a single imaginary array element of A(m,n)
        /// </summary>
        /// <param name="M">Row index</param>
        /// <param name="N">Column index</param>
        /// <returns>Array element</returns>
        public virtual T GetImaginary(int M, int N)
        {
            return GetImaginary(GetIndex(M, N));
        }

        /// <summary>
        /// Gets a single imaginary array element.
        /// </summary>
        /// <param name="Index">Column-packed vector index</param>
        /// <returns>Array Element</returns>
        public virtual T GetImaginary(int Index)
        {
            return _Get(_imaginary, Index);
        }

        /// <summary>
        /// Gets a single imaginary array element.
        /// </summary>
        /// <param name="subscripts">Subscripts in Matlab convention (starts from 1~M).</param>
        /// <returns>Array Element</returns>
        public virtual T GetImaginary(int[] subscripts)
        {
            return _Get(_imaginary, GetIndex(subscripts));
        }

        /// <summary>
        /// Gets the <c>ByteBuffer</c> for the Real Numbers.
        /// </summary>
        /// <returns>The real buffer</returns>
        public ByteBuffer GetImaginary()
        {
            if (IsComplex)
                return _imaginary;
            else
                return null;
        }

        /// <summary>
        /// Does the same as <c>SetReal</c>.
        /// </summary>
        /// <param name="Val">Element Value</param>
        /// <param name="M">Row index</param>
        /// <param name="N">Column index</param>
        public void Set(T Val, int M, int N)
        {
            if (IsComplex)
                throw new MethodAccessException("Cannot use this method for Complex matrices");
            SetReal(Val, M, N);
        }

        /// <summary>
        /// Does the same as <c>SetReal</c>.
        /// </summary>
        /// <param name="Val">Element Value</param>
        /// <param name="Index">Column-packed vector index</param>
        public void Set(T Val, int Index)
        {
            if (IsComplex)
                throw new MethodAccessException("Cannot use this method for Complex matrices");
            SetReal(Val, Index);
        }
        /// <summary>
        /// Does the same as <c>GetReal</c>.
        /// </summary>
        /// <param name="M">Row index</param>
        /// <param name="N">Column index</param>
        /// <returns>An array element value.</returns>
        public T Get(int M, int N)
        {
            if (IsComplex)
                throw new MethodAccessException("Cannot use this method for Complex matrices");
            return GetReal(M, N);
        }
        /// <summary>
        /// Does the same as <c>GetReal</c>.
        /// </summary>
        /// <param name="subscripts">Subscripts in Matlab convention (starts from 1~M).</param>
        /// <returns>An array element value.</returns>
        public T Get(int[] subscripts)
        {
            if(IsComplex)
                throw new MethodAccessException("Cannot use this method for Complex matrices");
            return GetReal(subscripts);
        }

        /// <summary>
        /// Does the same as <c>GetReal</c>.
        /// </summary>
        /// <param name="Index">Column-packed vector index</param>
        /// <returns>An array element value.</returns>
        public T Get(int Index)
        {
            if (IsComplex)
                throw new MethodAccessException("Cannot use this method for Complex matrices");
            return GetReal(Index);
        }

        ///// <summary>
        ///// Does the same as <c>SetReal</c>
        ///// </summary>
        ///// <param name="Vector">A column-packed vector of elements.</param>
        //public void Set( T[] Vector )
        //{
        //    if( IsComplex )
        //        throw new MethodAccessException("Cannot use this method for Complex matrices");
        //    SetReal( Vector );
        //}

        private int _GetByteOffset(int Index)
        {
            return Index * GetBytesAllocated;
        }

        /// <summary>
        /// Gets a single objects data from a <c>ByteBuffer</c>.
        /// </summary>
        /// <param name="Buffer">The <c>ByteBuffer</c> object.</param>
        /// <param name="Index">A column-packed index.</param>
        /// <returns>The object data.</returns>
        protected virtual T _Get(ByteBuffer Buffer, int Index)
        {
            Buffer.Position(_GetByteOffset(Index));
            Buffer.Get(ref _bytes, 0, _bytes.Length);
            return (T)BuildFromBytes(_bytes);
        }

        /// <summary>
        /// Sets a single object data into a <c>ByteBuffer</c>
        /// </summary>
        /// <param name="Buffer">The <c>ByteBuffer</c> to where the object data will be stored.</param>
        /// <param name="Val">The object data.</param>
        /// <param name="Index">A column-packed index</param>
        protected void _Set(ByteBuffer Buffer, T Val, int Index)
        {
            Buffer.Position(_GetByteOffset(Index));
            Buffer.Put(GetByteArray(Val));
        }

        /// <summary>
        /// Gets a two-dimensional array.
        /// </summary>
        /// <returns>2D array.</returns>
        /// <remarks>
        /// Only works for original array with 2 dimensions
        /// </remarks>
        public T[][] GetArray()
        {
            T[][] result = new T[M][];
            for (int m = 0; m < M; m++)
            {
                result[m] = new T[N];
                for (int n = 0; n < N; n++)
                {
                    result[m][n] = GetReal(m, n);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a one-dimensional array T[].
        /// </summary>
        /// <returns>1D array.</returns>
        /// <remarks>
        /// Works only when dimension is 2 and min(Dimensions) = 1. Or return <c>null</c>.
        /// </remarks>
        public T[] GetArray1D()
        {
            if (this.NDimensions != 2)
                return null;

            if (this.Dimensions.Min() != 1)
                return null;

            T[] result = new T[Size];
            int count = 0;
            for (int m = 0; m < M; m++)
            {
                for (int n = 0; n < N; n++)
                {
                    result[count] = GetReal(m, n);
                    count ++;
                }
            }
            return result;
        }

        /// <summary>
        /// Get a two-dimensional array T[,].
        /// </summary>
        /// <returns>2D array.</returns>
        /// <remarks>
        /// Works only when dimension is 2. Or return <c>null</c>.
        /// </remarks>
        public T[,] GetArray2D()
        {
            if (this.NDimensions !=2)
                return null;

            T[,] result = new T[M, N];
            for (int m = 0; m < M; m++)
            {
                for (int n = 0; n < N; n++)
                {
                    result[m, n] = GetReal(m, n);
                }
            }
            return result;
        }

        /// <summary>
        /// Get a three-dimension array T[,,].
        /// </summary>
        /// <returns>3D array.</returns>
        /// <remarks>
        /// Works only when dimension is 3. Or return <c>null</c>.
        /// </remarks>
        public T[,,] GetArray3D()
        {
            if (this.NDimensions != 3)
                return null;

            int col = Dimensions[1];
            int row = Dimensions[0];
            int depth = Dimensions[2];
            T[,,] result = new T[row,col,depth];
            for (int k = 0; k < depth; k++)
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        result[i, j, k] = GetReal(new int[3] {i, j, k});
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get a 2D Array List. In the list, each element is a 2D array T[,]
        /// </summary>
        /// <returns>2D Array List (IList)</returns>
        /// <remarks>
        /// Works only when dimension is 3. Or return <c>null</c>.
        /// </remarks>
        public IList<T[,]> GetArray2DList()
        {
            if (this.NDimensions != 3)
                return null;

            int col = Dimensions[1];
            int row = Dimensions[0];
            int depth = Dimensions[2];

            IList<T[,]> result = new List<T[,]>();
            for (int k = 0; k < depth; k++)
            {
                T[,] arr = new T[row, col];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        arr[i, j] = GetReal(new int[3] { i, j, k });
                    }
                }
                result.Add(arr);
            }

            return result;
        } 

        /// <summary>
        /// Gets the imaginary part of the number array.
        /// </summary>
        public ByteBuffer ImaginaryByteBuffer
        {
            get { return _imaginary; }
            set
            {
                if (!IsComplex)
                    throw new MethodAccessException("Array is not complex");
                _imaginary.Rewind();
                _imaginary.Put(value);
            }
        }

        /// <summary>
        /// Gets the <c>ByteBuffer</c> for the real numbers in the 
        /// array.
        /// </summary>
        public ByteBuffer RealByteBuffer
        {
            get { return _real; }
            set
            {
                _real.Rewind();
                _real.Put(value);
            }
        }

        /// <summary>
        /// Get a string representation for the content of the array.
        /// See <see cref="MatNETIO.types.MLArray.ContentToString()"/>
        /// </summary>
        /// <returns>A string representation.</returns>
        /// <remarks>Only display 2D array properly.</remarks>
        public override string ContentToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Name + " = \n");

            if (Size > 1000)
            {
                // sb.Append("Cannot display variables with more than 1000 elements.");
                sb.Append(this.ToString());
                return sb.ToString();
            }
            //for (int m = 0; m < M; m++)
            //{
            //    sb.Append("\t");
            //    for (int n = 0; n < N; n++)
            //    {
            //        sb.Append(GetReal(m, n));
            //        if (IsComplex)
            //            sb.Append("+" + GetImaginary(m, n));
            //        sb.Append("\t");
            //    }
            //    sb.Append("\n");
            //}
            if (this.NDimensions == 2)
            {
                for (int m = 0; m < M; m++)
                {
                    sb.Append("\t");
                    for (int n = 0; n < N; n++)
                    {
                        int[] subs = new int[2] {m, n};
                        sb.Append(GetReal(this.GetIndex(subs)));
                        if (IsComplex)
                            sb.Append("+" + GetImaginary(this.GetIndex(subs)));
                        sb.Append("\t");
                    }
                    sb.Append("\n");
                }
            }
            else if (this.NDimensions == 3)
            {
                for (int k = 0; k < this.Dimensions[2]; k++)
                {
                    for (int m = 0; m < this.Dimensions[0]; m++)
                    {
                        sb.Append("\t");
                        for (int n = 0; n < this.Dimensions[1]; n++)
                        {
                            int[] subs = new int[3] { m, n, k };
                            sb.Append(GetReal(this.GetIndex(subs)));
                            if (IsComplex)
                                sb.Append("+" + GetImaginary(this.GetIndex(subs)));
                            sb.Append("\t");
                        }
                        sb.Append("\n");
                    }
                    sb.Append("\t---\n");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Overridden equals operator, see <see cref="System.Object.Equals(System.Object)"/>
        /// </summary>
        /// <param name="o">A <c>System.Object</c> to be compared with.</param>
        /// <returns>True if the object match.</returns>
        public override bool Equals(object o)
        {
            if (o.GetType() == typeof(MLNumericArray<T>))
            {
                bool result = DirectByteBufferEquals(_real, ((MLNumericArray<T>)o).GetReal()) &&
                    Array.Equals(Dimensions, ((MLNumericArray<T>)o).Dimensions);

                if (IsComplex && result)
                    result &= DirectByteBufferEquals(_imaginary, ((MLNumericArray<T>)o).GetImaginary());
                return result;
            }
            return base.Equals(o);
        }

        /// <summary>
        /// Serves as a hash function for an MLNumericArray.
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// Equals implementation for a direct <c>ByteBuffer</c>
        /// </summary>
        /// <param name="buffa">The source buffer to be compared.</param>
        /// <param name="buffb">The destination buffer to be compared.</param>
        /// <returns><c>true</c> if buffers are equal in terms of content.</returns>
        private static bool DirectByteBufferEquals(ByteBuffer buffa, ByteBuffer buffb)
        {
            if (buffa == buffb)
                return true;

            if (buffa == null || buffb == null)
                return false;

            buffa.Rewind();
            buffb.Rewind();

            int length = buffa.Remaining;

            if (buffb.Remaining != length)
                return false;

            for (int i = 0; i < length; i++)
                if (buffa.Get() != buffb.Get())
                    return false;

            return true;
        }


        #region GenericArrayCreator Members

        /// <summary>
        /// Creates a generic array.
        /// </summary>
        /// <param name="m">The number of columns in the array</param>
        /// <param name="n">The number of rows in the array</param>
        /// <returns>A generic array.</returns>
        public virtual T[] CreateArray(int m, int n)
        {
            return new T[m * n];
        }

        public virtual T[] CreateArray(int[] dims)
        {
            return new T[dims.Aggregate(1, (current, t) => current * t)];
        }

        #endregion

        #region ByteStorageSupport Members

        /// <summary>
        /// Gets the number of bytes allocated for a type
        /// </summary>
        public virtual int GetBytesAllocated
        {
            get
            {
                int retval;
                Type tt = typeof(T);
                if (tt.IsValueType)
                {
                    retval = Marshal.SizeOf(tt);
                }
                else
                {
                    // tt is a reference type, so the size in memory is the pointer size.
                    // We could return "retval = IntPtr.Size", but probably thats not what the user wants?
                    // So tell him something went wrong.
                    retval = -1;
                }

                return retval;
            }
        }

        /// <summary>
        /// Builds a numeric object from a byte array.
        /// </summary>
        /// <param name="bytes">A byte array containing the data.</param>
        /// <returns>A numeric object</returns>
        public virtual object BuildFromBytes(byte[] bytes)
        {
            if (bytes.Length != GetBytesAllocated)
            {
                throw new ArgumentException(
                    "To build from a byte array, I need an array of size: " + GetBytesAllocated);
            }

            return BuildFromBytes2(bytes);
        }

        /// <summary>
        /// Gets a byte array from a numeric object.
        /// </summary>
        /// <param name="val">The numeric object to convert into a byte array.</param>
        public abstract byte[] GetByteArray(object val);

        /// <summary>
        /// Gets the type of numeric object that this byte storage represents
        /// </summary>
        public virtual Type GetStorageType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Builds a numeric object from a byte array.
        /// </summary>
        /// <param name="bytes">A byte array containing the data.</param>
        /// <returns>A numeric object</returns>
        protected abstract object BuildFromBytes2(byte[] bytes);

        #endregion

    }
}
