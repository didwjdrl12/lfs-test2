using J2y.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;



public static partial class eRpcTarget
{
    public const int None = 0x00000000;
    public const int Client = 0x00000001;
    public const int Server = 0x00000004;  // [C->S] 서버 함수 호출 (default: MainServer)
    public const int MainServer = 0x00000004;  // == Server 
    public const int FieldServer = 0x00000008;  // == Server
    public const int Multicast = 0x00000010;  // [C->S->C] 주변 클라이언트에 전파 (본인도 호출)
    public const int ExceptSelf = 0x00000020;
}



namespace J2y
{
    #region [enum] Replication Type
    public enum eReplicationType
    {
        Function,       // Method Invoke
        Actor,          // Spawn Actor
        Variable,       // Variable
    }
    #endregion

    
	#region [enum] Rpc Result
	public enum eRpcResult
	{
		Succeed,
		NoAuthority,        // AutonomousProxy 또는 Authority의 권한이 필요
        WrongArguments,     // 호출 인자 에러
        UnknownError,		
	}
	#endregion



	#region [enum] Net Role
	public enum eNetRole
    {
        None,               // Replication X
        SimulatedProxy,     // 리모트 액터 (권한은 서버에 있음, ex:상대방 캐릭터)
        AutonomousProxy,    // 내가 제어 하는 액터 (권한은 서버에 있음, ex:내 캐릭터)
        Authority,          // 권한 갖고 있음 (서버 또는 클라만 존재하는 객체)
    }
    #endregion




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [enum] RpcTarget
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++





    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [Attribute] RPC
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Attribute] RPC

    [System.AttributeUsage(AttributeTargets.Method)]
    public class RPC : Attribute
    {
        private int _rpc_type;

        public RPC(int rpc_type)
        {
            _rpc_type = rpc_type;
        }
    }
    #endregion





    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JReplicationHandler
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Replication] Handler
    public class JReplicationHelper
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 
        public NetPeer_base _net_sender;
        public string _rpc_func_name;
        #endregion


    }
    #endregion

}


