using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace USB_IO2
{
    partial class MainF : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainF));
            labelMsg = new Label();
            LogLb = new ListBox();
            SyncStartBtn = new Button();
            ConfigGb = new GroupBox();
            pullUpChk = new CheckBox();
            InLbl = new Label();
            OutLbl = new Label();
            InPnl = new Panel();
            OutPnl = new Panel();
            dirBtn2_0 = new Button();
            dirBtn2_1 = new Button();
            dirBtn2_2 = new Button();
            dirBtn2_3 = new Button();
            dirBtn1_4 = new Button();
            dirBtn1_3 = new Button();
            dirBtn1_7 = new Button();
            dirBtn1_2 = new Button();
            dirBtn1_5 = new Button();
            dirBtn1_1 = new Button();
            dirBtn1_6 = new Button();
            dirBtn1_0 = new Button();
            setDirectionBtn = new Button();
            valueGb = new GroupBox();
            nekoPb = new PictureBox();
            OffLbl = new Label();
            OffPnl = new Panel();
            OnLbl = new Label();
            OnPnl = new Panel();
            statusBtn2_0 = new Button();
            statusBtn2_1 = new Button();
            statusBtn2_2 = new Button();
            statusBtn2_3 = new Button();
            statusBtn1_4 = new Button();
            statusBtn1_5 = new Button();
            statusBtn1_6 = new Button();
            statusBtn1_7 = new Button();
            statusBtn1_0 = new Button();
            statusBtn1_1 = new Button();
            statusBtn1_2 = new Button();
            statusBtn1_3 = new Button();
            SyncStopBtn = new Button();
            ConfigGb.SuspendLayout();
            valueGb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nekoPb).BeginInit();
            SuspendLayout();
            // 
            // labelMsg
            // 
            labelMsg.AutoSize = true;
            labelMsg.Location = new Point(14, 316);
            labelMsg.Margin = new Padding(4, 0, 4, 0);
            labelMsg.Name = "labelMsg";
            labelMsg.Size = new Size(64, 15);
            labelMsg.TabIndex = 14;
            labelMsg.Text = "Messages :";
            // 
            // LogLb
            // 
            LogLb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LogLb.ItemHeight = 15;
            LogLb.Location = new Point(14, 332);
            LogLb.Margin = new Padding(4);
            LogLb.Name = "LogLb";
            LogLb.Size = new Size(410, 169);
            LogLb.TabIndex = 13;
            // 
            // SyncStartBtn
            // 
            SyncStartBtn.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Bold);
            SyncStartBtn.Location = new Point(14, 14);
            SyncStartBtn.Margin = new Padding(4);
            SyncStartBtn.Name = "SyncStartBtn";
            SyncStartBtn.Size = new Size(286, 29);
            SyncStartBtn.TabIndex = 12;
            SyncStartBtn.Text = "Sync Start  (Push to Begin!)";
            SyncStartBtn.UseVisualStyleBackColor = true;
            SyncStartBtn.Click += SyncStartBtn_Click;
            // 
            // ConfigGb
            // 
            ConfigGb.Controls.Add(pullUpChk);
            ConfigGb.Controls.Add(InLbl);
            ConfigGb.Controls.Add(OutLbl);
            ConfigGb.Controls.Add(InPnl);
            ConfigGb.Controls.Add(OutPnl);
            ConfigGb.Controls.Add(dirBtn2_0);
            ConfigGb.Controls.Add(dirBtn2_1);
            ConfigGb.Controls.Add(dirBtn2_2);
            ConfigGb.Controls.Add(dirBtn2_3);
            ConfigGb.Controls.Add(dirBtn1_4);
            ConfigGb.Controls.Add(dirBtn1_3);
            ConfigGb.Controls.Add(dirBtn1_7);
            ConfigGb.Controls.Add(dirBtn1_2);
            ConfigGb.Controls.Add(dirBtn1_5);
            ConfigGb.Controls.Add(dirBtn1_1);
            ConfigGb.Controls.Add(dirBtn1_6);
            ConfigGb.Controls.Add(dirBtn1_0);
            ConfigGb.Location = new Point(14, 50);
            ConfigGb.Margin = new Padding(4);
            ConfigGb.Name = "ConfigGb";
            ConfigGb.Padding = new Padding(4);
            ConfigGb.Size = new Size(411, 129);
            ConfigGb.TabIndex = 16;
            ConfigGb.TabStop = false;
            ConfigGb.Text = "Configuration / Change";
            // 
            // pullUpChk
            // 
            pullUpChk.AutoSize = true;
            pullUpChk.Font = new Font("MS UI Gothic", 9F, FontStyle.Bold);
            pullUpChk.Location = new Point(206, 86);
            pullUpChk.Margin = new Padding(4);
            pullUpChk.Name = "pullUpChk";
            pullUpChk.Size = new Size(116, 16);
            pullUpChk.TabIndex = 9;
            pullUpChk.Text = "Pullup J2 Input";
            pullUpChk.UseVisualStyleBackColor = true;
            // 
            // InLbl
            // 
            InLbl.AutoSize = true;
            InLbl.ImageAlign = ContentAlignment.MiddleLeft;
            InLbl.Location = new Point(371, 96);
            InLbl.Margin = new Padding(4, 0, 4, 0);
            InLbl.Name = "InLbl";
            InLbl.Size = new Size(19, 15);
            InLbl.TabIndex = 19;
            InLbl.Text = "IN";
            // 
            // OutLbl
            // 
            OutLbl.AutoSize = true;
            OutLbl.ImageAlign = ContentAlignment.MiddleLeft;
            OutLbl.Location = new Point(371, 74);
            OutLbl.Margin = new Padding(4, 0, 4, 0);
            OutLbl.Name = "OutLbl";
            OutLbl.Size = new Size(30, 15);
            OutLbl.TabIndex = 18;
            OutLbl.Text = "OUT";
            // 
            // InPnl
            // 
            InPnl.BorderStyle = BorderStyle.FixedSingle;
            InPnl.Location = new Point(350, 96);
            InPnl.Margin = new Padding(4);
            InPnl.Name = "InPnl";
            InPnl.Size = new Size(14, 14);
            InPnl.TabIndex = 8;
            // 
            // OutPnl
            // 
            OutPnl.BorderStyle = BorderStyle.FixedSingle;
            OutPnl.Location = new Point(350, 74);
            OutPnl.Margin = new Padding(4);
            OutPnl.Name = "OutPnl";
            OutPnl.Size = new Size(14, 14);
            OutPnl.TabIndex = 7;
            // 
            // dirBtn2_0
            // 
            dirBtn2_0.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn2_0.Location = new Point(10, 74);
            dirBtn2_0.Margin = new Padding(4);
            dirBtn2_0.Name = "dirBtn2_0";
            dirBtn2_0.Size = new Size(41, 44);
            dirBtn2_0.TabIndex = 6;
            dirBtn2_0.Text = "0";
            dirBtn2_0.UseVisualStyleBackColor = true;
            dirBtn2_0.BackColorChanged += btn_BackColorChanged;
            dirBtn2_0.Click += DirBtn_Click;
            // 
            // dirBtn2_1
            // 
            dirBtn2_1.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn2_1.Location = new Point(59, 74);
            dirBtn2_1.Margin = new Padding(4);
            dirBtn2_1.Name = "dirBtn2_1";
            dirBtn2_1.Size = new Size(41, 44);
            dirBtn2_1.TabIndex = 6;
            dirBtn2_1.Text = "1";
            dirBtn2_1.UseVisualStyleBackColor = true;
            dirBtn2_1.BackColorChanged += btn_BackColorChanged;
            dirBtn2_1.Click += DirBtn_Click;
            // 
            // dirBtn2_2
            // 
            dirBtn2_2.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn2_2.Location = new Point(108, 74);
            dirBtn2_2.Margin = new Padding(4);
            dirBtn2_2.Name = "dirBtn2_2";
            dirBtn2_2.Size = new Size(41, 44);
            dirBtn2_2.TabIndex = 6;
            dirBtn2_2.Text = "2";
            dirBtn2_2.UseVisualStyleBackColor = true;
            dirBtn2_2.BackColorChanged += btn_BackColorChanged;
            dirBtn2_2.Click += DirBtn_Click;
            // 
            // dirBtn2_3
            // 
            dirBtn2_3.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn2_3.Location = new Point(158, 74);
            dirBtn2_3.Margin = new Padding(4);
            dirBtn2_3.Name = "dirBtn2_3";
            dirBtn2_3.Size = new Size(41, 44);
            dirBtn2_3.TabIndex = 6;
            dirBtn2_3.Text = "3";
            dirBtn2_3.UseVisualStyleBackColor = true;
            dirBtn2_3.BackColorChanged += btn_BackColorChanged;
            dirBtn2_3.Click += DirBtn_Click;
            // 
            // dirBtn1_4
            // 
            dirBtn1_4.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_4.Location = new Point(206, 22);
            dirBtn1_4.Margin = new Padding(4);
            dirBtn1_4.Name = "dirBtn1_4";
            dirBtn1_4.Size = new Size(41, 44);
            dirBtn1_4.TabIndex = 6;
            dirBtn1_4.Text = "4";
            dirBtn1_4.UseVisualStyleBackColor = true;
            dirBtn1_4.BackColorChanged += btn_BackColorChanged;
            dirBtn1_4.Click += DirBtn_Click;
            // 
            // dirBtn1_3
            // 
            dirBtn1_3.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_3.Location = new Point(158, 22);
            dirBtn1_3.Margin = new Padding(4);
            dirBtn1_3.Name = "dirBtn1_3";
            dirBtn1_3.Size = new Size(41, 44);
            dirBtn1_3.TabIndex = 6;
            dirBtn1_3.Text = "3";
            dirBtn1_3.UseVisualStyleBackColor = true;
            dirBtn1_3.BackColorChanged += btn_BackColorChanged;
            dirBtn1_3.Click += DirBtn_Click;
            // 
            // dirBtn1_7
            // 
            dirBtn1_7.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_7.Location = new Point(354, 22);
            dirBtn1_7.Margin = new Padding(4);
            dirBtn1_7.Name = "dirBtn1_7";
            dirBtn1_7.Size = new Size(41, 44);
            dirBtn1_7.TabIndex = 6;
            dirBtn1_7.Text = "7";
            dirBtn1_7.UseVisualStyleBackColor = true;
            dirBtn1_7.BackColorChanged += btn_BackColorChanged;
            dirBtn1_7.Click += DirBtn_Click;
            // 
            // dirBtn1_2
            // 
            dirBtn1_2.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_2.Location = new Point(108, 22);
            dirBtn1_2.Margin = new Padding(4);
            dirBtn1_2.Name = "dirBtn1_2";
            dirBtn1_2.Size = new Size(41, 44);
            dirBtn1_2.TabIndex = 6;
            dirBtn1_2.Text = "2";
            dirBtn1_2.UseVisualStyleBackColor = true;
            dirBtn1_2.BackColorChanged += btn_BackColorChanged;
            dirBtn1_2.Click += DirBtn_Click;
            // 
            // dirBtn1_5
            // 
            dirBtn1_5.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_5.Location = new Point(255, 22);
            dirBtn1_5.Margin = new Padding(4);
            dirBtn1_5.Name = "dirBtn1_5";
            dirBtn1_5.Size = new Size(41, 44);
            dirBtn1_5.TabIndex = 6;
            dirBtn1_5.Text = "5";
            dirBtn1_5.UseVisualStyleBackColor = true;
            dirBtn1_5.BackColorChanged += btn_BackColorChanged;
            dirBtn1_5.Click += DirBtn_Click;
            // 
            // dirBtn1_1
            // 
            dirBtn1_1.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_1.Location = new Point(59, 22);
            dirBtn1_1.Margin = new Padding(4);
            dirBtn1_1.Name = "dirBtn1_1";
            dirBtn1_1.Size = new Size(41, 44);
            dirBtn1_1.TabIndex = 6;
            dirBtn1_1.Text = "1";
            dirBtn1_1.UseVisualStyleBackColor = true;
            dirBtn1_1.BackColorChanged += btn_BackColorChanged;
            dirBtn1_1.Click += DirBtn_Click;
            // 
            // dirBtn1_6
            // 
            dirBtn1_6.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_6.Location = new Point(304, 22);
            dirBtn1_6.Margin = new Padding(4);
            dirBtn1_6.Name = "dirBtn1_6";
            dirBtn1_6.Size = new Size(41, 44);
            dirBtn1_6.TabIndex = 6;
            dirBtn1_6.Text = "6";
            dirBtn1_6.UseVisualStyleBackColor = true;
            dirBtn1_6.BackColorChanged += btn_BackColorChanged;
            dirBtn1_6.Click += DirBtn_Click;
            // 
            // dirBtn1_0
            // 
            dirBtn1_0.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            dirBtn1_0.Location = new Point(10, 22);
            dirBtn1_0.Margin = new Padding(4);
            dirBtn1_0.Name = "dirBtn1_0";
            dirBtn1_0.Size = new Size(41, 44);
            dirBtn1_0.TabIndex = 6;
            dirBtn1_0.Text = "0";
            dirBtn1_0.UseVisualStyleBackColor = true;
            dirBtn1_0.BackColorChanged += btn_BackColorChanged;
            dirBtn1_0.Click += DirBtn_Click;
            // 
            // setDirectionBtn
            // 
            setDirectionBtn.Font = new Font("MS UI Gothic", 9F, FontStyle.Bold);
            setDirectionBtn.Location = new Point(307, 14);
            setDirectionBtn.Margin = new Padding(4);
            setDirectionBtn.Name = "setDirectionBtn";
            setDirectionBtn.Size = new Size(118, 29);
            setDirectionBtn.TabIndex = 20;
            setDirectionBtn.Text = "Set Direction";
            setDirectionBtn.UseVisualStyleBackColor = true;
            setDirectionBtn.Click += setDirectionBtn_Click;
            // 
            // valueGb
            // 
            valueGb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            valueGb.BackColor = Color.LightPink;
            valueGb.Controls.Add(nekoPb);
            valueGb.Controls.Add(OffLbl);
            valueGb.Controls.Add(OffPnl);
            valueGb.Controls.Add(OnLbl);
            valueGb.Controls.Add(OnPnl);
            valueGb.Controls.Add(statusBtn2_0);
            valueGb.Controls.Add(statusBtn2_1);
            valueGb.Controls.Add(statusBtn2_2);
            valueGb.Controls.Add(statusBtn2_3);
            valueGb.Controls.Add(statusBtn1_4);
            valueGb.Controls.Add(statusBtn1_5);
            valueGb.Controls.Add(statusBtn1_6);
            valueGb.Controls.Add(statusBtn1_7);
            valueGb.Controls.Add(statusBtn1_0);
            valueGb.Controls.Add(statusBtn1_1);
            valueGb.Controls.Add(statusBtn1_2);
            valueGb.Controls.Add(statusBtn1_3);
            valueGb.Location = new Point(14, 184);
            valueGb.Margin = new Padding(4);
            valueGb.Name = "valueGb";
            valueGb.Padding = new Padding(4);
            valueGb.Size = new Size(411, 129);
            valueGb.TabIndex = 17;
            valueGb.TabStop = false;
            valueGb.Text = "Signal Value / Change (OUT signal only)";
            // 
            // nekoPb
            // 
            nekoPb.BackgroundImageLayout = ImageLayout.Center;
            nekoPb.Image = Properties.Resources.InImage;
            nekoPb.Location = new Point(234, 78);
            nekoPb.Margin = new Padding(4);
            nekoPb.Name = "nekoPb";
            nekoPb.Size = new Size(77, 39);
            nekoPb.TabIndex = 18;
            nekoPb.TabStop = false;
            // 
            // OffLbl
            // 
            OffLbl.AutoSize = true;
            OffLbl.ImageAlign = ContentAlignment.MiddleLeft;
            OffLbl.Location = new Point(371, 102);
            OffLbl.Margin = new Padding(4, 0, 4, 0);
            OffLbl.Name = "OffLbl";
            OffLbl.Size = new Size(28, 15);
            OffLbl.TabIndex = 17;
            OffLbl.Text = "OFF";
            // 
            // OffPnl
            // 
            OffPnl.BackColor = Color.White;
            OffPnl.BorderStyle = BorderStyle.FixedSingle;
            OffPnl.Location = new Point(350, 102);
            OffPnl.Margin = new Padding(4);
            OffPnl.Name = "OffPnl";
            OffPnl.Size = new Size(14, 14);
            OffPnl.TabIndex = 16;
            // 
            // OnLbl
            // 
            OnLbl.AutoSize = true;
            OnLbl.ImageAlign = ContentAlignment.MiddleLeft;
            OnLbl.Location = new Point(371, 79);
            OnLbl.Margin = new Padding(4, 0, 4, 0);
            OnLbl.Name = "OnLbl";
            OnLbl.Size = new Size(25, 15);
            OnLbl.TabIndex = 15;
            OnLbl.Text = "ON";
            // 
            // OnPnl
            // 
            OnPnl.BackColor = Color.Black;
            OnPnl.BorderStyle = BorderStyle.FixedSingle;
            OnPnl.Location = new Point(350, 79);
            OnPnl.Margin = new Padding(4);
            OnPnl.Name = "OnPnl";
            OnPnl.Size = new Size(14, 14);
            OnPnl.TabIndex = 7;
            // 
            // statusBtn2_0
            // 
            statusBtn2_0.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn2_0.Location = new Point(10, 74);
            statusBtn2_0.Margin = new Padding(4);
            statusBtn2_0.Name = "statusBtn2_0";
            statusBtn2_0.Size = new Size(41, 44);
            statusBtn2_0.TabIndex = 6;
            statusBtn2_0.Text = "0";
            statusBtn2_0.UseVisualStyleBackColor = true;
            statusBtn2_0.BackColorChanged += btn_BackColorChanged;
            statusBtn2_0.Click += ValueBtn_Click;
            // 
            // statusBtn2_1
            // 
            statusBtn2_1.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn2_1.Location = new Point(59, 74);
            statusBtn2_1.Margin = new Padding(4);
            statusBtn2_1.Name = "statusBtn2_1";
            statusBtn2_1.Size = new Size(41, 44);
            statusBtn2_1.TabIndex = 6;
            statusBtn2_1.Text = "1";
            statusBtn2_1.UseVisualStyleBackColor = true;
            statusBtn2_1.BackColorChanged += btn_BackColorChanged;
            statusBtn2_1.Click += ValueBtn_Click;
            // 
            // statusBtn2_2
            // 
            statusBtn2_2.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn2_2.Location = new Point(108, 74);
            statusBtn2_2.Margin = new Padding(4);
            statusBtn2_2.Name = "statusBtn2_2";
            statusBtn2_2.Size = new Size(41, 44);
            statusBtn2_2.TabIndex = 6;
            statusBtn2_2.Text = "2";
            statusBtn2_2.UseVisualStyleBackColor = true;
            statusBtn2_2.BackColorChanged += btn_BackColorChanged;
            statusBtn2_2.Click += ValueBtn_Click;
            // 
            // statusBtn2_3
            // 
            statusBtn2_3.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn2_3.Location = new Point(158, 74);
            statusBtn2_3.Margin = new Padding(4);
            statusBtn2_3.Name = "statusBtn2_3";
            statusBtn2_3.Size = new Size(41, 44);
            statusBtn2_3.TabIndex = 6;
            statusBtn2_3.Text = "3";
            statusBtn2_3.UseVisualStyleBackColor = true;
            statusBtn2_3.BackColorChanged += btn_BackColorChanged;
            statusBtn2_3.Click += ValueBtn_Click;
            // 
            // statusBtn1_4
            // 
            statusBtn1_4.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_4.Location = new Point(206, 22);
            statusBtn1_4.Margin = new Padding(4);
            statusBtn1_4.Name = "statusBtn1_4";
            statusBtn1_4.Size = new Size(41, 44);
            statusBtn1_4.TabIndex = 6;
            statusBtn1_4.Text = "4";
            statusBtn1_4.UseVisualStyleBackColor = true;
            statusBtn1_4.BackColorChanged += btn_BackColorChanged;
            statusBtn1_4.Click += ValueBtn_Click;
            // 
            // statusBtn1_5
            // 
            statusBtn1_5.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_5.Location = new Point(255, 22);
            statusBtn1_5.Margin = new Padding(4);
            statusBtn1_5.Name = "statusBtn1_5";
            statusBtn1_5.Size = new Size(41, 44);
            statusBtn1_5.TabIndex = 6;
            statusBtn1_5.Text = "5";
            statusBtn1_5.UseVisualStyleBackColor = true;
            statusBtn1_5.BackColorChanged += btn_BackColorChanged;
            statusBtn1_5.Click += ValueBtn_Click;
            // 
            // statusBtn1_6
            // 
            statusBtn1_6.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_6.Location = new Point(304, 22);
            statusBtn1_6.Margin = new Padding(4);
            statusBtn1_6.Name = "statusBtn1_6";
            statusBtn1_6.Size = new Size(41, 44);
            statusBtn1_6.TabIndex = 6;
            statusBtn1_6.Text = "6";
            statusBtn1_6.UseVisualStyleBackColor = true;
            statusBtn1_6.BackColorChanged += btn_BackColorChanged;
            statusBtn1_6.Click += ValueBtn_Click;
            // 
            // statusBtn1_7
            // 
            statusBtn1_7.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_7.Location = new Point(354, 22);
            statusBtn1_7.Margin = new Padding(4);
            statusBtn1_7.Name = "statusBtn1_7";
            statusBtn1_7.Size = new Size(41, 44);
            statusBtn1_7.TabIndex = 6;
            statusBtn1_7.Text = "7";
            statusBtn1_7.UseVisualStyleBackColor = true;
            statusBtn1_7.BackColorChanged += btn_BackColorChanged;
            statusBtn1_7.Click += ValueBtn_Click;
            // 
            // statusBtn1_0
            // 
            statusBtn1_0.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_0.Location = new Point(10, 22);
            statusBtn1_0.Margin = new Padding(4);
            statusBtn1_0.Name = "statusBtn1_0";
            statusBtn1_0.Size = new Size(41, 44);
            statusBtn1_0.TabIndex = 6;
            statusBtn1_0.Text = "0";
            statusBtn1_0.UseVisualStyleBackColor = true;
            statusBtn1_0.BackColorChanged += btn_BackColorChanged;
            statusBtn1_0.Click += ValueBtn_Click;
            // 
            // statusBtn1_1
            // 
            statusBtn1_1.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_1.Location = new Point(59, 22);
            statusBtn1_1.Margin = new Padding(4);
            statusBtn1_1.Name = "statusBtn1_1";
            statusBtn1_1.Size = new Size(41, 44);
            statusBtn1_1.TabIndex = 6;
            statusBtn1_1.Text = "1";
            statusBtn1_1.UseVisualStyleBackColor = true;
            statusBtn1_1.BackColorChanged += btn_BackColorChanged;
            statusBtn1_1.Click += ValueBtn_Click;
            // 
            // statusBtn1_2
            // 
            statusBtn1_2.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_2.Location = new Point(108, 22);
            statusBtn1_2.Margin = new Padding(4);
            statusBtn1_2.Name = "statusBtn1_2";
            statusBtn1_2.Size = new Size(41, 44);
            statusBtn1_2.TabIndex = 6;
            statusBtn1_2.Text = "2";
            statusBtn1_2.UseVisualStyleBackColor = true;
            statusBtn1_2.BackColorChanged += btn_BackColorChanged;
            statusBtn1_2.Click += ValueBtn_Click;
            // 
            // statusBtn1_3
            // 
            statusBtn1_3.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Bold);
            statusBtn1_3.Location = new Point(158, 22);
            statusBtn1_3.Margin = new Padding(4);
            statusBtn1_3.Name = "statusBtn1_3";
            statusBtn1_3.Size = new Size(41, 44);
            statusBtn1_3.TabIndex = 6;
            statusBtn1_3.Text = "3";
            statusBtn1_3.UseVisualStyleBackColor = true;
            statusBtn1_3.BackColorChanged += btn_BackColorChanged;
            statusBtn1_3.Click += ValueBtn_Click;
            // 
            // SyncStopBtn
            // 
            SyncStopBtn.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Bold);
            SyncStopBtn.Location = new Point(14, 14);
            SyncStopBtn.Margin = new Padding(4);
            SyncStopBtn.Name = "SyncStopBtn";
            SyncStopBtn.Size = new Size(286, 29);
            SyncStopBtn.TabIndex = 18;
            SyncStopBtn.Text = "Sync Stop";
            SyncStopBtn.UseVisualStyleBackColor = true;
            SyncStopBtn.Visible = false;
            SyncStopBtn.Click += SyncStopBtn_Click;
            // 
            // MainF
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(439, 514);
            Controls.Add(SyncStartBtn);
            Controls.Add(setDirectionBtn);
            Controls.Add(SyncStopBtn);
            Controls.Add(LogLb);
            Controls.Add(valueGb);
            Controls.Add(ConfigGb);
            Controls.Add(labelMsg);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MainF";
            Text = "USB-IO2(AKI)";
            FormClosing += MainF_FormClosing;
            ConfigGb.ResumeLayout(false);
            ConfigGb.PerformLayout();
            valueGb.ResumeLayout(false);
            valueGb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nekoPb).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelMsg;
        private ListBox LogLb;
        private Button SyncStartBtn;
        private GroupBox ConfigGb;
        private Button dirBtn2_0;
        private Button dirBtn2_1;
        private Button dirBtn2_2;
        private Button dirBtn2_3;
        private Button dirBtn1_4;
        private Button dirBtn1_3;
        private Button dirBtn1_7;
        private Button dirBtn1_2;
        private Button dirBtn1_5;
        private Button dirBtn1_1;
        private Button dirBtn1_6;
        private Button dirBtn1_0;
        private GroupBox valueGb;
        private CheckBox pullUpChk;
        private Button statusBtn2_0;
        private Button statusBtn2_1;
        private Button statusBtn2_2;
        private Button statusBtn2_3;
        private Button statusBtn1_4;
        private Button statusBtn1_5;
        private Button statusBtn1_6;
        private Button statusBtn1_7;
        private Button statusBtn1_0;
        private Button statusBtn1_1;
        private Button statusBtn1_2;
        private Button statusBtn1_3;
        private Button SyncStopBtn;
        private Label OffLbl;
        private Panel OffPnl;
        private Label OnLbl;
        private Panel OnPnl;
        private Panel InPnl;
        private Panel OutPnl;
        private Label InLbl;
        private Label OutLbl;
        private Button setDirectionBtn;
        private PictureBox nekoPb;
    }
}