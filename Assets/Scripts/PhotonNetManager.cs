using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;      //引用Photon命名空间
using ExitGames.Client.Photon.Lite; //引用Lite命名空间
using System;

public class PhotonNetManager : MonoBehaviour, IPhotonPeerListener      //实现Photon接口
{
	private PhotonPeer _photonPeer;
	private PhotonPeer photonPeer
	{
		get
		{
			if(null == _photonPeer)
			{
				_photonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);      //初始化PhotonPeer,选择UDP协议
			}
			return _photonPeer;
		}
	}
	void Start()
	{
		photonPeer.Connect("192.168.0.100:5055", "Lite");                 //连接
	}

	void Update()
	{
		photonPeer.Service();               //用来监听服务器的响应，必须不停执行
	}
	public void SendPhotonMessage()                                     //发送请求至Photon
	{
		Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
		tParaDic[LiteOpKey.GameId] = "1";
		photonPeer.OpCustom(LiteOpKey.GameId, tParaDic, true);//调用客户端向服务器发送请求的函数//重载1：参数1:命令代码;参数2:命令带有的参数;参数3:命令是否可信(一般为true)
	}
	public void SendOperationReqMsg(OperationRequest _operationRequest = null)
	{
		_operationRequest = new OperationRequest();
		_operationRequest.OperationCode = LiteOpCode.Join;
		Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
		_operationRequest.Parameters = tParaDic;
		_operationRequest.Parameters[LiteOpKey.GameId] = "2";
		photonPeer.OpCustom(_operationRequest, true, 0, true);
	}
	public void DebugReturn(DebugLevel _level, string _message)
	{
		Debug.LogError(_level.ToString() + "...." + _message);
	}

	public void OnEvent(EventData _eventData)
	{
		Debug.LogError("事件触发：" + _eventData.Parameters[LiteOpKey.ActorNr]);
	}

	public void OnOperationResponse(OperationResponse _operationResponse)
	{
		Debug.Log("服务器返回响应：" + _operationResponse.ToString() + "   响应类型：" + _operationResponse.OperationCode);
		switch(_operationResponse.OperationCode)
		{
			case LiteOpCode.Join:
			int PlayerNum = (int)_operationResponse.Parameters[LiteOpKey.ActorNr];  //返回进入游戏房间的玩家的编号
			Debug.LogError("进入游戏房间,玩家编号：" + PlayerNum);
			//Debug.LogError("进入游戏房间,玩家编号：" + PlayerNum);

			break;
			case LiteOpCode.Leave:
			Debug.LogError("离开游戏房间");
			break;
			default:
			break;
		}
	}

	public void OnStatusChanged(StatusCode _statusCode)
	{
		switch(_statusCode)
		{
			case StatusCode.Connect:
			Debug.LogError("Connect Success!!!");
			break;
			case StatusCode.Disconnect:
			Debug.LogError("Connect Lost!!!");
			break;
			case StatusCode.Exception:
			Debug.LogError("Connect Exception!!!");

			break;
			case StatusCode.ExceptionOnConnect:
			break;
			case StatusCode.SecurityExceptionOnConnect:
			break;
			case StatusCode.QueueOutgoingReliableWarning:
			break;
			case StatusCode.QueueOutgoingUnreliableWarning:
			break;
			case StatusCode.SendError:
			break;
			case StatusCode.QueueOutgoingAcksWarning:
			break;
			case StatusCode.QueueIncomingReliableWarning:
			break;
			case StatusCode.QueueIncomingUnreliableWarning:
			break;
			case StatusCode.QueueSentWarning:
			break;
			//case StatusCode.ExceptionOnReceive:
			//break;
			//case StatusCode.InternalReceiveException:
			//break;
			case StatusCode.TimeoutDisconnect:
			break;
			case StatusCode.DisconnectByServer:
			break;
			case StatusCode.DisconnectByServerUserLimit:
			break;
			case StatusCode.DisconnectByServerLogic:
			break;
			case StatusCode.TcpRouterResponseOk:
			break;
			case StatusCode.TcpRouterResponseNodeIdUnknown:
			break;
			case StatusCode.TcpRouterResponseEndpointUnknown:
			break;
			case StatusCode.TcpRouterResponseNodeNotReady:
			break;
			case StatusCode.EncryptionEstablished:
			break;
			case StatusCode.EncryptionFailedToEstablish:
			break;
			default:
			break;
		}
	}


}
