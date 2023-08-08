using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace J2y.Network
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetException
	//		Exception thrown in the Network Library
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public sealed class NetException : Exception
	{

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// NetException constructor
        /// </summary>
        public NetException()
            : base()
        {
        }

        /// <summary>
        /// NetException constructor
        /// </summary>
        public NetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// NetException constructor
        /// </summary>
        public NetException(string message, Exception inner)
            : base(message, inner)
        {
        }
        #endregion
        
        #region [DEBUG] [Static] Assert
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Throws an exception, in DEBUG only, if first parameter is false
        /// </summary>
        [Conditional("DEBUG")]
        public static void Assert(bool isOk, string message)
        {
            if (!isOk)
                throw new NetException(message);
        }

        /// <summary>
        /// Throws an exception, in DEBUG only, if first parameter is false
        /// </summary>
        [Conditional("DEBUG")]
        public static void Assert(bool isOk)
        {
            if (!isOk)
                throw new NetException();
        }
        #endregion

    }
}
