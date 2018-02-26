// ========================================================
// 描 述：
// 作 者：牛水鱼 
// 创建时间：2018-02-26 09:26:46
// 版 本：v 1.0
// ========================================================
using System.Collections;

public struct TestProto :IProto
{

    public ushort ProtoCode{ get { return 10000; } }

    public int Id;
    public string Name;
    public int Type;
    public float Price;

    public byte[] ToArray()
    {
        using(MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(Id);
            ms.WriteUTF8String(Name);
            ms.WriteInt(Type);
            ms.WriteIFloat(Price);

            return ms.ToArray();
        }
    }

    public static TestProto GetProto(byte[] buffer)
    {
        TestProto proto = new TestProto();

        using(MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.Id = ms.ReadInt();
            proto.Name = ms.ReadUTF8String();
            proto.Type = ms.ReadInt();
            proto.Price = ms.ReadFloat();
        }

        return proto;
    }

}
