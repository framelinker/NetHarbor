using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetHarbor.PacketParser;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NetHarbor
{
    public partial class MainForm : Form
    {
        // Buffer锁，防止多线程冲突问题
        private object bufferLock = new object();
        // 包Buffer，处理前的暂存地
        private Queue<RawCapture> packetBuffer = new Queue<RawCapture>();

        // 消耗包Buffer的线程
        private System.Threading.Thread consumeBufferThread;
        // 标识是否让线程停止, true为停止
        private bool stopConsumeBufferThread = false;

        private PosixTimeval firstPacketTimeval;

        // 包抵达和捕获停止事件
        private PacketArrivalEventHandler arrivalEventHandler;
        private CaptureStoppedEventHandler captureStoppedEventHandler;

        // 统计相关参数
        private ICaptureStatistics captureStatistics;
        private DateTime LastStatisticsTime;
        private TimeSpan LastStatisticsInterval = new TimeSpan(0, 0, 2);

        public MainForm()
        {
            InitializeComponent();
            StopCaptureButton.Enabled = false;
            DoubleBuffered = true;
            // ListView懒加载
            PacketListView.VirtualMode = true;
            PacketListView.RetrieveVirtualItem += PacketListView_RetrieveVirtualItem;
            // UI刷新计时器
            FlushUITimer.Enabled = true;
            FlushUITimer.Interval = 250;
        }

        //刷新接口按钮点击事件
        private void RefreshInterfaceButton_Click(object sender, EventArgs e)
        {
            InterfaceComboBox.Items.Clear();
            var devices = LibPcapLiveDeviceList.Instance;
            if (devices.Count < 1)
            {
                MsgBox.ShowError("没有发现可用的网络接口!");
                return;
            }
            foreach (var dev in devices)
                InterfaceComboBox.Items.Add(String.Format("{0}  {1}", dev.Description, dev.Name));
        }

        //主界面加载完毕事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 界面加载后载入接口列表
            RefreshInterfaceButton.PerformClick();
            // 载入ListView列名
            PacketListView.View = View.Details;
            PacketListView.Columns.Add("No.", 70);
            PacketListView.Columns.Add("Time", 140);
            PacketListView.Columns.Add("Source", 250);
            PacketListView.Columns.Add("Destination", 250);
            PacketListView.Columns.Add("Protocol", 90);
            PacketListView.Columns.Add("Length", 80);
            PacketListView.Columns.Add("Info", 550);
            // 渲染Splitter
            splitContainer2.SplitterDistance = this.Width / 2;
            InfoLabel.Text = "";
        }

        private void StartCaptureButton_Click(object sender, EventArgs e)
        {
            if(InterfaceComboBox.SelectedIndex > 0)
            {
                StartCapture(InterfaceComboBox.SelectedIndex, PromiscuousCheck.Checked, CaptureFilterTextbox.Text);
            }
            else
            {
                MsgBox.ShowError("请先选择网络接口再开始捕获!");
            }
        }
        
        // private ICaptureDevice device; // 网络接口设备
        private LibPcapLiveDevice device;
        private int packetCount;
        private List<PacketWrapper> packetList;
        private List<PacketWrapper> readList;

        private void StartCapture(int interfaceIndex, bool promiscuousMode, string filter)
        {
            device = LibPcapLiveDeviceList.Instance[interfaceIndex];
            // device = CaptureDeviceList.Instance[interfaceIndex];
            packetCount = 0;  // 初始化包序号
            firstPacketTimeval = null;  // 初始化第一个包时间
            PacketListView.VirtualListSize = 0;  // 清除VirtualMode的计数否则会出问题
            PacketListView.Items.Clear();  // 确保清除了计数再清空项目渲染
            packetList = new List<PacketWrapper>();  // 然后再清除packetList
            readList = packetList;

            // 开启设备, 绑定事件
            arrivalEventHandler = new PacketArrivalEventHandler(OnPacketArrive);
            device.OnPacketArrival += arrivalEventHandler;
            captureStoppedEventHandler = new CaptureStoppedEventHandler(OnCaptureStop);
            device.OnCaptureStopped += captureStoppedEventHandler;

            if (promiscuousMode)
            {
                device.Open(DeviceModes.Promiscuous);
            }
            else
            {
                device.Open(DeviceModes.None);
            }

            try
            {
                if (filter != null && filter.Length > 0)
                {
                    device.Filter = filter;
                }
            }
            catch (Exception)
            {
                MsgBox.ShowError(String.Format("捕获过滤器: {0} 不合法!", filter));
                // 卸载事件
                device.OnPacketArrival -= arrivalEventHandler;
                device.OnCaptureStopped -= captureStoppedEventHandler;
                return;
            }

            // 统计数据刷新
            captureStatistics = device.Statistics;

            // 开启包buffer处理线程
            stopConsumeBufferThread = false;
            consumeBufferThread = new System.Threading.Thread(ConsumeBufferThreadOperation);
            consumeBufferThread.Start();

            // 开启捕获
            device.StartCapture();

            //GUI绘制
            StartCaptureButton.Enabled = false;
            StopCaptureButton.Enabled = true;
            InterfaceComboBox.Enabled = false;
            CaptureFilterTextbox.Enabled = false;
            this.Text = String.Format("NetHarbor 正在捕获, 设备 {0}", device.Description);
        }

        // 包到达事件
        void OnPacketArrive(object sender, PacketCapture e)
        {
            // 将包存入buffer, buffer机制可以防止包乱序, lock住防止多线程搞事情
            lock(bufferLock)
            {
                packetBuffer.Enqueue(e.GetPacket());
            }

            // 按时间间隔读取统计数据
            var NowTime = DateTime.Now;
            if ((NowTime - LastStatisticsTime) > LastStatisticsInterval)
            {
                captureStatistics = e.Device.Statistics;
                LastStatisticsTime = NowTime;
            }
        }

        // 停止捕获事件
        void OnCaptureStop(object sender, CaptureStoppedEventStatus status)
        {
            if (status != CaptureStoppedEventStatus.CompletedWithoutError)
            {
                MsgBox.ShowError("停止捕捉过程中发生错误");
            }
        }

        private void StopCaptureButton_Click(object sender, EventArgs e)
        {
            if(device != null)
            {
                StopCapture();
            }
            else
            {
                MsgBox.ShowError("发生异常错误!");
            }
        }

        private void StopCapture()
        {
            if (device != null)
            {
                device.StopCapture();
                this.Text = String.Format("NetHarbor 已停止捕获, 设备 {0}", device.Description);
                device.Close();
                device.OnPacketArrival -= arrivalEventHandler;
                device.OnCaptureStopped -= captureStoppedEventHandler;
                device = null;

                // 关闭消耗包Buffer的线程
                stopConsumeBufferThread = true;
                // 等待线程关闭
                consumeBufferThread.Join();

                // 绘制GUI
                StopCaptureButton.Enabled = false;
                StartCaptureButton.Enabled = true;
                InterfaceComboBox.Enabled = true;
                CaptureFilterTextbox.Enabled = true;
            }
        }

        // 包处理线程内容
        private async void ConsumeBufferThreadOperation()
        {
            RawCapture raw;
            while (!stopConsumeBufferThread)
            {
                // List<RawCapture> processQueue = null;
                Queue<RawCapture> processQueue = null;
                // 检查Buffer中是否有包
                lock (bufferLock)
                {
                    if (packetBuffer.Count != 0)
                    {
                        processQueue = packetBuffer;
                        //packetBuffer = new List<RawCapture>();
                        packetBuffer = new Queue<RawCapture>();
                    }
                }

                if(processQueue == null)
                {
                    System.Threading.Thread.Sleep(0);
                }
                else
                {
                    // Console.WriteLine(String.Format("Consuming... Buffer Len: {0}", processQueue.Count));

                    // 已经处在上一个if的else中，一定有一个包，无需判空
                    while (processQueue.Count > 0)
                    {
                        raw = processQueue.Dequeue();
                        // 时间所限, 无法处理非Ethernet
                        if (raw.LinkLayerType != LinkLayers.Ethernet)
                            continue;
                        packetCount++;
                        var packetWrapper = new PacketWrapper(packetCount, raw, firstPacketTimeval);
                        RoughParser.begin(packetWrapper);
                        packetList.Add(packetWrapper);
                    }

                    if (firstPacketTimeval == null && packetList.Count >= 1)
                    {
                        firstPacketTimeval = packetList[0].Timeval;
                    }

                    // Console.WriteLine(packetList.Count);
                }

            }
        }

        private void PacketListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            readList = packetList;
            if (e.ItemIndex >= 0 && e.ItemIndex < readList.Count)
            {
                PacketWrapper pkt = readList[e.ItemIndex];
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = pkt.Count.ToString();
                listViewItem.SubItems.Add(pkt.RelativeTimeval.ToString());
                listViewItem.SubItems.Add(pkt.TopSource.ToString());
                listViewItem.SubItems.Add(pkt.TopDestination.ToString());
                listViewItem.SubItems.Add(pkt.TopProtocol.ToString());
                listViewItem.SubItems.Add(pkt.Length.ToString());
                listViewItem.SubItems.Add(pkt.TopInfo.ToString());
                listViewItem.BackColor = GetProtocolColor(pkt.TopProtocol);
                // 提供虚拟项的数据
                e.Item = listViewItem;
            }
        }

        // 刷新UI计时器
        private void FlushUITimer_Tick(object sender, EventArgs e)
        {
            uint received = 0;
            if(captureStatistics != null)
            {
                received = captureStatistics.ReceivedPackets;
            }
            PacketListView.VirtualListSize = readList == null ? 0 : readList.Count;
            CaptureStatLabel.Text = string.Format("已收到分组: {0}", received);
        }

        private void exit()
        {
            if(StopCaptureButton.Enabled)
            {
                // 还在捕获, 需要停止
                StopCapture();
            }
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            exit();
        }


        int nowSelectedIndex = -1;
        private void PacketListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ListView.SelectedIndexCollection selectedIndices = PacketListView.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                // 只允许选择一行，所以[0]就够了
                nowSelectedIndex = selectedIndices[0];
                if(nowSelectedIndex >= 0)
                {
                    detailTreeview.Nodes.Clear();
                    PacketWrapper pkt = readList[nowSelectedIndex];
                    hexRichTextbox.Text = FormatByteArray(pkt.Data);
                    DetailedParser.begin(pkt, detailTreeview.Nodes);
                }
            }
        }

        public string FormatByteArray(byte[] byteArray)
        {
            int length = byteArray.Length;
            StringBuilder lineNumberBuilder = new StringBuilder();
            StringBuilder hexBuilder = new StringBuilder();
            StringBuilder charBuilder = new StringBuilder();

            int lineCounter = 0;

            for (int i = 0; i < length; i++)
            {
                hexBuilder.AppendFormat("{0:X2} ", byteArray[i]);
                charBuilder.Append(byteArray[i] >= 32 && byteArray[i] <= 126 ? (char)byteArray[i] : '.');  // 可打印字符显示为字符，其他显示为'.'

                // 在Wireshark样式的输出中，每16个字节分为一组，以空格分隔
                if ((i + 1) % 16 == 0 || i == length - 1)
                {
                    lineNumberBuilder.AppendFormat("{0:X4}  ", lineCounter);
                    hexBuilder.Append("  "); // 每16个字节后加两个空格分隔
                    if(i == length - 1)
                    {
                        // 最后一个字节，需要补齐hex这一行的空格，否则ASCII会错位
                        hexBuilder.Append("".PadRight( (16 - (length % 16)) * 3 ));
                    }
                    hexBuilder.Append(charBuilder.ToString());
                    lineNumberBuilder.AppendLine(hexBuilder.ToString());
                    hexBuilder.Clear();
                    charBuilder.Clear();
                    lineCounter += 16;
                }
            }

            return lineNumberBuilder.ToString();
        }

        public Color GetProtocolColor(string protocol)
        {
            switch(protocol)
            {
                case "TCP":
                case "HTTP":
                    return Color.FromArgb(228, 255, 199);
                case "UDP":
                    return Color.FromArgb(218, 238, 155);
                case "ARP":
                    return Color.FromArgb(250, 240, 215);
                case "DNS":
                    return Color.FromArgb(218, 238, 155);
                case "TLS":
                    return Color.FromArgb(231, 230, 255);
                case "ICMPv6":
                    return Color.FromArgb(252, 224, 255);
                default:
                    return Color.FromArgb(255, 255, 255);
            }
        }
    }
}
