using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    // 启发式的判断payload类型
    public class HeuristicJudger
    {
        // 判断是否是TLS流量
        public static unsafe bool IsTlsProtocol(byte* payloadPtr)
        {
            byte type = (byte)(*payloadPtr);
            // Console.WriteLine(String.Format("Type=>{0}", type));
            if (type >= 20 && type <= 23)
            {
                ushort* point = (ushort*)(payloadPtr + 1);
                int version = Tools.NetworkToHost(*point);
                // Console.WriteLine(String.Format("Type=>{0}, Version=>{1}", type, version));
                if (version >= 0x0301 && version <= 0x0304)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        // 判断是否是HTTP流量
        public static unsafe bool IsHttpProtocol(byte* payloadPtr)
        {
            byte[] startArray = Tools.CopyFromPointer(payloadPtr, 0, 8);
            Tools.NetworkToHost(startArray);
            string res = Encoding.UTF8.GetString(startArray);
            // 判断是否为HTTP/1.1，为http response
            // 说明：进一步可以设计启发式算法为“HTTP/1.1[20]状态码[20]状态描述[0x0d][0x0a]”来进一步提高置信度
            if (res.StartsWith("HTTP/1.1"))
            {
                return true;
            }

            // 如果非，判断是否为四大HTTP操作  GET/POST/PUT/DELETE
            // 说明：进一步可以设计启发式算法为“HTTP操作[0x20]URI[0x20]HTTP/1.1[0x0d][0x0a]”
            if(res.StartsWith("GET") || res.StartsWith("POST") || res.StartsWith("PUT") || res.StartsWith("DELETE"))
            {
                return true;
            }

            return false;
        }
    }
}
