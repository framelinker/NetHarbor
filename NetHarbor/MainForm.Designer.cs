namespace NetHarbor
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StatStrip = new System.Windows.Forms.StatusStrip();
            this.CaptureStatLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SideMenu = new System.Windows.Forms.ToolStrip();
            this.StartCaptureButton = new System.Windows.Forms.ToolStripButton();
            this.StopCaptureButton = new System.Windows.Forms.ToolStripButton();
            this.ExitButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CaptureFilterTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PromiscuousCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.RefreshInterfaceButton = new System.Windows.Forms.Button();
            this.InterfaceComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.PacketListView = new System.Windows.Forms.ListView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.detailTreeview = new System.Windows.Forms.TreeView();
            this.hexRichTextbox = new System.Windows.Forms.RichTextBox();
            this.FlushUITimer = new System.Windows.Forms.Timer(this.components);
            this.InfoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatStrip.SuspendLayout();
            this.SideMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatStrip
            // 
            this.StatStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CaptureStatLabel,
            this.toolStripStatusLabel2,
            this.InfoLabel});
            this.StatStrip.Location = new System.Drawing.Point(0, 730);
            this.StatStrip.Name = "StatStrip";
            this.StatStrip.Size = new System.Drawing.Size(1231, 31);
            this.StatStrip.TabIndex = 1;
            this.StatStrip.Text = "statusStrip1";
            // 
            // CaptureStatLabel
            // 
            this.CaptureStatLabel.Name = "CaptureStatLabel";
            this.CaptureStatLabel.Size = new System.Drawing.Size(159, 24);
            this.CaptureStatLabel.Text = "CaptureStatLabel";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(922, 24);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // SideMenu
            // 
            this.SideMenu.AutoSize = false;
            this.SideMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.SideMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartCaptureButton,
            this.StopCaptureButton,
            this.ExitButton});
            this.SideMenu.Location = new System.Drawing.Point(0, 0);
            this.SideMenu.Name = "SideMenu";
            this.SideMenu.Size = new System.Drawing.Size(1231, 72);
            this.SideMenu.TabIndex = 2;
            this.SideMenu.Text = "toolStrip1";
            // 
            // StartCaptureButton
            // 
            this.StartCaptureButton.Image = global::NetHarbor.Properties.Resources.play_fill;
            this.StartCaptureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StartCaptureButton.Name = "StartCaptureButton";
            this.StartCaptureButton.Size = new System.Drawing.Size(86, 67);
            this.StartCaptureButton.Text = "开始捕获";
            this.StartCaptureButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.StartCaptureButton.Click += new System.EventHandler(this.StartCaptureButton_Click);
            // 
            // StopCaptureButton
            // 
            this.StopCaptureButton.Image = global::NetHarbor.Properties.Resources.stop_fill;
            this.StopCaptureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopCaptureButton.Name = "StopCaptureButton";
            this.StopCaptureButton.Size = new System.Drawing.Size(86, 67);
            this.StopCaptureButton.Text = "停止捕获";
            this.StopCaptureButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.StopCaptureButton.Click += new System.EventHandler(this.StopCaptureButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ExitButton.Image = global::NetHarbor.Properties.Resources.close_fill;
            this.ExitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(86, 67);
            this.ExitButton.Text = "退出程序";
            this.ExitButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CaptureFilterTextbox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1225, 34);
            this.panel1.TabIndex = 5;
            // 
            // CaptureFilterTextbox
            // 
            this.CaptureFilterTextbox.Location = new System.Drawing.Point(116, 5);
            this.CaptureFilterTextbox.Name = "CaptureFilterTextbox";
            this.CaptureFilterTextbox.Size = new System.Drawing.Size(1005, 25);
            this.CaptureFilterTextbox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "捕获过滤器";
            // 
            // PromiscuousCheck
            // 
            this.PromiscuousCheck.AutoSize = true;
            this.PromiscuousCheck.Checked = true;
            this.PromiscuousCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PromiscuousCheck.Location = new System.Drawing.Point(1244, 8);
            this.PromiscuousCheck.Name = "PromiscuousCheck";
            this.PromiscuousCheck.Size = new System.Drawing.Size(93, 21);
            this.PromiscuousCheck.TabIndex = 3;
            this.PromiscuousCheck.Text = "混杂模式";
            this.PromiscuousCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F);
            this.label1.Location = new System.Drawing.Point(17, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "网络接口";
            // 
            // RefreshInterfaceButton
            // 
            this.RefreshInterfaceButton.Font = new System.Drawing.Font("宋体", 7F);
            this.RefreshInterfaceButton.Location = new System.Drawing.Point(1137, 5);
            this.RefreshInterfaceButton.Name = "RefreshInterfaceButton";
            this.RefreshInterfaceButton.Size = new System.Drawing.Size(88, 23);
            this.RefreshInterfaceButton.TabIndex = 2;
            this.RefreshInterfaceButton.Text = "刷新接口";
            this.RefreshInterfaceButton.UseVisualStyleBackColor = true;
            this.RefreshInterfaceButton.Click += new System.EventHandler(this.RefreshInterfaceButton_Click);
            // 
            // InterfaceComboBox
            // 
            this.InterfaceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterfaceComboBox.FormattingEnabled = true;
            this.InterfaceComboBox.Location = new System.Drawing.Point(116, 6);
            this.InterfaceComboBox.Name = "InterfaceComboBox";
            this.InterfaceComboBox.Size = new System.Drawing.Size(1005, 23);
            this.InterfaceComboBox.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 72);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1231, 658);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.PromiscuousCheck);
            this.panel2.Controls.Add(this.InterfaceComboBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.RefreshInterfaceButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1225, 34);
            this.panel2.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 83);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.PacketListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1225, 712);
            this.splitContainer1.SplitterDistance = 360;
            this.splitContainer1.TabIndex = 8;
            // 
            // PacketListView
            // 
            this.PacketListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PacketListView.FullRowSelect = true;
            this.PacketListView.HideSelection = false;
            this.PacketListView.Location = new System.Drawing.Point(0, 0);
            this.PacketListView.MultiSelect = false;
            this.PacketListView.Name = "PacketListView";
            this.PacketListView.Size = new System.Drawing.Size(1225, 360);
            this.PacketListView.TabIndex = 0;
            this.PacketListView.UseCompatibleStateImageBehavior = false;
            this.PacketListView.SelectedIndexChanged += new System.EventHandler(this.PacketListView_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.detailTreeview);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.hexRichTextbox);
            this.splitContainer2.Size = new System.Drawing.Size(1225, 348);
            this.splitContainer2.SplitterDistance = 363;
            this.splitContainer2.TabIndex = 0;
            // 
            // detailTreeview
            // 
            this.detailTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailTreeview.Location = new System.Drawing.Point(0, 0);
            this.detailTreeview.Name = "detailTreeview";
            this.detailTreeview.Size = new System.Drawing.Size(363, 348);
            this.detailTreeview.TabIndex = 0;
            // 
            // hexRichTextbox
            // 
            this.hexRichTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexRichTextbox.Location = new System.Drawing.Point(0, 0);
            this.hexRichTextbox.Name = "hexRichTextbox";
            this.hexRichTextbox.Size = new System.Drawing.Size(858, 348);
            this.hexRichTextbox.TabIndex = 0;
            this.hexRichTextbox.Text = "";
            // 
            // FlushUITimer
            // 
            this.FlushUITimer.Tick += new System.EventHandler(this.FlushUITimer_Tick);
            // 
            // InfoLabel
            // 
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(89, 24);
            this.InfoLabel.Text = "InfoLabel";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1231, 761);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.SideMenu);
            this.Controls.Add(this.StatStrip);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "NetHarbor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.StatStrip.ResumeLayout(false);
            this.StatStrip.PerformLayout();
            this.SideMenu.ResumeLayout(false);
            this.SideMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip StatStrip;
        private System.Windows.Forms.ToolStrip SideMenu;
        private System.Windows.Forms.ToolStripButton StartCaptureButton;
        private System.Windows.Forms.ToolStripStatusLabel CaptureStatLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox PromiscuousCheck;
        private System.Windows.Forms.Button RefreshInterfaceButton;
        private System.Windows.Forms.ComboBox InterfaceComboBox;
        private System.Windows.Forms.ToolStripButton StopCaptureButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CaptureFilterTextbox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView PacketListView;
        private System.Windows.Forms.Timer FlushUITimer;
        private System.Windows.Forms.ToolStripButton ExitButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox hexRichTextbox;
        private System.Windows.Forms.TreeView detailTreeview;
        private System.Windows.Forms.ToolStripStatusLabel InfoLabel;
    }
}

