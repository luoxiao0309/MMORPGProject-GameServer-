// ========================================================
// 描 述：
// 作 者：牛水鱼 
// 创建时间：2018-02-26 17:46:12
// 版 本：v 1.0
// ========================================================
using GameServer;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class EventDispatcher : Singleton<EventDispatcher>
    {

        //委托原型
        public delegate void OnActionHandler(Role role, byte[] buffer);

        //委托字典
        private Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="protoCode"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort protoCode, OnActionHandler handler)
        {
            if (dic.ContainsKey(protoCode))
            {
                dic[protoCode].Add(handler);
            }
            else
            {
                List<OnActionHandler> lstHandler = new List<OnActionHandler>();
                lstHandler.Add(handler);
                dic[protoCode] = lstHandler;
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(ushort protoCode, OnActionHandler handler)
        {
            if (dic.ContainsKey(protoCode))
            {
                List<OnActionHandler> lstHandler = dic[protoCode];
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0)
                {
                    dic.Remove(protoCode);
                }
            }
        }

        /// <summary>
        /// 派发协议
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="param"></param>
        public void Dispatch(ushort protoCode, Role role, byte[] buffer)
        {
            if (dic.ContainsKey(protoCode))
            {
                List<OnActionHandler> lstHandler = dic[protoCode];
                if (lstHandler != null && lstHandler.Count > 0)
                {
                    for (int i = 0; i < lstHandler.Count; i++)
                    {
                        if (lstHandler[i] != null)
                        {
                            lstHandler[i](role, buffer);
                        }
                    }
                }
            }
        }
    }
}