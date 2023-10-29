using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHarbor.PacketParser
{
    public class PacketWrapper
    {
        public byte[] Data;

        public int Count { get; private set; }
        public PosixTimeval RelativeTimeval { get; private set; }
        public PosixTimeval Timeval { get; private set; }
        public LinkLayers LinkLayerProtocol { get; private set; }
        public int Length { get; private set; }

        // 用于总体展示的最顶层的信息，加载时能够快速读取
        public string TopInfo { get; set; }
        public string TopSource { get; set; }
        public string TopDestination { get; set; }
        public string TopProtocol { get; set; }

        // 用于单独展示的包层级详细信息，分析时再去读取

        public string LinkLayerDestination { get; set; }
        public string LinkLayerSource { get; set; }

        public NetworkLayerProtocol NetworkLayerProtocol { get; set; }
        public string NetworkLayerDestination {  get; set; }
        public string NetworkLayerSource {  get; set; }
        public int NetworkLayerHeaderLength {  get; set; }
        public int NetworkLayerPayloadLength {  get; set; }

        public TransmissionLayerProtocol TransmissionLayerProtocol { get; set; }
        public ushort TransmissionSourcePort {  get; set; }
        public ushort TransmissionDestinationPort { get; set; }
        public int TransmissionLayerPayloadLength { get; set; }

        public byte[] TransmissionPayload { get; set; }


        public ApplicationLayerProtocol ApplicationLayerProtocol { get; set; }
        

        public PacketWrapper(int count, RawCapture p, PosixTimeval FirstPacketTimeval)
        {
            this.Count = count;
            this.Data = p.Data;
            this.Timeval = p.Timeval;
            this.LinkLayerProtocol = p.LinkLayerType;
            this.Length = p.PacketLength;

            TopInfo = "Info"; TopSource = "SRC."; TopDestination = "DEST."; TopProtocol = LinkLayerProtocol.ToString();

            //计算相对时间差值
            RelativeTimeval = new PosixTimeval();
            if(FirstPacketTimeval == null)
            {
                // 没有第一个包时间，代表没有相对时间差
                RelativeTimeval.Seconds = 0;
                RelativeTimeval.MicroSeconds = 0;
            }
            else
            {
                // 否则计算当前包与第一个包的时间差值
                RelativeTimeval.Seconds = p.Timeval.Seconds - FirstPacketTimeval.Seconds;
                // 当前应用情景不会出现微秒数差值大于long的情况，所以放心相减
                long microSecondsDifference = (long)(p.Timeval.MicroSeconds - FirstPacketTimeval.MicroSeconds);
                // 处理秒和微秒之间的进位
                if (microSecondsDifference < 0)
                {
                    microSecondsDifference += 1000000; // 1秒 = 1000000微秒
                    RelativeTimeval.Seconds--; // 秒数减一
                }
                // 当前情景long转ulong不会溢出
                RelativeTimeval.MicroSeconds = (ulong)microSecondsDifference;
                // Console.WriteLine(String.Format("{0} {1} {2}", p.Timeval, FirstPacketTimeval, RelativeTimeval));
            }
        }
    }
}
