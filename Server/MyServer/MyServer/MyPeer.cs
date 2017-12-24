using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Lite.Operations;
using Lite;

namespace MyServer
{
	class MyPeer : PeerBase
	{
		public MyPeer(InitRequest initRequest)
			: base(initRequest)
		{
		}

		//public MyPeer(IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
		//    : base(protocol, unmanagedPeer)
		//{
		//}

		protected override void OnOperationRequest(OperationRequest _operationRequest, SendParameters sendParameters)
		{
			switch(_operationRequest.OperationCode)
			{
				case (byte)OperationCode.Join:
				Hashtable tGameProperties = (Hashtable)_operationRequest.Parameters[(byte)ParameterKey.GameProperties];
				Hashtable tActorProperties =
					(Hashtable)_operationRequest.Parameters[(byte)ParameterKey.ActorProperties];
				string tUserName = tActorProperties[(byte)ChatEventKey.UserName].ToString();
				OperationResponse tOperationResponse = new OperationResponse();
				tOperationResponse.ReturnCode = 0;  //0表示返回成功
				tOperationResponse.OperationCode = (byte)OperationCode.Join; //告诉客户端做出了哪种类型的响应
				tOperationResponse.DebugMessage = "进入游戏房间成功！";
				Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
				tParaDic[(byte)ParameterKey.ActorNr] = 1;
				tOperationResponse.Parameters = tParaDic;
				SendOperationResponse(tOperationResponse, sendParameters);
				break;
				default:
				break;
			}
		}

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			throw new NotImplementedException();
		}
	}
}
//public enum ChatEventCode : byte            //仿照LiteEventCode自定义用于聊天功能的ChatEventCode
//{
//	Chat = 10,
//}
//public enum ChatEventKey : byte             //自定义用于聊天功能的ChatEventKey
//{
//	UserName = 11,
//	ChatMessage = 12,
//}
