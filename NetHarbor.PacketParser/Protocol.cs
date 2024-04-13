using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    /*public enum TLSHandshake : byte
    {
        [Description("Hello Request")]
        HelloRequest = 0,
        [Description("Client Hello")]
        ClientHello = 1,
        [Description("Server Hello")]
        ServerHello = 2,
        [Description("Certificate")]
        Certificate = 11,
        [Description("Certificate Request")]
        CertificateRequest = 13,
        [Description("Client Key Exchange")]
        ClientKeyExchange = 16,
        [Description("New Session Ticket")]
        NewSessionTicket = 4,
        [Description("Server Key Exchange")]
        ServerKeyExchange = 12,
        [Description("Server Hello Done")]
        ServerHelloDone = 14,
        [Description("Certificate Verify")]
        CertificateVerify = 15,
        [Description("Finished")]
        Finished = 20
    }

    public enum TLSRecord : byte
    {
        [Description("Change Cipher Spec")]
        ChangeCipherSpec = 20,
        [Description("Alert")]
        Alert = 21,
        [Description("Handshake")]
        Handshake = 22,
        [Description("Application Data")]
        ApplicationData = 23
    }

    public enum TLSVersion : ushort
    {
        [Description("SSL v3.0")]
        SSL3d0 = 0x0300,
        [Description("TLS v1.0")]
        TLS1d0 = 0x0301,
        [Description("TLS v1.1")]
        TLS1d1 = 0x0302,
        [Description("TLS v1.2")]
        TLS1d2 = 0x0303,
        [Description("TLS v1.3")]
        TLS1d3 = 0x0304
    }

    public enum TLSExtension : ushort
    {
        Default = -1,
        server_name = 0,
        max_fragment_length = 1,
        client_certificate_url = 2,
        trusted_ca_keys = 3,
        truncated_hmac = 4,
        status_request = 5,
        user_mapping = 6,
        client_authz = 7,
        server_authz = 8,
        cert_type = 9,
        supported_groups = 10,
        ec_point_formats = 11,
        srp = 12,
        signature_algorithms = 13,
        use_srtp = 14,
        heartbeat = 15,
        application_layer_protocol_negotiation = 16,
        status_request_v2 = 17,
        signed_certificate_timestamp = 18,
        client_certificate_type = 19,
        server_certificate_type = 20,
        padding = 21,
        encrypt_then_mac = 22,
        extended_master_secret = 23,
        token_binding = 24,
        cached_info = 25,
        tls_tls = 26,
        compress_certificate = 27,
        record_size_limit = 28,
        pwd_protect = 29,
        pwd_clear = 30,
        password_salt = 31,
        session_ticket = 35,
        renegotiation_info = 65281,
        pre_shared_key = 41,
        early_data = 42,
        supported_versions = 43,
        cookie = 44,
        psk_key_exchange_modes = 45,
        certificate_authorities = 47,
        oid_filters = 48,
        post_handshake_auth = 49,
        signature_algorithms_cert = 50,
    }

    public enum ARPOperation : ushort
    {
        request = 1,
        reply = 2
    }

    public enum ICMPType : byte
    {
        [Description("Echo Reply")]
        EchoReply = 0,

        [Description("Destination Unreachable")]
        DestinationUnreachable = 3,

        [Description("Redirect")]
        Redirect = 5,

        [Description("Echo Request")]
        EchoRequest = 8,

        [Description("Time Exceeded")]
        TimeExceeded = 11,

        [Description("Parameter Problem")]
        ParameterProblem = 12,

        [Description("Timestamp Request")]
        TimestampRequest = 13,

        [Description("Timestamp Reply")]
        TimestampReply = 14,

        [Description("Address Mask Request")]
        AddressMaskRequest = 17,

        [Description("Address Mask Reply")]
        AddressMaskReply = 18
    }

    public enum ICMPv6Type : byte
    {
        [Description("Destination Unreachable")]
        DestinationUnreachable = 1,

        [Description("Packet Too Big")]
        PacketTooBig = 2,

        [Description("Time Exceeded")]
        TimeExceeded1 = 3,

        [Description("Time Exceeded")]
        TimeExceeded2 = 4,

        [Description("Echo Request")]
        EchoRequest = 128,

        [Description("Echo Reply")]
        EchoReply = 129,

        [Description("Router Solicitation")]
        RouterSolicitation = 133,

        [Description("Router Advertisement")]
        RouterAdvertisement = 134,

        [Description("Neighbor Solicitation")]
        NeighborSolicitation = 135,

        [Description("Neighbor Advertisement")]
        NeighborAdvertisement = 136
    }

    static class EnumExtensions
    {
        public static string GetDescription(this Enum val)
        {
            if (val == null)
            {
                return "";
            }
            var field = val.GetType().GetField(val.ToString());
            if (field == null)
            {
                return val.ToString();
            }
            var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            if (customAttribute == null) { return val.ToString(); }
            else { return ((DescriptionAttribute)customAttribute).Description; }
        }
    }*/

    public class ProtocolVariable
    {
        public static string GetTlsHandshakeType(byte type)
        {
            switch (type)
            {
                //case 0: return "Hello Request";
                case 1: return "Client Hello";
                case 2: return "Server Hello";
                case 11: return "Certificate";
                case 16: return "Client Key Exchange";
                case 4: return "New Session Ticket";
                case 12: return "Server Key Exchange";
                case 14: return "Server Hello Done";
                case 15: return "Certificate Verify";
                case 20: return "Finished";
                default: return "";
            }
        }

        public static string GetTlsRecordType(byte type)
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

        public static string GetTlsExtension(ushort type)
        {
            switch (type)
            {
                case 0: return "server_name";
                case 1: return "max_fragment_length";
                case 2: return "client_certificate_url";
                case 3: return "trusted_ca_keys";
                case 4: return "truncated_hmac";
                case 5: return "status_request";
                case 6: return "user_mapping";
                case 7: return "client_authz";
                case 8: return "server_authz";
                case 9: return "cert_type";
                case 10: return "supported_groups";
                case 11: return "ec_point_formats";
                case 12: return "srp";
                case 13: return "signature_algorithms";
                case 14: return "use_srtp";
                case 15: return "heartbeat";
                case 16: return "application_layer_protocol_negotiation";
                case 17: return "status_request_v2";
                case 18: return "signed_certificate_timestamp";
                case 19: return "client_certificate_type";
                case 20: return "server_certificate_type";
                case 21: return "padding";
                case 22: return "encrypt_then_mac";
                case 23: return "extended_master_secret";
                case 24: return "token_binding";
                case 25: return "cached_info";
                case 26: return "tls_tls";
                case 27: return "compress_certificate";
                case 28: return "record_size_limit";
                case 29: return "pwd_protect";
                case 30: return "pwd_clear";
                case 31: return "password_salt";
                case 35: return "session_ticket";
                case 65281: return "renegotiation_info";
                case 41: return "pre_shared_key";
                case 42: return "early_data";
                case 43: return "supported_versions";
                case 44: return "cookie";
                case 45: return "psk_key_exchange_modes";
                case 47: return "certificate_authorities";
                case 48: return "oid_filters";
                case 49: return "post_handshake_auth";
                case 50: return "signature_algorithms_cert";
                case 51: return "key_share";
                case 17513: return "application_settings";
                case 65037: return "encrypted_client_hello";
                default: return "RESERVED / UNASSIGNED";
            }
        }

        public static string GetTlsServerNameType(byte type)
        {
            switch (type)
            {
                case 0:
                    return "host_name";
                default:
                    return "?";
            }
        }

        public static string GetARPOperation(ushort opcode)
        {
            switch (opcode)
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
            switch (type)
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
