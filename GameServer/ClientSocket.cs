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
        //所属角色
        private Role m_Role;

        //客户端Socket
        private Socket m_Socket;

        //接收数据的线程
        private Thread m_ReveiveThread;

        //接收数据包的字节数组缓冲区
        private byte[] m_ReceiveBuffer = new byte[10240];

        //接收数据包的缓冲数据流
        private MMO_MemoryStream m_ReceiveMs = new MMO_MemoryStream();

        public ClientSocket(Socket socket,Role role)
        {
            m_Socket = socket;
            m_Role = role;
            m_Role.Client_Socket = this;

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
            try
            {
                int len = m_Socket.EndReceive(ar);
                if (len > 0)
                {
                    //已经接收到数据
                    //把接收到的数据 写入缓冲数据流的尾部
                    m_ReceiveMs.Position = m_ReceiveMs.Length;

                    //把指定长度的字节写入数据流
                    m_ReceiveMs.Write(m_ReceiveBuffer, 0, len);

                    //byte[] buffer = m_ReceiveMs.ToArray();

                    //如果缓存数据流的长度 大于 2 说明至少有个不完整的包过来
                    //我们客户端封装数据包 用的ushort 长度就是2
                    if(m_ReceiveMs.Length > 2)
                    {
                        //循环拆分数据包
                        while (true)
                        {
                            //把数据流指针位置放在0处
                            m_ReceiveMs.Position = 0;

                            //当前包体长度
                            int currMsgLen = m_ReceiveMs.ReadUShort();

                            //当前总包的长度
                            int currFullMsgLen = 2 + currMsgLen;

                            //如果数据流长度大于或等于总包长度 说明至少收到一个完整包
                            if(m_ReceiveMs.Length >= currMsgLen)
                            {
                                //收到完整包
                                byte[] buffer = new byte[currMsgLen];

                                //把数据流指针放在包体位置
                                m_ReceiveMs.Position = 2;

                                //把包体读取到byte数组中
                                m_ReceiveMs.Read(buffer, 0, currMsgLen);

                                //这里的buffer就是我们拆分的数据包
                                using (MMO_MemoryStream ms2 = new MMO_MemoryStream(buffer))
                                {
                                    string msg = ms2.ReadUTF8String();
                                    Console.WriteLine("接收的消息是 "+msg);
                                }

                                //处理剩余字节长度
                                int remainLen = (int)(m_ReceiveMs.Length - currFullMsgLen);
                                if(remainLen > 0)
                                {
                                    //把指针放在第一个包的尾部
                                    m_ReceiveMs.Position = currFullMsgLen;

                                    //定义剩余字节数组
                                    byte[] remainBuffer = new byte[remainLen];

                                    //将数据流读取到剩余字节数组当中
                                    m_ReceiveMs.Read(remainBuffer, 0, remainLen);

                                    //清空数据流
                                    m_ReceiveMs.Position = 0;
                                    m_ReceiveMs.SetLength(0);

                                    //将剩余字节数组重新写入数据流
                                    m_ReceiveMs.Write(remainBuffer,0,remainBuffer.Length);

                                    remainBuffer = null;
                                }
                                else
                                {
                                    //没有剩余字节
                                    //清空数据流
                                    m_ReceiveMs.Position = 0;
                                    m_ReceiveMs.SetLength(0);

                                    break;
                                }
                            }
                            else
                            {
                                //还没有完整包
                                break;
                            }
                        }
                    }

                    //进行下一次接收数据包
                    m_Socket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallBack, m_Socket);
                }
                else
                {
                    //说明客户端断开连接了
                    Console.WriteLine("客户端{0}断开连接了", m_Socket.RemoteEndPoint.ToString());

                    //将角色移除
                    RoleMgr.Instance.AllRole.Remove(m_Role);
                }
            }
            catch
            {
                //说明客户端断开连接了
                Console.WriteLine("客户端{0}断开连接了", m_Socket.RemoteEndPoint.ToString());

                //将角色移除
                RoleMgr.Instance.AllRole.Remove(m_Role);
            }

            
        }
    }
}
