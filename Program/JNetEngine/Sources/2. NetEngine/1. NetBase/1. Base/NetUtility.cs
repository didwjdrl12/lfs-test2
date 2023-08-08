#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
using NetAddress = System.Net.IPAddress;
#endif

using System;
using System.Net;

using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace J2y.Network
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetUtility
    //		
    //      1. [Resolve] (hostname, port) -> IPv4 endpoint (Async)
    //      2. BroadcastAddress, LocalAddress
    //      3. BitControl, Byte[]
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class NetUtility
	{

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property / delegate
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] IsMono
        private static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;
        #endregion

        #region [delegate] ResolveEndPoint/ResolveAddress
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Resolve endpoint callback
        /// </summary>
        public delegate void ResolveEndPointCallback(NetEndPoint endPoint);

        /// <summary>
        /// Resolve address callback
        /// </summary>
        public delegate void ResolveAddressCallback(NetAddress adr);
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 네트워크
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Resolve] (hostname, port) -> IPv4 endpoint (Async)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get IPv4 endpoint from notation (xxx.xxx.xxx.xxx) or hostname and port number (asynchronous version)
        /// </summary>
        public static void ResolveAsync(string ipOrHost, int port, ResolveEndPointCallback callback)
        {
            ResolveAsync(ipOrHost, delegate (NetAddress adr)
            {
                if (adr == null)
                {
                    callback(null);
                }
                else
                {
                    callback(new NetEndPoint(adr, port));
                }
            });
        }
        #endregion

        #region [Resolve] (hostname, port) -> IPv4 endpoint 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get IPv4 endpoint from notation (xxx.xxx.xxx.xxx) or hostname and port number
        /// </summary>
        public static NetEndPoint Resolve(string ipOrHost, int port)
        {
            var adr = Resolve(ipOrHost);
            return adr == null ? null : new NetEndPoint(adr, port);
        }
        #endregion

        #region [Resolve] (hostname) -> IPv4 endpoint (Async) 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
		/// Get IPv4 address from notation (xxx.xxx.xxx.xxx) or hostname (asynchronous version)
		/// </summary>
		public static void ResolveAsync(string ipOrHost, ResolveAddressCallback callback)
        {
            if (string.IsNullOrEmpty(ipOrHost))
                throw new ArgumentException("Supplied string must not be empty", "ipOrHost");

            ipOrHost = ipOrHost.Trim();
            
            if (NetAddress.TryParse(ipOrHost, out NetAddress ipAddress))
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    callback(ipAddress);
                    return;
                }
                throw new ArgumentException("This method will not currently resolve other than ipv4 addresses");
            }

            // ok must be a host name
            IPHostEntry entry;
            try
            {
                Dns.BeginGetHostEntry(ipOrHost, delegate (IAsyncResult result)
                {
                    try
                    {
                        entry = Dns.EndGetHostEntry(result);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.HostNotFound)
                        {
                            //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                            callback(null);
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (entry == null)
                    {
                        callback(null);
                        return;
                    }

                    // check each entry for a valid IP address
                    foreach (var ipCurrent in entry.AddressList)
                    {
                        if (ipCurrent.AddressFamily == AddressFamily.InterNetwork)
                        {
                            callback(ipCurrent);
                            return;
                        }
                    }

                    callback(null);
                }, null);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                    callback(null);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region [Resolve] (hostname) -> IPv4 endpoint 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get IPv4 address from notation (xxx.xxx.xxx.xxx) or hostname
        /// </summary>
        public static NetAddress Resolve(string ipOrHost)
        {
            if (string.IsNullOrEmpty(ipOrHost))
                throw new ArgumentException("Supplied string must not be empty", "ipOrHost");

            ipOrHost = ipOrHost.Trim();
            
            if (NetAddress.TryParse(ipOrHost, out NetAddress ipAddress))
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    return ipAddress;
                throw new ArgumentException("This method will not currently resolve other than ipv4 addresses");
            }

            // ok must be a host name
            try
            {
                var addresses = Dns.GetHostAddresses(ipOrHost);
                if (addresses == null)
                    return null;
                foreach (var address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        return address;
                }
                return null;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion


        #region [네트워크] BroadcastAddress
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static IPAddress s_broadcastAddress;
        public static IPAddress GetCachedBroadcastAddress()
        {
            if (s_broadcastAddress == null)
                s_broadcastAddress = GetBroadcastAddress();
            return s_broadcastAddress;
        }

        #endregion
        
        #region [네트워크] IsLocal (NetEndPoint, NetAddress)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the endpoint supplied is on the same subnet as this host
        /// </summary>
        public static bool IsLocal(NetEndPoint endPoint)
        {
            if (endPoint == null)
                return false;
            return IsLocal(endPoint.Address);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the IPAddress supplied is on the same subnet as this host
        /// </summary>
        public static bool IsLocal(NetAddress remote)
        {
            var local = GetMyAddress(out NetAddress mask);

            if (mask == null)
                return false;

            uint maskBits = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint remoteBits = BitConverter.ToUInt32(remote.GetAddressBytes(), 0);
            uint localBits = BitConverter.ToUInt32(local.GetAddressBytes(), 0);

            // compare network portions
            return ((remoteBits & maskBits) == (localBits & maskBits));
        }
        #endregion

        

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // BitControl / Byte Array
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [BitControl] BitsToHoldUInt
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns how many bits are necessary to hold a certain number
        /// </summary>
        //[CLSCompliant(false)]
        public static int BitsToHoldUInt(uint value)
        {
            int bits = 1;
            while ((value >>= 1) != 0)
                bits++;
            return bits;
        }

        /// <summary>
        /// Returns how many bits are necessary to hold a certain number
        /// </summary>
        //[CLSCompliant(false)]
        public static int BitsToHoldUInt64(ulong value)
        {
            int bits = 1;
            while ((value >>= 1) != 0)
                bits++;
            return bits;
        }

        /// <summary>
        /// Returns how many bytes are required to hold a certain number of bits
        /// </summary>
        public static int BytesToHoldBits(int numBits)
        {
            return (numBits + 7) / 8;
        }
        #endregion

        #region [BitControl] SwapByteOrder
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static UInt32 SwapByteOrder(UInt32 value)
        {
            return
                ((value & 0xff000000) >> 24) |
                ((value & 0x00ff0000) >> 8) |
                ((value & 0x0000ff00) << 8) |
                ((value & 0x000000ff) << 24);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static UInt64 SwapByteOrder(UInt64 value)
        {
            return
                ((value & 0xff00000000000000L) >> 56) |
                ((value & 0x00ff000000000000L) >> 40) |
                ((value & 0x0000ff0000000000L) >> 24) |
                ((value & 0x000000ff00000000L) >> 8) |
                ((value & 0x00000000ff000000L) << 8) |
                ((value & 0x0000000000ff0000L) << 24) |
                ((value & 0x000000000000ff00L) << 40) |
                ((value & 0x00000000000000ffL) << 56);
        }
        #endregion

        #region [Byte Array] Compare
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static bool CompareElements(byte[] one, byte[] two)
        {
            if (one.Length != two.Length)
                return false;
            for (int i = 0; i < one.Length; i++)
                if (one[i] != two[i])
                    return false;
            return true;
        }
        #endregion

        #region [Byte Array] from hexString
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Convert a hexadecimal string to a byte array
        /// </summary>
        public static byte[] ToByteArray(String hexString)
        {
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }
        #endregion

        #region [HumanReadable] long -> string
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts a number of bytes to a shorter, more readable string representation
        /// </summary>
        public static string ToHumanReadable(long bytes)
        {
            if (bytes < 4000) // 1-4 kb is printed in bytes
                return bytes + " bytes";
            if (bytes < 1000 * 1000) // 4-999 kb is printed in kb
                return Math.Round(((double)bytes / 1000.0), 2) + " kilobytes";
            return Math.Round(((double)bytes / (1000.0 * 1000.0)), 2) + " megabytes"; // else megabytes
        }
        #endregion

       
        #region [HexString] Convert
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
		/// Create a hex string from an Int64 value
		/// </summary>
		public static string ToHexString(long data)
        {
            return ToHexString(BitConverter.GetBytes(data));
        }

        /// <summary>
        /// Create a hex string from an array of bytes
        /// </summary>
        public static string ToHexString(byte[] data)
        {
            return ToHexString(data, 0, data.Length);
        }

        /// <summary>
        /// Create a hex string from an array of bytes
        /// </summary>
        public static string ToHexString(byte[] data, int offset, int length)
        {
            char[] c = new char[length * 2];
            byte b;
            for (int i = 0; i < length; ++i)
            {
                b = ((byte)(data[offset + i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(data[offset + i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }
        #endregion
        
        #region [SHAHash] Compute
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] ComputeSHAHash(byte[] bytes)
        {
            // this is defined in the platform specific files
            return ComputeSHAHash(bytes, 0, bytes.Length);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] RelativeSequenceNumber
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static int RelativeSequenceNumber(int nr, int expected)
        {
            return (nr - expected + NetBase.NumSequenceNumbers + (NetBase.NumSequenceNumbers / 2)) % NetBase.NumSequenceNumbers - (NetBase.NumSequenceNumbers / 2);

            // old impl:
            //int retval = ((nr + NetConstants.NumSequenceNumbers) - expected) % NetConstants.NumSequenceNumbers;
            //if (retval > (NetConstants.NumSequenceNumbers / 2))
            //	retval -= NetConstants.NumSequenceNumbers;
            //return retval;
        }
        #endregion
        
        #region [유틸] MakeCommaDelimitedList
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a comma delimited string from a lite of items
        /// </summary>
        public static string MakeCommaDelimitedList<T>(IList<T> list)
        {
            var cnt = list.Count;
            StringBuilder bdr = new StringBuilder(cnt * 5); // educated guess
            for (int i = 0; i < cnt; i++)
            {
                bdr.Append(list[i].ToString());
                if (i != cnt - 1)
                    bdr.Append(", ");
            }
            return bdr.ToString();
        }
        #endregion
        


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Temp: BinaryFormatter/Serialize
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Temp] BinaryFormatter/Serialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static NamedPipePacket makeNamedPipePacket(char type, int number, Object objectPacket)
        //{
        //    NamedPipePacket namedPipePacket = new NamedPipePacket();
        //    NPHeader nPHeader = new NPHeader();
        //    nPHeader.type = type;
        //    nPHeader.number = number;
        //    namedPipePacket.byteDate = PacketUtils.ObjectToBytes(objectPacket);
        //    nPHeader.dataSize = namedPipePacket.byteDate.Length;
        //    namedPipePacket.nPHeader = nPHeader;

        //    return namedPipePacket;
        //}

        //public static byte[] ObjectToBytes(Object obj)
        //{
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        binaryFormatter.Serialize(memoryStream, obj);
        //        return memoryStream.ToArray();
        //    }
        //}

        //public static Object BytesToObject(byte[] bytes)
        //{
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    using (MemoryStream memoryStream = new MemoryStream(bytes))
        //    {
        //        return binaryFormatter.Deserialize(memoryStream);
        //    }
        //}
        #endregion

        #region [백업] GetWindowSize/GetDeliveryMethod
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Gets the window size used internally in the library for a certain delivery method
        ///// </summary>
        //public static int GetWindowSize(NetDeliveryMethod method)
        //{
        //	switch (method)
        //	{
        //		case NetDeliveryMethod.Unknown:
        //			return 0;

        //		case NetDeliveryMethod.Unreliable:
        //		case NetDeliveryMethod.UnreliableSequenced:
        //			return NetBase.UnreliableWindowSize;

        //		case NetDeliveryMethod.ReliableOrdered:
        //			return NetBase.ReliableOrderedWindowSize;

        //		case NetDeliveryMethod.ReliableSequenced:
        //		case NetDeliveryMethod.ReliableUnordered:
        //		default:
        //			return NetBase.DefaultWindowSize;
        //	}
        //}

        //internal static NetDeliveryMethod GetDeliveryMethod(NetMessageType mtp)
        //{
        //	if (mtp >= NetMessageType.UserReliableOrdered1)
        //		return NetDeliveryMethod.ReliableOrdered;
        //	else if (mtp >= NetMessageType.UserReliableSequenced1)
        //		return NetDeliveryMethod.ReliableSequenced;
        //	else if (mtp >= NetMessageType.UserReliableUnordered)
        //		return NetDeliveryMethod.ReliableUnordered;
        //	else if (mtp >= NetMessageType.UserSequenced1)
        //		return NetDeliveryMethod.UnreliableSequenced;
        //	return NetDeliveryMethod.Unreliable;
        //}
        #endregion

    }
}