using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 使用顺序布局，Pack = 1 表示按照字节对齐
    public unsafe struct EthernetHeader
    {
        public fixed byte destinationAddress[6];
        public fixed byte sourceAddress[6];
        public ushort type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IPHeader
    {
        public byte versionAndHeadLength;
        public byte typeOfService;
        public ushort totalLength;
        public ushort identification;
        public ushort flagAndOffset;
        public byte ttl;
        public byte protocol;
        public ushort checksum;
        public uint sourceAddress;
        public uint destinationAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IPv6Header 
    {
        public uint versionAndTrafficClassAndFlowLabel;
        public ushort payloadLength;
        public byte nextHeader;
        public byte hopLimit;
        public fixed byte sourceAddress[16];
        public fixed byte destinationAddress[16];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ICMPv6Header
    {
        public byte type;        // ICMPv6消息类型
        public byte code;        // 消息代码
        public ushort checksum;  // 校验和
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TCPHeader
    {
        public ushort sourcePort;
        public ushort destinationPort;
        public uint sequence;
        public uint acknowledgement;
        public byte headerLength;
        public byte flags;
        public ushort windowSize;
        public ushort checksum;
        public ushort urgent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UDPHeader
    {
        public ushort sourcePort;
        public ushort destinationPort;
        public ushort dataLength;
        public ushort checksum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ICMPHeader
    {
        public byte type;
        public byte code;
        public ushort checksum;
        public ushort identification;
        public ushort sequence;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ARPHeader
    {
        public ushort hardwareType;
        public ushort protocolType;
        public byte macLength;
        public byte ipLength;
        public ushort operationCode;
        public fixed byte sourceEthernetAddress[6];
        public uint sourceIPAddress;
        public fixed byte destinationEthernetAddress[6];
        public uint destinationIPAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DNSHeader     // 12 byte
    {
        public ushort identification; // Identification [2 byte]
        public ushort flags;          // Flags [total 2 byte]
        public ushort question;       // Question Number [2 byte]
        public ushort answer;         // Answer RRs [2 byte]
        public ushort authority;      // Authority RRs [2 byte]
        public ushort additional;     // Additional RRs [2 byte]
    }
}
