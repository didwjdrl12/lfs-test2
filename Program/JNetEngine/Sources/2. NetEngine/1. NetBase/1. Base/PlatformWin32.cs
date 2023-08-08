#if !__ANDROID__ && !__CONSTRAINED__ && !WINDOWS_RUNTIME && !UNITY_STANDALONE_LINUX
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace J2y.Network
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetUtility
    //		
    //      1. NetworkInterface
    //      2. MacAddress
    //      3. GetMyAddress
    //      4. GetBroadcastAddress
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class NetUtility
	{
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [readonly] Stopwatch
        private static readonly long s_timeInitialized = Stopwatch.GetTimestamp();
        private static readonly double s_dInvFreq = 1.0 / (double)Stopwatch.Frequency;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 네트워크
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        #region [네트워크] Get NetworkInterface 
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        private static NetworkInterface GetNetworkInterface()
        {
            var computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            if (computerProperties == null)
                return null;

            var nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
                return null;

            NetworkInterface best = null;
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback || adapter.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                    continue;
                if (!adapter.Supports(NetworkInterfaceComponent.IPv4))
                    continue;
                if (best == null)
                    best = adapter;
                if (adapter.OperationalStatus != OperationalStatus.Up)
                    continue;

                // make sure this adapter has any ipv4 addresses
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach (UnicastIPAddressInformation unicastAddress in properties.UnicastAddresses)
                {
                    if (unicastAddress != null && unicastAddress.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Yes it does, return this network interface.
                        return adapter;
                    }
                }
            }
            return best;
        }
        #endregion

        #region [네트워크] Get MacAddress (Bytes)
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        /// <summary>
        /// If available, returns the bytes of the physical (MAC) address for the first usable network interface
        /// </summary>
        public static byte[] GetMacAddressBytes()
        {
            var ni = GetNetworkInterface();
            if (ni == null)
                return null;
            return ni.GetPhysicalAddress().GetAddressBytes();
        }

        #endregion

        #region [네트워크] Get BroadcastAddress
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static IPAddress GetBroadcastAddress()
        {
            var ni = GetNetworkInterface();
            if (ni == null)
                return null;

            var properties = ni.GetIPProperties();
            foreach (UnicastIPAddressInformation unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress != null && unicastAddress.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    var mask = unicastAddress.IPv4Mask;
                    byte[] ipAdressBytes = unicastAddress.Address.GetAddressBytes();
                    byte[] subnetMaskBytes = mask.GetAddressBytes();

                    if (ipAdressBytes.Length != subnetMaskBytes.Length)
                        throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

                    byte[] broadcastAddress = new byte[ipAdressBytes.Length];
                    for (int i = 0; i < broadcastAddress.Length; i++)
                    {
                        broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
                    }
                    return new IPAddress(broadcastAddress);
                }
            }
            return IPAddress.Broadcast;
        }

        #endregion

        #region [네트워크] Get MyAddress
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Gets my local IPv4 address (not necessarily external) and subnet mask
        /// </summary>
        public static IPAddress GetMyAddress(out IPAddress mask)
        {
            var ni = GetNetworkInterface();
            if (ni == null)
            {
                mask = null;
                return null;
            }

            IPInterfaceProperties properties = ni.GetIPProperties();
            foreach (UnicastIPAddressInformation unicastAddress in properties.UnicastAddresses)
            {
                if (unicastAddress != null && unicastAddress.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    mask = unicastAddress.IPv4Mask;
                    return unicastAddress.Address;
                }
            }

            mask = null;
            return null;
        }

        #endregion

        #region [네트워크] Create Address From Bytes
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static IPAddress CreateAddressFromBytes(byte[] bytes)
        {
            return new IPAddress(bytes);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] [Hash] Compute SHA256
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        private static readonly SHA256 s_sha = SHA256.Create();
        public static byte[] ComputeSHAHash(byte[] bytes, int offset, int count)
        {
            return s_sha.ComputeHash(bytes, offset, count);
        }
        #endregion

      
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Timer/Speed
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] Now
        public static double Now { get { return (double)(Stopwatch.GetTimestamp() - s_timeInitialized) * s_dInvFreq; } }
        #endregion
        
        #region [유틸] GetPlatformSeed
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        //[CLSCompliant(false)]
        public static ulong GetPlatformSeed(int seedInc)
        {
            ulong seed = (ulong)System.Diagnostics.Stopwatch.GetTimestamp();
            return seed ^ ((ulong)Environment.WorkingSet + (ulong)seedInc);
        }
        #endregion

        #region [유틸] Sleep
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        #endregion
        
    }

    #region [Class] NetTime
    public static partial class NetTime
    {
        private static readonly long s_timeInitialized = Stopwatch.GetTimestamp();
        private static readonly double s_dInvFreq = 1.0 / (double)Stopwatch.Frequency;

        /// <summary>
        /// Get number of seconds since the application started
        /// </summary>
        public static double Now { get { return (double)(Stopwatch.GetTimestamp() - s_timeInitialized) * s_dInvFreq; } }
    }
    #endregion



}
#endif