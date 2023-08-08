using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;


namespace J2y.Network
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // interface 
    //		��Ŷ �ڵ鷯
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessageHandler
    {
        // ����	
        void RegisterMessageHandler(int message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        void RegisterSimpleMessageHandler(string message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        void RegisterShortMessageHandler(int message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        //void OnServerConnected();
        //void OnServerDisconnected();

        // ����
        eNetSendResult SendMessage(JNetMessage message);
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // interface
    //      �ݸ޽��� ��ε�ĳ����
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessage_broadcaster
    {
        void Broadcast(JNetMessage message);
        void NetBroadcastExcept(JNetMessage message, NetPeer_base except);
    }


}


