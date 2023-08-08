﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace J2y.Network
{
	internal enum MessageResendReason
	{
		Delay,
		HoleInSequence
	}

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// NetConnectionStatistics
	//		Statistics for a NetPeer_base instance
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		
	public sealed class NetConnectionStatistics
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Variable
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		private readonly NetTcpPeer_base m_connection;

		internal long m_sentPackets;
		internal long m_receivedPackets;

		internal long m_sentMessages;
		internal long m_receivedMessages;
		internal long m_droppedMessages;
		internal long m_receivedFragments;

		internal long m_sentBytes;
		internal long m_receivedBytes;

		internal long m_resentMessagesDueToDelay;
		internal long m_resentMessagesDueToHole;



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		/// <summary>
		/// Gets the number of sent packets for this connection
		/// </summary>
		public long SentPackets { get { return m_sentPackets; } }

		/// <summary>
		/// Gets the number of received packets for this connection
		/// </summary>
		public long ReceivedPackets { get { return m_receivedPackets; } }

		/// <summary>
		/// Gets the number of sent bytes for this connection
		/// </summary>
		public long SentBytes { get { return m_sentBytes; } }

		/// <summary>
		/// Gets the number of received bytes for this connection
		/// </summary>
		public long ReceivedBytes { get { return m_receivedBytes; } }

		/// <summary>
		/// Gets the number of sent messages for this connection
		/// </summary>
		public long SentMessages { get { return m_sentMessages; } }

		/// <summary>
		/// Gets the number of received messages for this connection
		/// </summary>
		public long ReceivedMessages { get { return m_receivedMessages; } }

		/// <summary>
		/// Gets the number of resent reliable messages for this connection
		/// </summary>
		public long ResentMessages { get { return m_resentMessagesDueToHole + m_resentMessagesDueToDelay; } }

		/// <summary>
		/// Gets the number of dropped messages for this connection
		/// </summary>
		public long DroppedMessages { get { return m_droppedMessages; } }

		// public double LastSendRespondedTo { get { return m_connection.m_lastSendRespondedTo; } }



		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		internal NetConnectionStatistics(NetTcpPeer_base conn)
		{
			m_connection = conn;
			Reset();
		}

		internal void Reset()
		{
			m_sentPackets = 0;
			m_receivedPackets = 0;
			m_sentMessages = 0;
			m_receivedMessages = 0;
			m_receivedFragments = 0;
			m_sentBytes = 0;
			m_receivedBytes = 0;
			m_resentMessagesDueToDelay = 0;
			m_resentMessagesDueToHole = 0;
		}



#if !USE_RELEASE_STATISTICS
		[Conditional("DEBUG")]
#endif
		internal void PacketSent(int numBytes, int numMessages)
		{
			NetException.Assert(numBytes > 0 && numMessages > 0);
			m_sentPackets++;
			m_sentBytes += numBytes;
			m_sentMessages += numMessages;
		}

#if !USE_RELEASE_STATISTICS
		[Conditional("DEBUG")]
#endif
		internal void PacketReceived(int numBytes, int numMessages, int numFragments)
		{
			NetException.Assert(numBytes > 0 && numMessages > 0);
			m_receivedPackets++;
			m_receivedBytes += numBytes;
			m_receivedMessages += numMessages;
			m_receivedFragments += numFragments;
		}

#if !USE_RELEASE_STATISTICS
		[Conditional("DEBUG")]
#endif
		internal void MessageResent(MessageResendReason reason)
		{
			if (reason == MessageResendReason.Delay)
				m_resentMessagesDueToDelay++;
			else
				m_resentMessagesDueToHole++;
		}

#if !USE_RELEASE_STATISTICS
		[Conditional("DEBUG")]
#endif
		internal void MessageDropped()
		{
			m_droppedMessages++;
		}

		/// <summary>
		/// Returns a string that represents this object
		/// </summary>
		public override string ToString()
		{
			StringBuilder bdr = new StringBuilder();
			////bdr.AppendLine("Average roundtrip time: " + NetTime.ToReadable(m_connection.m_averageRoundtripTime));
			//bdr.AppendLine("Current MTU: " + m_connection.m_currentMTU);
			//bdr.AppendLine("Sent " + m_sentBytes + " bytes in " + m_sentMessages + " messages in " + m_sentPackets + " packets");
			//bdr.AppendLine("Received " + m_receivedBytes + " bytes in " + m_receivedMessages + " messages (of which " + m_receivedFragments + " fragments) in " + m_receivedPackets + " packets");
			//bdr.AppendLine("Dropped " + m_droppedMessages + " messages (dupes/late/early)");

			//if (m_resentMessagesDueToDelay > 0)
			//	bdr.AppendLine("Resent messages (delay): " + m_resentMessagesDueToDelay);
			//if (m_resentMessagesDueToHole > 0)
			//	bdr.AppendLine("Resent messages (holes): " + m_resentMessagesDueToHole);

			//int numUnsent = 0;
			//int numStored = 0;
			//foreach (NetSenderChannelBase sendChan in m_connection.m_sendChannels)
			//{
			//	if (sendChan == null)
			//		continue;
			//	numUnsent += sendChan.QueuedSendsCount;

			//	var relSendChan = sendChan as NetReliableSenderChannel;
			//	if (relSendChan != null)
			//	{
			//		for (int i = 0; i < relSendChan.m_storedMessages.Length; i++)
			//			if (relSendChan.m_storedMessages[i].Message != null)
			//				numStored++;
			//	}
			//}

			//int numWithheld = 0;
			//foreach (NetReceiverChannelBase recChan in m_connection.m_receiveChannels)
			//{
			//	var relRecChan = recChan as NetReliableOrderedReceiver;
			//	if (relRecChan != null)
			//	{
			//		for (int i = 0; i < relRecChan.m_withheldMessages.Length; i++)
			//			if (relRecChan.m_withheldMessages[i] != null)
			//				numWithheld++;
			//	}
			//}

			//bdr.AppendLine("Unsent messages: " + numUnsent);
			//bdr.AppendLine("Stored messages: " + numStored);
			//bdr.AppendLine("Withheld messages: " + numWithheld);

			return bdr.ToString();
		}
	}
}