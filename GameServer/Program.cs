using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        private static string m_ServerIP = "127.0.0.1";
        private static int m_Port = 111;

        private static Socket m_ServerSocket;

        static void Main(string[] args)
        {
            //实例化Socket
            m_ServerSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            //向操作系统申请可用的IP和端口 用来通信
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(m_ServerIP),m_Port));

            //设置最多3000个排队连接请求
            m_ServerSocket.Listen(3000);

            Console.WriteLine("启动监听{0}",m_ServerSocket.LocalEndPoint.ToString());

            Thread mThread = new Thread(ListenClientCallBack);
            mThread.Start();

            Console.ReadLine();
        }

        /// <summary>
        /// 监听客户端连接的回调
        /// </summary>
        private static void ListenClientCallBack()
        {
            while (true)
            {
                //接收客户端请求
                Socket socket = m_ServerSocket.Accept();

                Console.WriteLine("客户端{0}已经连接", socket.RemoteEndPoint.ToString());

                //一个角色相当于一个客户端
                Role role = new Role();
                ClientSocket clientSocket = new ClientSocket(socket,role);

                //把角色添加到角色管理
                RoleMgr.Instance.AllRole.Add(role);
            }
        }
    }
}
