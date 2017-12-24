using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Lite;

namespace MyServer
{
    public class MyApplication : LiteApplication
    {
        protected override PeerBase CreatePeer(InitRequest _initRequest)//创建与客户端连接的Peer
        {
            //throw new NotImplementedException();
            //return null;
            MyPeer tMyPeer = new MyPeer(_initRequest);
            return tMyPeer;
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
