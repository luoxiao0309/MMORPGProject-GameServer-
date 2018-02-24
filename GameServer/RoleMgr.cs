using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class RoleMgr
    {
        private RoleMgr() { }

        #region 单例
        private static object lock_object = new object();

        private static RoleMgr instance;

        public static RoleMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lock_object)
                    {
                        if (instance == null)
                        {
                            instance = new RoleMgr();
                            
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        private List<Role> m_AllRole = new List<Role>();

        public List<Role> AllRole
        {
            get
            {
                return m_AllRole;
            }
        }

    }
}
