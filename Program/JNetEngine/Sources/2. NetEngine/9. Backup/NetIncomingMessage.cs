using System;
using System.Net;
using System.Diagnostics;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace J2y.Network
{
	////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	////
	//// NetIncomingMessage
	////		Incoming message either sent from a remote peer or generated within the library
	////
	////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	//[DebuggerDisplay("Type={MessageType} LengthBits={LengthBits}")]
	//public sealed class NetIncomingMessage : NetBuffer_lid
	//{
	//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	// Variable
	//	//
	//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	//	internal NetIncomingMessageType m_incomingMessageType;
	//	internal NetEndPoint m_senderEndPoint;
	//	internal NetPeer_base m_senderConnection;
	//	internal int m_sequenceNumber;
	//	internal NetMessageType m_receivedMessageType;
	//	internal bool m_isFragment;
	//	internal double m_receiveTime;

	//	/// <summary>
	//	/// Gets the type of this incoming message
	//	/// </summary>
	//	public NetIncomingMessageType MessageType { get { return m_incomingMessageType; } }

	//	/// <summary>
	//	/// Gets the delivery method this message was sent with (if user data)
	//	/// </summary>
	//	public NetDeliveryMethod DeliveryMethod { get { return NetUtility.GetDeliveryMethod(m_receivedMessageType); } }

	//	/// <summary>
	//	/// Gets the sequence channel this message was sent with (if user data)
	//	/// </summary>
	//	public int SequenceChannel { get { return (int)m_receivedMessageType - (int)NetUtility.GetDeliveryMethod(m_receivedMessageType); } }

	//	/// <summary>
	//	/// endpoint of sender, if any
	//	/// </summary>
	//	public NetEndPoint SenderEndPoint { get { return m_senderEndPoint; } }

	//	/// <summary>
	//	/// NetPeer_base of sender, if any
	//	/// </summary>
	//	public NetPeer_base SenderConnection { get { return m_senderConnection; } }

	//	/// <summary>
	//	/// What local time the message was received from the network
	//	/// </summary>
	//	public double ReceiveTime { get { return m_receiveTime; } }


	//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	// 
	//	//
	//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	//	internal NetIncomingMessage()
	//	{
	//	}

	//	internal NetIncomingMessage(NetIncomingMessageType tp)
	//	{
	//		m_incomingMessageType = tp;
	//	}

	//	internal void Reset()
	//	{
	//		m_incomingMessageType = NetIncomingMessageType.Error;
	//		m_readPosition = 0;
	//		m_receivedMessageType = NetMessageType.LibraryError;
	//		m_senderConnection = null;
	//		m_bitLength = 0;
	//		m_isFragment = false;
	//	}

	//	/// <summary>
	//	/// Decrypt a message
	//	/// </summary>
	//	/// <param name="encryption">The encryption algorithm used to encrypt the message</param>
	//	/// <returns>true on success</returns>
	//	public bool Decrypt(NetEncryption encryption)
	//	{
	//		return encryption.Decrypt(this);
	//	}

	//	/// <summary>
	//	/// Reads a value, in local time comparable to NetTime.Now, written using WriteTime()
	//	/// Must have a connected sender
	//	/// </summary>
	//	public double ReadTime(bool highPrecision)
	//	{
	//		return ReadTime(m_senderConnection, highPrecision);
	//	}

	//	/// <summary>
	//	/// Returns a string that represents this object
	//	/// </summary>
	//	public override string ToString()
	//	{
	//		return "[NetIncomingMessage #" + m_sequenceNumber + " " + this.LengthBytes + " bytes]";
	//	}
	//}
}
