using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    // 粗粒度解析
    public class RoughParser
    {
        public unsafe static void begin(PacketWrapper pkt)
        {
            fixed (byte* ptr = pkt.Data)
            {
                LinkLayer(pkt, ptr);
            }
        }

        public unsafe static void LinkLayer(PacketWrapper pkt, byte* ptr)
        {
            EthernetHeader eth = *(EthernetHeader*) ptr;
            switch(Tools.NetworkToHost(eth.type))
            {
                case 0x0800:
                    pkt.NetworkLayerProtocol = NetworkLayerProtocol.IP;  
                    break;
                case 0x0806:
                    pkt.NetworkLayerProtocol = NetworkLayerProtocol.ARP;
                    break;
                case 0x86DD:
                    pkt.NetworkLayerProtocol = NetworkLayerProtocol.IPv6;
                    break;
                default:
                    pkt.NetworkLayerProtocol = NetworkLayerProtocol.Other;
                    break;
            }
            pkt.TopProtocol = pkt.NetworkLayerProtocol.ToString();
            pkt.LinkLayerSource = Tools.GetMacHexFromBytesArray(eth.sourceAddress);
            pkt.LinkLayerDestination = Tools.GetMacHexFromBytesArray(eth.destinationAddress);
            pkt.TopDestination = pkt.LinkLayerDestination;
            pkt.TopSource = pkt.LinkLayerSource;
            pkt.TopInfo = "Ethernet";
            NetworkLayer(pkt, ptr+sizeof(EthernetHeader));
        }

        public unsafe static void NetworkLayer(PacketWrapper pkt, byte* ptr)
        {
            switch (pkt.NetworkLayerProtocol)
            {
                case NetworkLayerProtocol.IP:
                    IPHeader ip = *(IPHeader*)ptr;
                    switch (ip.protocol)
                    {
                        // 此处理解并非属于传输层协议
                        // 只是方便管理, ICMP、IGMP等本身传递一定信息，且后没有其他传输层payload
                        // 具体见Protocol.cs文件中注释 
                        case 1:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.ICMP;
                            break;
                        case 2:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.IGMP;
                            break;
                        case 6:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.TCP;
                            break;
                        case 17:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.UDP;
                            break;
                        case 41:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.IPv6OverIPv4;
                            break;
                        default:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.Other;
                            break;
                    }
                    pkt.NetworkLayerSource = Tools.GetIPFromBytes(ip.sourceAddress);
                    pkt.NetworkLayerDestination = Tools.GetIPFromBytes(ip.destinationAddress);
                    pkt.TopSource = pkt.NetworkLayerSource;
                    pkt.TopDestination = pkt.NetworkLayerDestination;
                    pkt.TopProtocol = pkt.TransmissionLayerProtocol.ToString();
                    pkt.TopInfo = "IP";
                    pkt.NetworkLayerHeaderLength = (4 * (ip.versionAndHeadLength & 0x0F));
                    pkt.NetworkLayerPayloadLength = Tools.NetworkToHost(ip.totalLength);
                    // 取出HeadLength, 并乘以4跳过IP头。
                    TrasmissionLayer(pkt, ptr + pkt.NetworkLayerHeaderLength);
                    break;
                case NetworkLayerProtocol.ARP:
                    ARPHeader arp = *(ARPHeader*)ptr;
                    if (Tools.NetworkToHost(arp.operationCode) == 1)
                        pkt.TopInfo = String.Format("Who is {0}? Tell {1}",
                            Tools.GetIPFromBytes(arp.destinationIPAddress),
                            Tools.GetIPFromBytes(arp.sourceIPAddress));
                    else
                        pkt.TopInfo = String.Format("{0} is at {1}",
                            Tools.GetIPFromBytes(arp.sourceIPAddress),
                            Tools.GetMacHexFromBytesArray(arp.sourceEthernetAddress));
                    pkt.TopProtocol = "ARP";
                    break;
                case NetworkLayerProtocol.IPv6:
                    IPv6Header v6 = *(IPv6Header*)ptr;
                    // Console.WriteLine(v6.nextHeader);
                    bool extendHeader = true;
                    switch(v6.nextHeader)
                    {
                        case 0:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.HOPOPT;
                            break;
                        case 6:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.TCP;
                            extendHeader = false;
                            break;
                        case 17:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.UDP;
                            extendHeader = false;
                            break;
                        case 58:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.ICMPv6;
                            // 此处理解并非属于传输层协议
                            // 为方便管理, ICMPv6本身传递一定信息，且后没有其他传输层payload
                            // 具体见Protocol.cs文件中注释
                            extendHeader = false;  
                            break;
                        default:
                            pkt.TransmissionLayerProtocol = TransmissionLayerProtocol.Other;
                            break;
                    }
                    pkt.NetworkLayerSource = Tools.GetIPv6FromBytesArray(v6.sourceAddress);
                    pkt.NetworkLayerDestination = Tools.GetIPv6FromBytesArray(v6.destinationAddress);
                    pkt.TopSource = pkt.NetworkLayerSource;
                    pkt.TopDestination = pkt.NetworkLayerDestination;
                    pkt.TopProtocol = pkt.TransmissionLayerProtocol.ToString();
                    pkt.TopInfo = "IPv6";
                    pkt.NetworkLayerPayloadLength = Tools.NetworkToHost(v6.payloadLength);
                    if (!extendHeader)
                    {
                        pkt.NetworkLayerHeaderLength = sizeof(IPv6Header);
                        TrasmissionLayer(pkt, ptr + pkt.NetworkLayerHeaderLength);
                    }
                    // extendHeader解析太复杂了,没时间了...
                    break;
            }
        }

        public unsafe static void TrasmissionLayer(PacketWrapper pkt, byte* ptr) 
        {
            switch(pkt.TransmissionLayerProtocol)
            {
                case TransmissionLayerProtocol.TCP:
                    TCPHeader tcp = *(TCPHeader*)ptr;
                    int tcpHeaderLength = (tcp.headerLength >> 4) * 4;
                    pkt.TransmissionLayerPayloadLength = pkt.NetworkLayerPayloadLength - pkt.NetworkLayerHeaderLength - tcpHeaderLength;
                    if(pkt.TransmissionLayerPayloadLength < 0)
                    {
                        pkt.TransmissionLayerPayloadLength = 0;
                    }
                    // Console.WriteLine("{0} {1} {2}", pkt.NetworkLayerPayloadLength, pkt.NetworkLayerHeaderLength, tcpHeaderLength);
                    pkt.TransmissionSourcePort = Tools.NetworkToHost(tcp.sourcePort);
                    pkt.TransmissionDestinationPort = Tools.NetworkToHost(tcp.destinationPort);
                    string flag_str = "";
                    if((tcp.flags & 0x20) >> 5 == 1)
                    {
                        flag_str += "URG, ";
                    }
                    if((tcp.flags & 0x10) >> 4 == 1)
                    {
                        flag_str += "ACK, ";
                    }
                    if ((tcp.flags & 0x08) >> 3 == 1)
                    {
                        flag_str += "PSH, ";
                    }
                    if ((tcp.flags & 0x04) >> 2 == 1)
                    {
                        flag_str += "RST, ";
                    }
                    if ((tcp.flags & 0x02) >> 1 == 1)
                    {
                        flag_str += "SYN, ";
                    }
                    if ((tcp.flags & 0x01) == 1)
                    {
                        flag_str += "FIN, ";
                    }
                    if (flag_str.Length > 0)
                    {
                        flag_str = flag_str.Substring(0, flag_str.Length - 2);
                    }

                    uint seqNumber = Tools.NetworkToHost(tcp.sequence);
                    uint ackNumber = Tools.NetworkToHost(tcp.acknowledgement);
                    pkt.TopInfo = String.Format("{0}->{1} [{2}] SEQ={3} ACK={4} LEN={5}", 
                        pkt.TransmissionSourcePort, pkt.TransmissionDestinationPort, flag_str, 
                        seqNumber, ackNumber, pkt.TransmissionLayerPayloadLength);
                    ApplicationLayer(pkt, ptr + tcpHeaderLength);
                    break;
                case TransmissionLayerProtocol.UDP:
                    UDPHeader udp = *(UDPHeader*)ptr;
                    pkt.TransmissionLayerPayloadLength = Tools.NetworkToHost(udp.dataLength) - 8;
                    pkt.TransmissionDestinationPort = Tools.NetworkToHost(udp.destinationPort);
                    pkt.TransmissionSourcePort = Tools.NetworkToHost(udp.sourcePort);
                    pkt.TopInfo = String.Format("{0}->{1} LEN={2}", 
                        pkt.TransmissionSourcePort, pkt.TransmissionDestinationPort, pkt.TransmissionLayerPayloadLength);
                    ApplicationLayer(pkt, ptr + sizeof(UDPHeader)); 
                    break;
                case TransmissionLayerProtocol.ICMP:
                    ICMPHeader icmp = *(ICMPHeader*)ptr;
                    pkt.TopInfo = ProtocolVariable.GetICMPType(icmp.type);
                    break;
                case TransmissionLayerProtocol.ICMPv6:
                    ICMPv6Header icmpv6 = *(ICMPv6Header*)ptr;
                    pkt.TopInfo = string.Format("{0} ({1})", ProtocolVariable.GetICMPv6Type(icmpv6.type), icmpv6.type);
                    break;
                case TransmissionLayerProtocol.IGMP:
                    pkt.TopInfo = "IGAP/IGMP/RGMP";
                    break;
                case TransmissionLayerProtocol.IPv6OverIPv4:
                    pkt.TopInfo = "IPv6 Over IPv4";
                    break;

            }
        }

        public unsafe static void ApplicationLayer(PacketWrapper pkt, byte* ptr)
        {
            switch (pkt.TransmissionLayerProtocol)
            {
                case TransmissionLayerProtocol.TCP:
                    // 解析TCP payload
                    // 启发式解析判断应用层协议类型
                    if(HeuristicJudger.IsTlsProtocol(ptr))
                    {
                        // 是TLS
                        pkt.ApplicationLayerProtocol = ApplicationLayerProtocol.TLS;
                        pkt.TopProtocol = pkt.ApplicationLayerProtocol.ToString();

                        byte contentType = *ptr;
                        ptr++;
                        ushort* pointer = (ushort*)ptr;
                        ushort version = Tools.NetworkToHost(*pointer);
                        
                        ptr += sizeof(ushort);
                        pointer++;
                        ushort length = Tools.NetworkToHost(*pointer);
                        ptr += sizeof(ushort);

                        pkt.TopInfo = ProtocolVariable.GetTlsVersion(version) + " ";
                        pkt.TopInfo += ProtocolVariable.GetTlsContentType(contentType) + " ";
                        switch (contentType)
                        {
                            case 22:
                                // Handshake, 读取handshake状态
                                byte handshakeType = *ptr;
                                ptr += 4;  // 跳过length
                                pkt.TopInfo += ProtocolVariable.GetTlsHandshakeType(handshakeType) + " ";
                                break;
                        }
                    }
                    else if(HeuristicJudger.IsHttpProtocol(ptr))
                    {
                        // 是HTTP
                        pkt.ApplicationLayerProtocol = ApplicationLayerProtocol.HTTP;
                        pkt.TopProtocol = pkt.ApplicationLayerProtocol.ToString();
                    }
                    // 时间原因, 剩下协议就用端口暂时判断
                    else if(pkt.TransmissionDestinationPort == 53 || pkt.TransmissionSourcePort == 53)
                    {
                        // DNS协议
                        pkt.ApplicationLayerProtocol = ApplicationLayerProtocol.DNS;
                        pkt.TopProtocol = pkt.ApplicationLayerProtocol.ToString();
                        pkt.TopInfo = "DNS";
                        DNSHeader dns = *(DNSHeader*)ptr;
                        ushort identification = Tools.NetworkToHost(dns.identification);
                        ushort type = Tools.NetworkToHost(dns.flags);
                        if((type & 0xf800) == 0x0000)
                        {
                            pkt.TopInfo += String.Format(" Standard query 0x{0:X4}", identification);
                        }
                        else if((type & 0xf800) == 0x8000)
                        {
                            pkt.TopInfo += String.Format(" Standard query response 0x{0:X4}", identification);
                        }
                        // 详细内容就放在细粒度解析再说吧
                    }
                    
                    break;
                case TransmissionLayerProtocol.UDP:
                    // 解析UDP payload
                    // 暂用端口号判断
                    if (pkt.TransmissionDestinationPort == 53 || pkt.TransmissionSourcePort == 53)
                    {
                        pkt.ApplicationLayerProtocol = ApplicationLayerProtocol.DNS;
                        pkt.TopProtocol = pkt.ApplicationLayerProtocol.ToString();
                        pkt.TopInfo = "DNS";
                        DNSHeader dns = *(DNSHeader*)ptr;
                        ushort identification = Tools.NetworkToHost(dns.identification);
                        ushort type = Tools.NetworkToHost(dns.flags);
                        if ((type & 0xf800) == 0x0000)
                        {
                            pkt.TopInfo += String.Format(" Standard query 0x{0:X4}", identification);
                        }
                        else if ((type & 0xf800) == 0x8000)
                        {
                            pkt.TopInfo += String.Format(" Standard query response 0x{0:X4}", identification);
                        }
                    }
                    break;
            }
        }
    }
}
