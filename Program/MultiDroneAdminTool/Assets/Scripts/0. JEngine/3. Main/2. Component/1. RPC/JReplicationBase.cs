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
    public const int Server = 0x00000004;  // [C->S] ���� �Լ� ȣ�� (default: MainServer)
    public const int MainServer = 0x00000004;  // == Server 
    public const int FieldServer = 0x00000008;  // == Server
    public const int Multicast = 0x00000010;  // [C->S->C] �ֺ� Ŭ���̾�Ʈ�� ���� (���ε� ȣ��)
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
		NoAuthority,        // AutonomousProxy �Ǵ� Authority�� ������ �ʿ�
        WrongArguments,     // ȣ�� ���� ����
        UnknownError,		
	}
	#endregion



	#region [enum] Net Role
	public enum eNetRole
    {
        None,               // Replication X
        SimulatedProxy,     // ����Ʈ ���� (������ ������ ����, ex:���� ĳ����)
        AutonomousProxy,    // ���� ���� �ϴ� ���� (������ ������ ����, ex:�� ĳ����)
        Authority,          // ���� ���� ���� (���� �Ǵ� Ŭ�� �����ϴ� ��ü)
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


