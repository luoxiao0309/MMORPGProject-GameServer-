using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// 客户端连接对象 负责和客户端通信
    /// </summary>
    public class ClientSocket
    {
        //客户端Socket
        private Socket m_Socket;

        //接收数据的线程
        private Thread m_ReveiveThread;

        //接收数据包的字节数组缓冲区
        private byte[] m_ReceiveBuffer = new byte[10240];

        //接收数据包的缓冲数据流
        private MMO_MemoryStream m_ReceiveMs = new MMO_MemoryStream();

        public ClientSocket(Socket socket)
        {
            m_Socket = socket;

            //启动线程 进行数据接收
            m_ReveiveThread = new Thread(ReceiveMsg);
            m_ReveiveThread.Start();
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        private void ReceiveMsg()
        {
            //异步接收数据
            m_Socket.BeginReceive(m_ReceiveBuffer,0,m_ReceiveBuffer.Length,SocketFlags.None,ReceiveCallBack,m_Socket);
        }


        /// <summary>
        /// 接收数据回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            int len = m_Socket.EndReceive(ar);
            if(len > 0)
            {
                //已经接收到数据

                //把接收到的数据 写入缓冲数据流的尾部
                m_ReceiveMs.Position = m_ReceiveMs.Length;

                //把指定长度的字节写入数据流
                m_ReceiveMs.Write(m_ReceiveBuffer,0,len);

                byte[] buffer = m_ReceiveMs.ToArray();
            }
            else
            {
                //没有接收到数据

            }
        }
    }
}
