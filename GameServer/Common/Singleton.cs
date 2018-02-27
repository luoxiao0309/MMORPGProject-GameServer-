// ========================================================
// 描 述：
// 作 者：牛水鱼 
// 创建时间：2018-02-06 15:37:51
// 版 本：v 1.0
// ========================================================
using System;

/// <summary>
/// 单例的基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T>:IDisposable where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    /// <summary>
    /// 可以被子类继承的资源卸载的方法
    /// </summary>
    public virtual void Dispose()
    {
    }
}
