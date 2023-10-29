using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    public enum LinkLayerProtocol
    {
        Other, Ethernet
    }

    public enum NetworkLayerProtocol
    {
        Other, IP, ARP, IPv6
    }

    // 注：这里理解ICMP、IGMP等协议并不属于传输层, 只是网络层中较IP层较高的层次
    // 但为了实现方便, 且这些协议确实传递一部分信息，
    // 并且不会放入真正属于传输层TCP、UDP等协议的payload
    // 故分类至此。
    public enum TransmissionLayerProtocol
    {
        Other, TCP, UDP, ICMP, IGMP, IPv6OverIPv4, HOPOPT, ICMPv6
    }

    public enum ApplicationLayerProtocol
    {
        OtherApplication, HTTP, DNS, TLS
    }

    public class ProtocolVariable
    {
        public static string GetTlsHandshakeType(byte type)
        {
            switch (type)
            {
                case 1: return "Client Hello";
                case 2: return "Server Hello";
                case 11: return "Certificate";
                case 16: return "Client Key Exchange";
                case 4: return "New Session Ticket";
                case 12: return "Server Key Exchange";
                case 14: return "Server Hello Done";
                default: return "";
            }
        }

        public static string GetTlsContentType(byte type)
        {
            switch (type)
            {
                case 20: return "Change Cipher Spec";
                case 21: return "Alert";
                case 22: return "Handshake";
                case 23: return "Application Data";
                default: return "";
            }
        }

        public static string GetTlsVersion(ushort version)
        {
            switch (version)
            {
                case 0x0300: return "SSL v3.0";
                case 0x0301: return "TLS v1.0";
                case 0x0302: return "TLS v1.1";
                case 0x0303: return "TLS v1.2";
                case 0x0304: return "TLS v1.3";
                default: return "Unkonwn";
            }
        }

        public static string GetARPOperation(ushort opcode)
        {
            switch(opcode)
            {
                case 1: return "request";
                case 2: return "reply";
                default: return "";
            }
        }

        public static string GetICMPType(byte type)
        {
            switch (type)
            {
                case 0:
                    return "Echo Reply";
                case 3:
                    return "Destination Unreachable";
                case 5:
                    return "Redirect";
                case 8:
                    return "Echo Request";
                case 11:
                    return "Time Exceeded";
                case 12:
                    return "Parameter Problem";
                case 13:
                    return "Timestamp Request";
                case 14:
                    return "Timestamp Reply";
                case 17:
                    return "Addres Mask Request";
                case 18:
                    return "Address Mask Reply";
                default:
                    return "";
            }
        }

        public static string GetICMPCode(byte type, byte code)
        {
            switch(type)
            {
                case 3:
                    if (code == 0) return "Network unreachable";
                    else if (code == 1) return "Host unreachable";
                    else if (code == 2) return "Protocol unreachable";
                    else if (code == 3) return "Port unreachable";
                    else return "";
                case 5:
                    if (code == 0) return "Redirect Datagram for the Network";
                    else if (code == 1) return "Redirect Datagram for the Host";
                    else if (code == 2) return "Redirect Datagram for the Type of Service and Network";
                    else if (code == 3) return "Redirect Datagram for the Type of Service and Host";
                    else return "";
                case 11:
                    if (code == 0) return "Time to Live exceeded in Transit";
                    else if (code == 1) return "Fragment Reassembly Time Exceeded";
                    else return "";
                case 12:
                    if (code == 0) return "Pointer indicates the error";
                    else if (code == 1) return "Missing a Required Option";
                    else if (code == 2) return "Bad Length";
                    else return "";
                default:
                    return "";
            }
        }

        public static string GetICMPv6Type(byte type)
        {
            switch (type)
            {
                case 1:
                    return "Destination Unreachable";
                case 2:
                    return "Packet Too Big";
                case 3:
                    return "Time Exceeded";
                case 4:
                    return "Time Exceeded";
                case 128:
                    return "Echo Request";
                case 129:
                    return "Echo Reply";
                case 133:
                    return "Router Solicitation";
                case 134:
                    return "Router Advertisement";
                case 135:
                    return "Neighbor Solicitation";
                case 136:
                    return "Neighbor Advertisement";
                default:
                    return "";
            }
        }
    }
}
