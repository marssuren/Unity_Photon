using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;      //引用Photon命名空间
using ExitGames.Client.Photon.Lite; //引用Lite命名空间
using System;
using UnityEngine.UI;

public class PhotonNetManager : MonoBehaviour, IPhotonPeerListener      //实现Photon接口
{
	private PhotonPeer _photonPeer;
	private PhotonPeer photonPeer       //通用的Photon操作类
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
	private LitePeer _litePeer;          //比PhotonPeer更强大的基于游戏房间的操作类
	private LitePeer litePeer
	{
		get
		{
			if(null == _litePeer)
			{
				_litePeer = new LitePeer(this, ConnectionProtocol.Udp);
			}
			return _litePeer;
		}
	}
	[SerializeField]
	private InputField playerNameField;
	[SerializeField]
	private Text userAnnounce;

	private Dictionary<int, string> actorDic = new Dictionary<int, string>();   //用于存放已经加入游戏的玩家信息
	void Start()
	{
		//photonPeer.Connect("192.168.0.100:5055", "Lite");                 //连接
		litePeer.Connect("192.168.0.100:5055", "Lite");
	}
	void Update()
	{
		//photonPeer.Service();               //用来监听服务器的响应，必须不停执行
		litePeer.Service();
	}
	public void SendPhotonMessage()                                     //发送请求至Photon
	{
		Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
		tParaDic[LiteOpKey.GameId] = "1";
		photonPeer.OpCustom(LiteOpKey.GameId, tParaDic, true);//调用客户端向服务器发送请求的函数//重载1：参数1:命令代码;参数2:命令带有的参数;参数3:命令是否可信(一般为true)
	}
	public void SendOperationReqMsg()       //使用OperationRequest封装请求信息
	{
		OperationRequest operationRequest = new OperationRequest();
		operationRequest.OperationCode = LiteOpCode.Join;
		Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
		operationRequest.Parameters = tParaDic;
		operationRequest.Parameters[LiteOpKey.GameId] = "2";
		photonPeer.OpCustom(operationRequest, true, 0, false);
	}
	public void LitePeerJoin()           //使用LitePeer发送消息至服务器,加入游戏房间
	{
		//litePeer.OpJoin("Unity_Photon");

		Hashtable tGameProperties = new Hashtable();

		Hashtable tActorProperties = new Hashtable();   //玩家属性
		tActorProperties.Add(ChatEventKey.UserName, playerNameField.text);
		litePeer.OpJoin("Unity_Photon", tGameProperties, tActorProperties, true);          //参数1：设置游戏(房间)的名称；参数2：传递游戏的属性；参数3：传递玩家的属性；参数4：是否广播玩家属性
	}
	public void LitePeerLeave()         //离开游戏房间,之后客户端可以进行资源清理等操作
	{
		litePeer.OpLeave();
	}
	public void LiteChat()
	{
		Hashtable tChatMessage = new Hashtable();
		string tFieldText = "hhhh";
		tChatMessage.Add(ChatEventKey.ChatMessage, tFieldText);
		litePeer.OpRaiseEvent((byte)ChatEventCode.Chat, tChatMessage, true);
	}

	public void DebugReturn(DebugLevel _level, string _message)
	{
		Debug.LogError(_level.ToString() + "...." + _message);
	}

	public void OnEvent(EventData _eventData)
	{
		Debug.LogError("事件触发：" + _eventData.Parameters[LiteOpKey.ActorNr]);
		switch(_eventData.Code)
		{
			case LiteEventCode.Join:
			{
				string tActorName = ((Hashtable)_eventData.Parameters[LiteEventKey.ActorProperties])[(byte)ChatEventKey.UserName].ToString();     //获取玩家姓名
				int tActorNum = (int)_eventData.Parameters[LiteEventKey.ActorNr];   //获取玩家编号
				userAnnounce.text = "玩家" + tActorName + "进入了游戏";
				if(!actorDic.ContainsKey(tActorNum))        //加入新入玩家信息
				{
					actorDic.Add(tActorNum, tActorName);
				}
				else
				{
					actorDic[tActorNum] = tActorName;       //更新已入玩家信息
				}
			}
			break;
			case LiteEventCode.Leave:
			{
				//Debug.LogError("!!!!");
				int tActorNum = (int)_eventData.Parameters[LiteEventKey.ActorNr];
				Debug.LogError("!!!!" + tActorNum);
				if(actorDic.ContainsKey(tActorNum))
				{
					string tActorName = actorDic[tActorNum];
					userAnnounce.text = "玩家:" + tActorName + "离开了游戏";
					actorDic.Remove(tActorNum);
				}
			}
			break;
			case (byte)ChatEventCode.Chat:
			{
				Hashtable tContentTable = (Hashtable)_eventData.Parameters[LiteEventKey.Data];
				string tContentStr = tContentTable[(byte)ChatEventKey.ChatMessage].ToString();
				int tActorNum = (int)_eventData.Parameters[LiteEventKey.ActorNr];
				if(actorDic.ContainsKey(tActorNum))
				{
					string tUserName = actorDic[tActorNum];
					userAnnounce.text = "玩家" + tUserName + "说：" + tContentStr;
				}
			}
			break;
		}
	}

	public void OnOperationResponse(OperationResponse _operationResponse)
	{
		Debug.Log("服务器返回响应：" + _operationResponse.Parameters + "   响应类型：" + _operationResponse.OperationCode);
		switch(_operationResponse.OperationCode)
		{
			case LiteOpCode.Join:
			{
				int PlayerNum = (int)_operationResponse.Parameters[LiteOpKey.ActorNr]; //返回进入游戏房间的玩家的编号
				Debug.LogError("进入游戏房间,玩家编号：" + PlayerNum);
			}
			break;
			case LiteOpCode.Leave:
			{
				//Debug.Log("离开游戏房间");
			}
			break;
			default:
			{
				excuteMessage(_operationResponse);
			}
			break;
		}
	}
	void excuteMessage(OperationResponse _operationResponse)        //用于处理除了进入和离开房间以外的其他响应
	{
		//switch(_operationResponse.Parameters)
		//{
		//	default:
		//	break;
		//}

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
public enum ChatEventCode : byte            //仿照LiteEventCode自定义用于聊天功能的ChatEventCode
{
	Chat = 10,
}
public enum ChatEventKey : byte             //自定义用于聊天功能的ChatEventKey
{
	UserName = 11,
	ChatMessage = 12,

}








