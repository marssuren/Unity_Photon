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
    class MyPeer : LitePeer
    {
        //public MyPeer(InitRequest initRequest)
        //	: base(initRequest)
        //{
        //}
        Dictionary<int, int> customerScoresDic = new Dictionary<int, int>();
        public MyPeer(InitRequest initRequest)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            customerScoresDic.Add(1, 100);
            customerScoresDic.Add(2, 200);
        }
        //public MyPeer(IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
        //    : base(protocol, unmanagedPeer)
        //{
        //}

        protected override void OnOperationRequest(OperationRequest _operationRequest, SendParameters sendParameters)
        {
            //MyPeer继承了LitePeer，直接调用父类提供的Response
            if (_operationRequest.OperationCode >= 248 && _operationRequest.OperationCode <= 255)
            {
                base.OnOperationRequest(_operationRequest, sendParameters); //常用的房间操作，直接调用父类提供的实现去处理
            }
            //处理非248-255的请求，实现不属于房间操作之类的请求
            else
            {
                OperationResponse tOperationResponse = ExcuteCustomerOperation(_operationRequest);
                SendOperationResponse(tOperationResponse, sendParameters);  //调用API返回信息给客户端
            }
        }

        OperationResponse ExcuteCustomerOperation(OperationRequest _operationRequest)   //处理客户端自定义请求
        {
            OperationResponse tOperationResponse = new OperationResponse();
            switch (_operationRequest.OperationCode)
            {
                case (byte)CustomOperationCode.GetMyScore:
                    //1.需要获取客户端传递的参数
                    int tUserID = (int)_operationRequest.Parameters[(byte)CustomParameterKey.UserID];
                    int tScore = 0;
                    if (customerScoresDic.ContainsKey(tUserID))
                    {
                        tScore = customerScoresDic[tUserID];
                    }


                    tOperationResponse.ReturnCode = 0;  //0表示返回成功
                    tOperationResponse.DebugMessage = "获得积分成功！";

                    tOperationResponse.OperationCode = (byte)CustomOperationCode.GetMyScore; //告诉客户端做出了哪种类型的响应
                    Dictionary<byte, object> tParaDic = new Dictionary<byte, object>();
                    tParaDic[(byte)CustomParameterKey.UserID] = tScore;
                    tOperationResponse.Parameters = tParaDic;
                    break;
            }
            return tOperationResponse;
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            throw new NotImplementedException();
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

public enum CustomOperationCode : byte      //自定义的请求指令，客户端最好保持与其一致
{
    GetMyScore = 1,       //获取个人积分

}

public enum CustomParameterKey : byte
{
    UserID = 1,               //用户ID
}
