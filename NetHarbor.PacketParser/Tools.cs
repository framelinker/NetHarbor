using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    public class Tools
    {
        private static char GetHexDigit(int value)
        {
            return (char)(value < 10 ? '0' + value : 'a' + value - 10);
        }

        public unsafe static string GetMacHexFromBytesArray(byte* ptr)
        {
            int size = 6 * sizeof(byte);  // MAC地址6B=48b
            string res = "";
            for(int i = 0;i < size;i++)
            {
                int x = ptr[i];
                res += GetHexDigit(x / 16);
                res += GetHexDigit(x % 16);
                // res += hexchar[x / 16];
                // res += hexchar[x % 16];
                res += ':';
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        // 网络使用大端序, x86使用小端序, 即为网络转主机, 会进行判断
        public static byte[] NetworkToHost(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
                return value.Reverse().ToArray();
            else
                return value;
        }

        public unsafe static void NetworkToHost(byte* ptr, int len)
        {
            byte t = 0;
            if (BitConverter.IsLittleEndian)
                for(int i = 0;i < len / 2; ++i)
                {
                    t = ptr[len - i - 1];
                    ptr[len - i - 1] = ptr[i];
                    ptr[i] = t;
                }
            else
                return;
        }

        public static ushort NetworkToHost(ushort value)
        {
            if (BitConverter.IsLittleEndian)
                return (ushort)((value >> 8) | (value << 8));
            else
                return value;
        }

        public static uint NetworkToHost(uint value)
        {
            if (BitConverter.IsLittleEndian)
                return ((value & 0x000000FFU) << 24) |
               ((value & 0x0000FF00U) << 8) |
               ((value & 0x00FF0000U) >> 8) |
               ((value & 0xFF000000U) >> 24);
            else
                return value;
        }

        // 会进行大小端序转换
        public static string GetIPFromBytes(uint ipAddress)
        {
            // return new IPAddress(value).ToString();
            ipAddress = NetworkToHost(ipAddress);
            return $"{(ipAddress >> 24) & 0xFF}.{(ipAddress >> 16) & 0xFF}.{(ipAddress >> 8) & 0xFF}.{ipAddress & 0xFF}";
        }

        public unsafe static string GetIPv6FromBytesArray(byte* ptr)
        {
            int size = 16 * sizeof(byte);
            var str = new StringBuilder();
            for (var i = 0; i < size; i += 2)
            {
                var segment = (ushort)ptr[i] << 8 | ptr[i + 1];
                str.AppendFormat("{0:X}", segment);
                if (i + 2 != size)
                {
                    str.Append(':');
                }
            }

            return str.ToString();
        }

        public unsafe static byte[] CopyFromPointer(byte* pointer, int startFrom, int length)
        {
            byte[] result = new byte[length];
            for(int i = 0;i < length;i++)
            {
                result[i] = pointer[startFrom + i];
            }
            return result;
        }

        // 三字节大端字节指针转小端int64
        public unsafe static int ThreeBytesNetworkPtrToInt(byte* pointer, int startFrom)
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 3; i++)
            {
                result[2 - i] = pointer[startFrom + i];
            }
            return BitConverter.ToInt32(result, 0);
        }

        public unsafe static ushort ReadUshortFromPtr(byte* pointer)
        {
            ushort* tempPtr = (ushort*)pointer;
            ushort res = (*tempPtr);
            return res;
        }

        public unsafe static ushort ReadUshortFromPtrAndToHostEndian(byte* pointer)
        {
            ushort* tempPtr = (ushort*)pointer;
            ushort res = Tools.NetworkToHost(*tempPtr);
            return res;
        }

        public unsafe static uint ReadUintFromPtrAndToHostEndian(byte* pointer)
        {
            uint* tempPtr = (uint*)pointer;
            uint res = Tools.NetworkToHost(*tempPtr);
            return res;
        }

        public static string ConvertBytesArrayToHex(byte[] bytesArray)
        {
            StringBuilder hexBuilder = new StringBuilder();
            foreach(byte b in bytesArray)
            {
                hexBuilder.Append(b.ToString("X2"));
            }
            return hexBuilder.ToString();
        }

        /*public static int NetworkToHost(int number)
        {
            if (BitConverter.IsLittleEndian)
                return IPAddress.NetworkToHostOrder(number);
            else
                return number;
        }*/
    }
}
