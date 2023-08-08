using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;


namespace J2y.Network
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // interface 
    //		패킷 핸들러
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessageHandler
    {
        // 수신	
        void RegisterMessageHandler(int message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        void RegisterSimpleMessageHandler(string message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        void RegisterShortMessageHandler(int message_type, Action<NetPeer_base, BinaryReader> handler, bool overwrite);
        //void OnServerConnected();
        //void OnServerDisconnected();

        // 전송
        eNetSendResult SendMessage(JNetMessage message);
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // interface
    //      넷메시지 브로드캐스터
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessage_broadcaster
    {
        void Broadcast(JNetMessage message);
        void NetBroadcastExcept(JNetMessage message, NetPeer_base except);
    }


}


