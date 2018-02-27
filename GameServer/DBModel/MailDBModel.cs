using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DBModel
{
    class MailDBModel:Singleton<MailDBModel>
    {
        /// <summary>
        /// 初始化（监听消息）
        /// </summary>
        public void Init()
        {
            EventDispatcher.Instance.AddEventListener(ProtoCodeDef.Test,OnRequestTest);
        }

        private void OnRequestTest(Role role, byte[] buffer)
        {
            Console.WriteLine("客户端请求测试协议 ");

            TestProto proto = new TestProto();
            proto.Id = 100;
            proto.Name = "Response";
            proto.Price = 1200;
            proto.Type = 2;

            role.Client_Socket.SendMsg(proto.ToArray());
        }
    }
}
