using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Lite;
using System.Threading;

namespace MyServer
{
    public class MyApplication : LiteApplication
    {
        private static Dictionary<int, MyPeer> clientPeers = new Dictionary<int, MyPeer>();
        Timer tServerTimer = new Timer(broadCastServerTime, null, 2000, 1000);
        protected override PeerBase CreatePeer(InitRequest _initRequest)//创建与客户端连接的Peer
        {
            MyPeer tMyPeer = new MyPeer(_initRequest);
            clientPeers.Add(tMyPeer.ConnectionId, tMyPeer);
            return tMyPeer;
        }

        static void broadCastServerTime(object _obj)
        {

        }
        protected override void Setup()
        {
            //throw new NotImplementedException();

        }

        protected override void TearDown()
        {
            //throw new NotImplementedException();

        }
    }
}
