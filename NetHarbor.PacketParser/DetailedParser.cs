using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetHarbor.PacketParser
{
    // 细粒度解析
    public class DetailedParser
    {
        public unsafe static void begin(PacketWrapper pkt, TreeNodeCollection node)
        {
            fixed (byte* ptr = pkt.Data)
            {
                TreeNode physicalLayer = node.Add(String.Format("Frame {0}, {1} 字节", pkt.Count, pkt.Length));
                physicalLayer.Nodes.Add(String.Format("相对到达时间: {0}", pkt.RelativeTimeval));
                LinkLayer(pkt, ptr, node);
            }
        }

        public unsafe static void LinkLayer(PacketWrapper pkt, byte* ptr, TreeNodeCollection node)
        {
            EthernetHeader eth = *(EthernetHeader*)ptr;
            TreeNode linkLayer = node.Add(String.Format("Ethernet II, 源: {0}, 目的: {1}", 
                pkt.LinkLayerSource, pkt.LinkLayerDestination));
            linkLayer.Nodes.Add(String.Format("目的地址: {0}", pkt.LinkLayerDestination));
            linkLayer.Nodes.Add(String.Format("源地址: {0}", pkt.LinkLayerSource));
            linkLayer.Nodes.Add(String.Format("上层协议: {0}", pkt.NetworkLayerProtocol));

            NetworkLayer(pkt, ptr + sizeof(EthernetHeader), node);
        }

        public unsafe static void NetworkLayer(PacketWrapper pkt, byte* ptr, TreeNodeCollection node)
        {
            TreeNode networkLayer;
            switch (pkt.NetworkLayerProtocol)
            {
                case NetworkLayerProtocol.IP:
                    IPHeader ip = *(IPHeader*)ptr;
                    networkLayer = node.Add(String.Format("Internet Protocol Version 4, 源: {0}, 目的: {1}",
                        pkt.NetworkLayerSource, pkt.NetworkLayerDestination));
                    networkLayer.Nodes.Add(string.Format("版本: 4"));
                    networkLayer.Nodes.Add(string.Format("首部长度: {0} bytes (0x{1:X2})", pkt.NetworkLayerHeaderLength, (ip.versionAndHeadLength & 0x0F)));
                    networkLayer.Nodes.Add(string.Format("服务类型: 0x{0:X2}", ip.typeOfService));
                    networkLayer.Nodes.Add(string.Format("总长度: {0}", Tools.NetworkToHost(ip.totalLength)));
                    networkLayer.Nodes.Add(string.Format("标识字段: 0x{0:X4} ({0})", Tools.NetworkToHost(ip.identification)));
                    int df = (Tools.NetworkToHost(ip.flagAndOffset) & 0x4000) >> 14;
                    int mf = (Tools.NetworkToHost(ip.flagAndOffset) & 0x2000) >> 13;
                    TreeNode ipFlags;
                    if (df == 1)
                    {
                        ipFlags = networkLayer.Nodes.Add(string.Format("标志字段 (Flags): 不要分片 Don't fragment"));
                    }
                    else if(mf == 1)
                    {
                        ipFlags = networkLayer.Nodes.Add(string.Format("标志字段 (Flags): 更多分片 More fragments"));
                    }
                    else
                    {
                        ipFlags = networkLayer.Nodes.Add(string.Format("标志字段 (Flags)"));
                    }
                    ipFlags.Nodes.Add(string.Format("{0}.. = Reserved bit", 
                        (Tools.NetworkToHost(ip.flagAndOffset) & 0x8000) >> 15 ));
                    ipFlags.Nodes.Add(string.Format(".{0}. = Don't fragment", df));
                    ipFlags.Nodes.Add(string.Format("..{0} = More fragments", mf));
                    networkLayer.Nodes.Add(string.Format("片偏移: {0}", 
                        (Tools.NetworkToHost(ip.flagAndOffset) & 0x1fff)));
                    networkLayer.Nodes.Add(string.Format("生存时间 (Time To Live): {0}", ip.ttl));
                    networkLayer.Nodes.Add(string.Format("承载协议: {0}", pkt.TransmissionLayerProtocol));
                    networkLayer.Nodes.Add(string.Format("首部校验和: 0x{0:X4}", Tools.NetworkToHost(ip.checksum)));
                    networkLayer.Nodes.Add(string.Format("源地址: {0}", pkt.NetworkLayerSource));
                    networkLayer.Nodes.Add(string.Format("目的地址: {0}", pkt.NetworkLayerDestination));
                    TrasmissionLayer(pkt, ptr + pkt.NetworkLayerHeaderLength, node);
                    break;
                case NetworkLayerProtocol.ARP:
                    ARPHeader arp = *(ARPHeader*)ptr;
                    networkLayer = node.Add(String.Format("{0}", "Address Resolution Protocol"));
                    networkLayer.Nodes.Add(String.Format("硬件类型: {0}", Tools.NetworkToHost(arp.hardwareType)));
                    networkLayer.Nodes.Add(String.Format("协议类型: {0}", Tools.NetworkToHost(arp.protocolType)));
                    networkLayer.Nodes.Add(String.Format("硬件地址大小: {0}", arp.macLength));
                    networkLayer.Nodes.Add(String.Format("协议地址大小: {0}", arp.ipLength));
                    networkLayer.Nodes.Add(String.Format("操作码: {0}({1})", 
                        ProtocolVariable.GetARPOperation(Tools.NetworkToHost(arp.operationCode)),
                        Tools.NetworkToHost(arp.operationCode)));
                    networkLayer.Nodes.Add(String.Format("源MAC地址: {0}", Tools.GetMacHexFromBytesArray(arp.sourceEthernetAddress)));
                    networkLayer.Nodes.Add(String.Format("源IP地址: {0}", Tools.GetIPFromBytes(arp.sourceIPAddress)));
                    networkLayer.Nodes.Add(String.Format("目标MAC地址: {0}", Tools.GetMacHexFromBytesArray(arp.destinationEthernetAddress)));
                    networkLayer.Nodes.Add(String.Format("目标IP地址: {0}", Tools.GetIPFromBytes(arp.destinationIPAddress)));
                    // 没有其他层的协议了
                    break;
                case NetworkLayerProtocol.IPv6:
                    IPv6Header v6 = *(IPv6Header*)ptr;
                    networkLayer = node.Add(String.Format("Internet Protocol Version 6, 源: {0}, 目的: {1}",
                        pkt.NetworkLayerSource, pkt.NetworkLayerDestination));
                    networkLayer.Nodes.Add(string.Format("版本: 6"));
                    networkLayer.Nodes.Add(string.Format("流量类别(Traffic Class): 0x{0:X2}", 
                        (Tools.NetworkToHost(v6.versionAndTrafficClassAndFlowLabel) >> 20 ) & 0x0F));
                    networkLayer.Nodes.Add(string.Format("流标签(Flow Label): 0x{0:X5}",
                        Tools.NetworkToHost(v6.versionAndTrafficClassAndFlowLabel) & 0xFFFF));
                    networkLayer.Nodes.Add(string.Format("载荷长度(Payload Length): {0}", Tools.NetworkToHost(v6.payloadLength)));
                    networkLayer.Nodes.Add(string.Format("下个首部(Next Header): {0} ({1})", pkt.TransmissionLayerProtocol, v6.nextHeader));
                    networkLayer.Nodes.Add(string.Format("跳数限制(Hop Limit): {0}", v6.hopLimit));
                    networkLayer.Nodes.Add(string.Format("源地址: {0}", pkt.NetworkLayerSource));
                    networkLayer.Nodes.Add(string.Format("目的地址: {0}", pkt.NetworkLayerDestination));
                    // ExtendHeader做不完了, 继续向下解析...
                    TrasmissionLayer(pkt, ptr + pkt.NetworkLayerHeaderLength, node);
                    break;
                case NetworkLayerProtocol.Other:
                    node.Add("未知协议, 等待实现中...");
                    break;
            }
        }

        public unsafe static void TrasmissionLayer(PacketWrapper pkt, byte* ptr, TreeNodeCollection node)
        {
            TreeNode transmissionLayer;
            switch (pkt.TransmissionLayerProtocol)
            {
                case TransmissionLayerProtocol.TCP:
                    TCPHeader tcp = *(TCPHeader*)ptr;
                    int urg = (tcp.flags & 0x20) >> 5;
                    int ack = (tcp.flags & 0x10) >> 4;
                    int psh = (tcp.flags & 0x08) >> 3;
                    int rst = (tcp.flags & 0x04) >> 2;
                    int syn = (tcp.flags & 0x02) >> 1;
                    int fin = (tcp.flags & 0x01);
                    uint seqNumber = Tools.NetworkToHost(tcp.sequence);
                    uint ackNumber = Tools.NetworkToHost(tcp.acknowledgement);
                    transmissionLayer = node.Add(string.Format("Transmission Control Protocol, 源端口: {0}, 目的端口: {1}, Seq: {2}, Ack: {3}, 长度: {4}",
                        pkt.TransmissionSourcePort, pkt.TransmissionDestinationPort, seqNumber, ackNumber, pkt.TransmissionLayerPayloadLength));
                    transmissionLayer.Nodes.Add(string.Format("源端口: {0}", pkt.TransmissionSourcePort));
                    transmissionLayer.Nodes.Add(string.Format("目的端口: {0}", pkt.TransmissionDestinationPort));
                    transmissionLayer.Nodes.Add(string.Format("序列号(Sequence Number): {0}", seqNumber));
                    transmissionLayer.Nodes.Add(string.Format("确认号(Acknowledgment Number): {0}", ackNumber));
                    int tcpHeaderLength = (tcp.headerLength >> 4) * 4;
                    transmissionLayer.Nodes.Add(string.Format("首部长度: {0}", tcpHeaderLength));
                    string flag_str = "";
                    if (urg == 1)
                    {
                        flag_str += "URG, ";
                    }
                    if (ack == 1)
                    {
                        flag_str += "ACK, ";
                    }
                    if (psh == 1)
                    {
                        flag_str += "PSH, ";
                    }
                    if (rst == 1)
                    {
                        flag_str += "RST, ";
                    }
                    if (syn == 1)
                    {
                        flag_str += "SYN, ";
                    }
                    if (fin == 1)
                    {
                        flag_str += "FIN, ";
                    }
                    if (flag_str.Length > 0)
                    {
                        flag_str = flag_str.Substring(0, flag_str.Length - 2);
                    }
                    TreeNode tcpFlags = transmissionLayer.Nodes.Add(string.Format("Flags: 0x{0:X3} [{1}]", tcp.flags, flag_str));
                    tcpFlags.Nodes.Add(string.Format("{0}..... = Urgent: {1}", urg, urg == 1?"Set":"Not Set"));
                    tcpFlags.Nodes.Add(string.Format(".{0}.... = Acknowledgment: {1}", ack, ack == 1 ? "Set" : "Not Set"));
                    tcpFlags.Nodes.Add(string.Format("..{0}... = Push: {1}", psh, psh == 1 ? "Set" : "Not Set"));
                    tcpFlags.Nodes.Add(string.Format("...{0}.. = Reset: {1}", rst, rst == 1 ? "Set" : "Not Set"));
                    tcpFlags.Nodes.Add(string.Format("....{0}. = Syn: {1}", syn, syn == 1 ? "Set" : "Not Set"));
                    tcpFlags.Nodes.Add(string.Format(".....{0} = Fin: {1}", fin, fin == 1 ? "Set" : "Not Set"));
                    transmissionLayer.Nodes.Add(string.Format("窗口(Window): {0}", Tools.NetworkToHost(tcp.windowSize)));
                    transmissionLayer.Nodes.Add(string.Format("校验和: 0x{0:X4}", Tools.NetworkToHost(tcp.checksum)));
                    transmissionLayer.Nodes.Add(string.Format("紧急指针(Urgent Pointer): {0}", Tools.NetworkToHost(tcp.urgent)));
                    transmissionLayer.Nodes.Add(string.Format("TCP载荷: {0} bytes", pkt.TransmissionLayerPayloadLength));
                    ApplicationLayer(pkt, ptr + tcpHeaderLength, node);
                    break;
                case TransmissionLayerProtocol.UDP:
                    UDPHeader udp = *(UDPHeader*)ptr;
                    transmissionLayer = node.Add(string.Format("User Datagram Protocol, 源端口: {0}, 目的端口: {1}",
                        pkt.TransmissionSourcePort, pkt.TransmissionDestinationPort));
                    transmissionLayer.Nodes.Add(string.Format("源端口: {0}", pkt.TransmissionSourcePort));
                    transmissionLayer.Nodes.Add(string.Format("目的端口: {0}", pkt.TransmissionDestinationPort));
                    // 总长度，包含udp首部8B
                    transmissionLayer.Nodes.Add(string.Format("长度: {0}", Tools.NetworkToHost(udp.dataLength)));
                    transmissionLayer.Nodes.Add(string.Format("校验和: 0x{0:X4}", Tools.NetworkToHost(udp.checksum)));
                    transmissionLayer.Nodes.Add(string.Format("UDP载荷: {0} bytes", pkt.TransmissionLayerPayloadLength));
                    ApplicationLayer(pkt, ptr + sizeof(UDPHeader), node);
                    break;
                case TransmissionLayerProtocol.ICMP:
                    ICMPHeader icmp = *(ICMPHeader*)ptr;
                    transmissionLayer = node.Add("Internet Control Message Protocol");
                    transmissionLayer.Nodes.Add(string.Format("类型(Type): {0} ({1})",
                        icmp.type, ProtocolVariable.GetICMPType(icmp.type)));
                    transmissionLayer.Nodes.Add(string.Format("代码(Code): {0} ({1})",
                        icmp.code, ProtocolVariable.GetICMPCode(icmp.type, icmp.code)));
                    transmissionLayer.Nodes.Add(string.Format("校验和: 0x{0:X4}", icmp.checksum));
                    switch(icmp.type)
                    {
                        case 0:
                        case 8:
                            transmissionLayer.Nodes.Add(string.Format("Identifier: {0} (0x{0:X4})", 
                                Tools.NetworkToHost(icmp.identification)));
                            transmissionLayer.Nodes.Add(string.Format("Sequence: {0} (0x{0:X4})", 
                                Tools.NetworkToHost(icmp.sequence)));
                            break;
                    }
                    break;
                case TransmissionLayerProtocol.ICMPv6:
                    ICMPv6Header icmpv6 = *(ICMPv6Header*)ptr;
                    transmissionLayer = node.Add("Internet Control Message Protocol v6");
                    transmissionLayer.Nodes.Add(string.Format("类型(Type): {0} ({1})",
                        icmpv6.type, ProtocolVariable.GetICMPv6Type(icmpv6.type)));
                    transmissionLayer.Nodes.Add(string.Format("代码(Code): {0}", icmpv6.code));
                    transmissionLayer.Nodes.Add(string.Format("校验和: 0x{0:X4}", icmpv6.checksum));
                    break;
                case TransmissionLayerProtocol.IGMP:
                    node.Add("IAMP/IGMP/RGMP协议, 等待实现中...");
                    break;
                case TransmissionLayerProtocol.IPv6OverIPv4:
                    node.Add("IPv6 Over IPv4协议, 等待实现中...");
                    break;
                default:
                    node.Add("未知协议, 等待实现中...");
                    break;
            }
        }

        public unsafe static void ApplicationLayer(PacketWrapper pkt, byte* ptr, TreeNodeCollection node)
        {
            TreeNode applicationLayer;
            switch(pkt.ApplicationLayerProtocol)
            {
                case ApplicationLayerProtocol.HTTP:
                    applicationLayer = node.Add(string.Format("Hypertext Transfer Protocol"));
                    byte[] payloadBytes = Tools.CopyFromPointer(ptr, 0, pkt.TransmissionLayerPayloadLength);
                    Tools.NetworkToHost(payloadBytes);
                    string[] payloadArr = Encoding.UTF8.GetString(payloadBytes).Split('\n');
                    for(int i = 0;i < payloadArr.Length;i++)
                    {
                        string payload = payloadArr[i];
                        if (payload.Length == 1 && payload.StartsWith("\r"))
                        {
                            // 空行, 剩余内容为HTTP消息
                            break;
                        }
                        applicationLayer.Nodes.Add(payload + '\n');
                    }
                    break;
                case ApplicationLayerProtocol.DNS:
                    DNSHeader dns = *(DNSHeader*)ptr;
                    applicationLayer = node.Add(string.Format("Domain Name System"));
                    applicationLayer.Nodes.Add(string.Format("Transaction ID: 0x{0:X4}", 
                        Tools.NetworkToHost(dns.identification)));
                    ushort flags = Tools.NetworkToHost(dns.flags);
                    bool qr = (flags & 0x8000) != 0;  // 获取QR位
                    int opcode = (flags & 0x7800) >> 11;  // 获取Opcode字段
                    bool aa = (flags & 0x0400) != 0;  // 获取AA位
                    bool tc = (flags & 0x0200) != 0;  // 获取TC位
                    bool rd = (flags & 0x0100) != 0;  // 获取RD位
                    bool ra = (flags & 0x0080) != 0;  // 获取RA位
                    int rcode = flags & 0x000F;  // 获取RCODE字段
                    TreeNode flag_node = applicationLayer.Nodes.Add(string.Format("Flags: 0x{0:X4} {1}", qr ? 1 : 0, qr ? "Standard query response" : "Standard query"));
                    flag_node.Nodes.Add(string.Format("{0}............... = Response: {0}", qr ? 1 : 0));
                    flag_node.Nodes.Add(string.Format(".{0:X4}........... = Opcode: {0}", opcode));
                    flag_node.Nodes.Add(string.Format(".....{0}.......... = Authoritative: {0}", aa?1:0));
                    flag_node.Nodes.Add(string.Format("......{0}......... = Truncated: {0}", tc? 1 : 0));
                    flag_node.Nodes.Add(string.Format(".......{0}........ = Recursion desired: {0}", rd? 1 : 0));
                    flag_node.Nodes.Add(string.Format("........{0}....... = Recursion available: {0}", ra ? 1 : 0));
                    flag_node.Nodes.Add(string.Format(".........000.... = Z: reserved"));
                    flag_node.Nodes.Add(string.Format("............{0:X4} = Reply code: {0}", rcode));
                    applicationLayer.Nodes.Add(string.Format("Questions: {0}", Tools.NetworkToHost(dns.question)));
                    applicationLayer.Nodes.Add(string.Format("Answer RRs: {0}", Tools.NetworkToHost(dns.answer)));
                    applicationLayer.Nodes.Add(string.Format("Authority RRs: {0}", Tools.NetworkToHost(dns.authority)));
                    applicationLayer.Nodes.Add(string.Format("Additional RRs: {0}", Tools.NetworkToHost(dns.additional)));
                    break;
                case ApplicationLayerProtocol.TLS:
                    applicationLayer = node.Add("Transport Layer Security");
                    byte contentType = *ptr;
                    ptr++;
                    ushort* pointer = (ushort*)ptr;
                    ushort version = Tools.NetworkToHost(*pointer);

                    ptr += sizeof(ushort);
                    pointer++;
                    ushort length = Tools.NetworkToHost(*pointer);
                    ptr += sizeof(ushort);

                    TreeNode tls_node = applicationLayer.Nodes.Add(string.Format("{0} Record Layer: {1}",
                        ProtocolVariable.GetTlsVersion(version), ProtocolVariable.GetTlsContentType(contentType)));

                    tls_node.Nodes.Add(string.Format("Content Type: {0} ({1})",
                        ProtocolVariable.GetTlsContentType(contentType), contentType));
                    tls_node.Nodes.Add(string.Format("Version: {0} (0x{1:X4})",
                        ProtocolVariable.GetTlsVersion(version), version));
                    tls_node.Nodes.Add(string.Format("Length: {0}", length));

                    switch (contentType)
                    {
                        case 22:
                            // Handshake, 读取handshake状态
                            byte handshakeType = *ptr;
                            ptr += 4;  // 跳过length
                            tls_node.Nodes.Add(string.Format("Handshake Protocol: {0}", ProtocolVariable.GetTlsHandshakeType(handshakeType)));
                            break;
                    }
                    break;
            }
        }
    }
}
