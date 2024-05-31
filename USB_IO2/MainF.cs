using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;				// for Directory/File/Path
using System.Linq;
using System.Text;
using System.Threading;			// for Mutex
using System.Threading.Tasks;
using System.Windows.Forms;

using USBGPIO;
using Notify;

namespace USB_IO2
{
    public enum portEnum : int
    {
        none = -1,
        J1 = 1,
        J2 = 2,
        maxEnum
    }

    public enum PinName : int
    {
        none = -1,
        pin0 = 0,
        pin1,
        pin2,
        pin3,
        pin4,
        pin5,
        pin6,
        pin7,
        pin8,
        pin9,
        pin10,
        pin11,
        maxEnum
    }

    public enum PortJ1 : int
    {
        none = -1,
        gpio0 = PinName.pin0,
        gpio1 = PinName.pin1,
        gpio2 = PinName.pin2,
        gpio3 = PinName.pin3,
        gpio4 = PinName.pin4,
        gpio5 = PinName.pin5,
        gpio6 = PinName.pin6,
        gpio7 = PinName.pin7,
        maxEnum
    }

    public enum PortJ2 : int
    {
        none = -1,
        gpio0 = PinName.pin8,
        gpio1 = PinName.pin9,
        gpio2 = PinName.pin10,
        gpio3 = PinName.pin11,
        maxEnum
    }
    public partial class MainF : Form
    {
        delegate void SignalTimerDelegate();

        USBDeviceMonitor usb;

        int m_id = -1;

        byte[] m_ReceivedData { get; set; }

        Button[] m_btnValueJ1;
        Button[] m_btnValueJ2;
        Button[] m_btnDirJ1;
        Button[] m_btnDirJ2;

        byte m_bC1;
        byte m_bC2;

        internal const int J1c = 0x8;   // J1 ピン数
        internal const int J2c = 0x4;   // J2 ピン数

        internal static Color OnColor = Settings.Default.OnColor;
        internal static Color OffColor = Settings.Default.OffColor;
        internal static Color InColor = Settings.Default.InColor;
        internal static Color OutColor = Settings.Default.OutColor;

        System.Windows.Forms.Timer SyncTimer;
        USBGPIO.USBGPIO.Port m_PortStatus;

        private Signal[] m_signals;
        public enum SigName : int
        {
            none = -1,
            DoJobs = 0,
            EndAck,
            o2,
            CycleStart,
            o4,
            o5,
            o6,
            o7,
            CanRead,
            JobEnd,
            i2,
            i3,
            maxEnum
        }
        internal const int CycleStart = 0;  // 出力
        internal const int DoJobs = 1;      // 出力
        internal const int EndAck = 2;      // 出力
        internal const int CanRead = 8;     // 入力
        internal const int JobEnd = 9;      // 入力

        Color m_statusBackColor;
        Color m_statusForeColor;

        //========================================================================
        /// <summary>
        /// 起動確認
        /// </summary>
        public MainF()
        {
            if (!Settings.Default.IsUpgraded)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsUpgraded = true;
                Settings.Default.Save();
            }

            int DevMax = Settings.Default.DevMax;
            int MuMin = Settings.Default.MuMin;
            int MuMax = DevMax + MuMin;

            string[] _options = System.Environment.GetCommandLineArgs();
            Mutex mutex;

            string aMutex = "PinPorter";

            foreach (string option in _options)
            {
                string[] token = option.Split('=');
                if (token[0].Trim().ToUpper() == "ID")
                {
                    int id;
                    if (Int32.TryParse(token[1], out id) && (id >= MuMin) && (id < MuMax))
                    {
                        m_id = id - MuMin;
                        aMutex += "_" + m_id.ToString();
                        mutex = new Mutex(false, aMutex);

                        if (!mutex.WaitOne(0, false))
                        {
                            MessageBox.Show("PinPorter: application has already been invoked to maximum times.");
                            Environment.Exit(0);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("PinPorter: the specified id is invalid.");
                        Environment.Exit(0);
                        return;
                    }
                }
                else
                {
                    ;
                    /*
                    MessageBox.Show("PinJuggler: unknown option has been specified.");
                    Environment.Exit(0);
                    return;
                    */
                }
            }

            if (m_id == -1) // id has not been specified
            {
                bool gotID = false;
                for (m_id = MuMin; m_id < MuMax; m_id++)
                {
                    aMutex += "_" + m_id.ToString();
                    mutex = new Mutex(false, aMutex);
                    if (!mutex.WaitOne(0, false))
                    {
                        continue;
                    }
                    else
                    {
                        gotID = true;
                        break;
                    }
                }
                if (!gotID)
                {
                    MessageBox.Show("PinPorter: has already been invoked to maximum times.");
                    Environment.Exit(0);
                    return;
                }
            }

            InitializeComponent();
            this.Text = aMutex;

            SyncTimer = new System.Windows.Forms.Timer();
            SyncTimer.Tick += new EventHandler(SignalEventProcessor);
            SyncTimer.Interval = Settings.Default.SyncTimerInterval;

            m_btnValueJ1 = new Button[] { statusBtn1_0, statusBtn1_1, statusBtn1_2, statusBtn1_3, statusBtn1_4, statusBtn1_5, statusBtn1_6, statusBtn1_7 };
            m_btnValueJ2 = new Button[] { statusBtn2_0, statusBtn2_1, statusBtn2_2, statusBtn2_3 };
            m_btnDirJ1 = new Button[] { dirBtn1_0, dirBtn1_1, dirBtn1_2, dirBtn1_3, dirBtn1_4, dirBtn1_5, dirBtn1_6, dirBtn1_7 };
            m_btnDirJ2 = new Button[] { dirBtn2_0, dirBtn2_1, dirBtn2_2, dirBtn2_3 };

            // 色の登録
            m_statusBackColor = statusBtn1_0.BackColor;
            m_statusForeColor = statusBtn1_0.ForeColor;

            // 例示パネル色
            OnPnl.BackColor = OnColor;
            OffPnl.BackColor = OffColor;
            OutPnl.BackColor = OutColor;
            InPnl.BackColor = InColor;

            usb = new USBDeviceMonitor(m_id);   // このときusb.Deviceもできる。デバイスクラスも作られる ConfigFPathもできる。

            // イベントハンドラの登録
            usb.OnDeviceArrived += new System.EventHandler(this.usb_OnDeviceArrived);   // デバイス接続検知
            usb.OnDeviceRemoved += new System.EventHandler(this.usb_OnDeviceRemoved);   // デバイス削除検知
            usb.OnSpecifiedDeviceArrived += new System.EventHandler(this.usb_OnSpecifiedDeviceArrived); // USBIOデバイス接続検知
            usb.OnSpecifiedDeviceRemoved += new System.EventHandler(this.usb_OnSpecifiedDeviceRemoved); // USBIOデバイス削除検知
            usb.OnDataSend += new System.EventHandler(this.usb_OnDataSend);
            usb.OnDataReceived += new OnDataReceivedEventHandler(this.usb_OnDataReceived);
            usb.Device.OnNotify += new OnNotifyEventHandler(this.usb_OnLogWrite);       // ログなどに使える情報を拾う

            // 現状確認
            usb.CheckDevicePresent();   // usb.Deviceが存在するか確認し、あればデバイスパスを開いてOnSpecifiedDeviceArrivedを発火。でなければRemovedを発火。
        }

        /// <summary>
        /// forecolorの設定。誤ってBackColorを指定すると無限ループ
        /// 処理はBackColorで動くので、ForeColorは見やすければ何でも良い
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BackColorChanged(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == OnColor)
            {
                ((Button)sender).ForeColor = OffColor;
            }
            else if (((Button)sender).BackColor == OffColor)
            {
                ((Button)sender).ForeColor = OnColor;
            }
            else if (((Button)sender).BackColor == InColor)
            {
                ((Button)sender).ForeColor = OnColor;
            }
            else if (((Button)sender).BackColor == OutColor)
            {
                ((Button)sender).ForeColor = OffColor;
            }
            else if (((Button)sender).BackColor == m_statusBackColor)
            {
                ((Button)sender).ForeColor = m_statusForeColor;
            }
            else if (((Button)sender).BackColor == m_statusForeColor)
            {
                ((Button)sender).ForeColor = m_statusBackColor;
            }
            this.Update();
        }

        /// <summary>
        /// ログ追記
        /// </summary>
        /// <param name="mes"></param>
        private void LogWrite(string mes)
        {
            string message = DateTime.Now.ToString("HH:mm:ss, I/O: ") + mes;
            LogLb.Items.Add(message);
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usb_OnDeviceArrived(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usb_OnDeviceArrived), new object[] { sender, e });
            }
            else
            {
                LogWrite("Info: USB device inserted.");
            }
        }


        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usb_OnDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usb_OnDeviceRemoved), new object[] { sender, e });
            }
            else
            {
                LogWrite("Info: USB device removed.");
                SyncStartBtn.Enabled = false;   // 念のため
                SyncStopBtn.Enabled = false;    // 念のため
            }
        }

        //========================================================================
        /// <summary>
        /// USB-IO2.0(AKI)が接続された時のイベントを受け取ります。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usb_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usb_OnSpecifiedDeviceArrived), new object[] { sender, e });
            }
            else
            {
                LogWrite(usb.ProductName + " is connected.");
                SyncTimer.Stop();
                SyncStartBtn.Enabled = true; SyncStartBtn.Visible = true;
                SyncStopBtn.Enabled = true; SyncStopBtn.Visible = false;
                m_PortStatus = usb.Device.GetPortConfiguration();
                UpdateStatus(m_PortStatus);
            }
        }

        //========================================================================
        /// <summary>
        /// USB-IO2.0(AKI)が取り外された時のイベントを受け取ります。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usb_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usb_OnSpecifiedDeviceRemoved), new object[] { sender, e });
            }
            else
            {
                LogWrite(usb.ProductName + " disconnected.");
                SyncTimer.Stop();
                SyncStartBtn.Enabled = false;
                SyncStopBtn.Enabled = false;
            }
        }

        //========================================================================
        /// <summary>
        /// データを受信した後にイベントとして通知を受け取ります。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void usb_OnDataReceived(object sender, USBGPIO.OnDataReceivedEventArgs args)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new OnDataReceivedEventHandler(usb_OnDataReceived), new object[] { sender, args });
                }
                catch (Exception ex)
                {
                    LogWrite(ex.ToString());
                }
            }
            else
            {
                nekoPb.Image = Properties.Resources.OutImage;
#if DEBUG
                string rec_data = string.Format("{0:hh:mm:ss} - ", DateTime.Now);
                foreach (byte myData in args.data)
                {
                    if (myData.ToString().Length == 1)
                    {
                        rec_data += "00";
                    }

                    if (myData.ToString().Length == 2)
                    {
                        rec_data += "0";
                    }

                    rec_data += myData.ToString() + " ";
                }
                System.Diagnostics.Debug.WriteLine(rec_data);
#endif
                m_ReceivedData = args.data;
                if (((usb.ProductId == (uint)USBGPIO.USBGPIO.DeviceId.u120) || (usb.ProductId == (uint)USBGPIO.USBGPIO.DeviceId.u121)) && (m_ReceivedData.Length < 64))
                    return;
                //else if (((USBGPIO.USBGPIO.DeviceId)usb.ProductId == USBGPIO.USBGPIO.DeviceId.u100) && (ReceivedData.Length < 9))
                //	return;

                switch ((byte)m_ReceivedData[(int)USBGPIO.USBGPIO.RecvConfig.Command])
                {
                case (byte)USBGPIO.USBGPIO.CommandFixed.InOut:
                    UpdateSigValue();
                    break;
                case (byte)USBGPIO.USBGPIO.CommandFixed.ReadSet:
                    UpdateStatus();
                    break;
                case (byte)USBGPIO.USBGPIO.CommandFixed.WriteSet:
                    break;
                default:
                    break;
                }
            }
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usb_OnDataSend(object sender, EventArgs e)
        {
            // データ送信後に通知を受け取ります。
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usb_OnDataSend), new object[] { sender, e });
            }
            else
            {
                nekoPb.Image = Properties.Resources.InImage;
                Update();
            }
        }

        private void usb_OnLogWrite(object sender, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new OnNotifyEventHandler(usb_OnLogWrite), new object[] { sender, e });
            }
            else
            {
                LogWrite(e.Message);
            }
        }
        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            usb.RegisterHandle(Handle);
        }

        //========================================================================
        /// <summary>
        /// ウィンドウへのメッセージを監視してデバイスの脱着があるとそのイベントを発火
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            try
            {
                usb.ParseMessages(ref m);   // ここでWindowsの脱着イベントを読み，このアプリ内のイベントハンドラを起動するイベントを起こす
                base.WndProc(ref m);        // ベースオブジェクトに下げ渡す
            }
            catch
            {
            }
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncStartBtn_Click(object sender, EventArgs e)
        {
            Connect();
        }
        private void SyncStopBtn_Click(object sender, EventArgs e)
        {
            SyncStop();
        }

        /// <summary>
        /// 設定をロードして同期タイマーをスタート
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (usb.Device.OutputReportLength < 2)
                return false;

            try
            {
                usb.Device.LoadPortConfiguration(usb.ConfigFPath, m_id);
            }
            catch
            {
                LogWrite("Error: failed to load settin file, \"" + usb.ConfigFPath + "\".");
                return false;
            }
            SyncStart();

            SyncStartBtn.Visible = false;
            SyncStopBtn.Visible = true;
            setDirectionBtn.Enabled = false;
            pullUpChk.Enabled = false;

            return true;
        }

        /// <summary>
        /// 同期開始。外部I/F用
        /// </summary>
        public void SyncStart()
        {
            SyncTimer.Start();
        }

        /// <summary>
        /// 同期停止。外部I/F用
        /// </summary>
        public void SyncStop()
        {
            SyncTimer.Stop();

            SyncStopBtn.Visible = false;
            SyncStartBtn.Visible = true;
            setDirectionBtn.Enabled = true;
            pullUpChk.Enabled = true;
        }

        //========================================================================
        /// <summary>
        /// 信号ピンの値を表示する
        /// </summary>
        private void UpdateSigValue()
        {
            USBGPIO.USBGPIO.Port port = new USBGPIO.USBGPIO.Port    // わざわざ作って
            {
                J1 = (USBGPIO.USBGPIO.IO)m_ReceivedData[(int)USBGPIO.USBGPIO.RecvIO.Port1],
                J2 = (USBGPIO.USBGPIO.IO)m_ReceivedData[(int)USBGPIO.USBGPIO.RecvIO.Port2]//,
                                                                                          //Pullup = false	// これは受信データには無い。単に構造体を埋めているだけ。
            };
            UpdateSigValue(port);
        }

        //========================================================================
        /// <summary>
        /// 信号ピンの値を表示する
        /// </summary>
        private void UpdateSigValue(USBGPIO.USBGPIO.Port port)
        {
            byte nMask;

            // pullUpChk.Checked = port.Pullup;	// これは受信データには無い

            nMask = 1;
            for (int i = 0; i < m_btnValueJ1.Length; i++, nMask <<= 1)
            {
                if (m_btnDirJ1[i].BackColor == InColor)
                {
                    if (((byte)port.J1 & nMask) > 0)
                    {
                        m_btnValueJ1[i].BackColor = OnColor;
                    }
                    else
                    {
                        m_btnValueJ1[i].BackColor = OffColor;
                    }
                }
            }
            nMask = 1;
            for (int i = 0; i < m_btnValueJ2.Length; i++, nMask <<= 1)
            {
                if (m_btnDirJ2[i].BackColor == InColor)
                {
                    if (((byte)port.J2 & nMask) > 0)
                    {
                        m_btnValueJ2[i].BackColor = OnColor;
                    }
                    else
                    {
                        m_btnValueJ2[i].BackColor = OffColor;
                    }
                }
            }
        }

        //========================================================================
        /// <summary>
        /// 信号ピンのI/O設定を表示する
        /// </summary>
        private void UpdateStatus()
        {
            m_PortStatus = new USBGPIO.USBGPIO.Port // わざわざ作って
            {
                J1 = (USBGPIO.USBGPIO.IO)m_ReceivedData[(int)USBGPIO.USBGPIO.RecvConfig.Port1],
                J2 = (USBGPIO.USBGPIO.IO)m_ReceivedData[(int)USBGPIO.USBGPIO.RecvConfig.Port2],
                Pullup = (m_ReceivedData[(int)USBGPIO.USBGPIO.RecvConfig.PullUp] == 0)
            };
            UpdateStatus(m_PortStatus); // 表示させる
        }

        /// <summary>
        /// 信号ピンのI/O設定を表示する
        /// </summary>
        /// <param name="port"></param>
        private void UpdateStatus(USBGPIO.USBGPIO.Port port)
        {
            byte nMask;

            pullUpChk.Checked = port.Pullup;

            nMask = 1;
            for (int i = 0; i < m_btnDirJ1.Length; i++, nMask <<= 1)
            {
                if (((byte)port.J1 & nMask) > 0)
                {
                    m_btnDirJ1[i].BackColor = InColor;  // 入力が赤
                                                        //m_btnStatusJ1[i].Enabled = false;      // 入力なので出力操作できないようにする
                }
                else
                {
                    m_btnDirJ1[i].BackColor = OutColor;
                    //m_btnStatusJ1[i].Enabled = true;
                }
                /*if (!m_btnStatusJ1[i].Enabled)
                {
                    m_btnStatusJ1[i].BackColor = m_statusBackColor;
                }*/
            }
            nMask = 1;
            for (int i = 0; i < m_btnDirJ2.Length; i++, nMask <<= 1)
            {
                if (((byte)port.J2 & nMask) > 0)
                {
                    m_btnDirJ2[i].BackColor = InColor;  // 入力が赤
                                                        //m_btnStatusJ2[i].Enabled = false;	// 入力なので出力操作できないようにする
                }
                else
                {
                    m_btnDirJ2[i].BackColor = OutColor;
                    //m_btnStatusJ2[i].Enabled = true;
                }
                /*if (!m_btnStatusJ2[i].Enabled)
                {
                    m_btnStatusJ2[i].BackColor = m_statusBackColor;
                }*/
            }
            this.Update();
        }

        //========================================================================
        /// <summary>
        /// バックがOnColorの奴だけオンにして送信データbC1, bC2を作る
        /// 色で情報伝達なんて、GUIと操作の分離がしにくい
        /// </summary>
        private void UpdateCommand()
        {
            byte bMask;

            bMask = 0;
            for (int i = 0; i < m_btnValueJ1.Length; i++)
            {
                if ((m_btnDirJ1[i].BackColor == OutColor) && (m_btnValueJ1[i].BackColor == OnColor))
                    bMask |= (byte)(1 << i);
            }
            m_bC1 = bMask;

            bMask = 0;
            for (int i = 0; i < m_btnValueJ2.Length; i++)
            {
                if ((m_btnDirJ2[i].BackColor == OutColor) && (m_btnValueJ2[i].BackColor == OnColor))
                    bMask |= (byte)(1 << i);
            }
            m_bC2 = bMask;
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirBtn_Click(object sender, EventArgs e)
        {
            if (setDirectionBtn.Enabled)
            {
                if (((Button)sender).BackColor == InColor)
                {
                    ((Button)sender).BackColor = OutColor;
                }
                else
                {
                    ((Button)sender).BackColor = InColor;
                }
                UpdateCommand();
#if DEBUG
                System.Diagnostics.Debug.WriteLine(string.Format("Command: C1 : {0:x} / C2 : {1:x}", m_bC1, m_bC2));
#endif
            }
        }

        /// <summary>
        /// Enabledの時だけ押せる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueBtn_Click(object sender, EventArgs e)
        {
            int j1 = Array.IndexOf(m_btnValueJ1, (Button)sender);
            int j2 = Array.IndexOf(m_btnValueJ2, (Button)sender);
            if (j1 >= 0)  // J1有効
            {
                if (m_btnDirJ1[j1].BackColor == OutColor)
                {
                    if (m_btnValueJ1[j1].BackColor != OnColor)
                    {
                        m_btnValueJ1[j1].BackColor = OnColor;
                    }
                    else
                    {
                        m_btnValueJ1[j1].BackColor = OffColor;
                    }
                }
            }
            else if (j2 >= 0) // J2有効
            {
                if (m_btnDirJ2[j2].BackColor == OutColor)
                {
                    if (m_btnValueJ2[j2].BackColor != OnColor)
                    {
                        m_btnValueJ2[j2].BackColor = OnColor;
                    }
                    else
                    {
                        m_btnValueJ2[j2].BackColor = OffColor;
                    }
                }
            }
            /*
            if (((Button)sender).BackColor != OnColor)
            {
                ((Button)sender).BackColor = OnColor;
            }
            else
            {
                ((Button)sender).BackColor = OffColor;
            }
            */
            UpdateCommand();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("Command: C1 : {0:x} / C2 : {1:x}", m_bC1, m_bC2));
#endif
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        private void SyncSignalStatus()
        {
            USBGPIO.USBGPIO.Port port;

            // port = (usb.Device as USBGPIO.USBGPIO).SyncStatus((USBGPIO.USBGPIO.IO)bC1, (USBGPIO.USBGPIO.IO)bC2);
            port = usb.Device.SyncStatus((USBGPIO.USBGPIO.IO)m_bC1, (USBGPIO.USBGPIO.IO)m_bC2);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("port J1: " + port.J1.ToString());
            System.Diagnostics.Debug.WriteLine("port J2: " + port.J2.ToString());
#endif
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        void IOInUIThread()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("DoInUIThread");
#endif
            SyncSignalStatus();
            SetGUI();
        }

        private void SetGUI()
        {
            ;// this.Update();
        }


        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void SignalEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            SignalTimerDelegate td = new SignalTimerDelegate(IOInUIThread);
            BeginInvoke(td);
        }

        //========================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainF_FormClosing(object sender, FormClosingEventArgs e)
        {
            SyncTimer.Stop();
            usb.UnregisterHandle();
            usb.Dispose();
        }

        private void MainF_Shown(object sender, EventArgs e)
        {
            DefineSignals();
        }

        /// <summary>
        /// 信号の定義 ここは個々に定義するところ。もしくはまとめてシリアライズしたものを読む。
        /// 尚ここでは、ハードとしてのPinJugglerのピン属性は、I/Oコントロールタブで設定済みと想定
        /// し、デバイスの挿抜を伴う電気的な設定は行わない
        /// </summary>
        private void DefineSignals()
        {
            m_signals = new Signal[(int)SigName.maxEnum];
            for (int i = 0; i < m_signals.Length; i++)
            {
                m_signals[i].Port = (i < (int)PortJ1.maxEnum) ? portEnum.J1 : portEnum.J2;
                m_signals[i].Pin = (PinName)((i < (int)PortJ1.maxEnum) ? i : i - (int)PortJ1.maxEnum);  // いまはたまたま通し番号
                m_signals[i].IsInput = (i < m_btnDirJ1.Length) ? false : true;
                m_signals[i].DevId = 0;
            }
        }

        /// <summary>
        /// ポート方向設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setDirectionBtn_Click(object sender, EventArgs e)
        {
            USBGPIO.USBGPIO.Port port = new USBGPIO.USBGPIO.Port();

            port.J1 = 0;
            port.J2 = 0;
            port.Pullup = pullUpChk.Checked;

            port.J1 |= (dirBtn1_0.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p0 : 0;
            port.J1 |= (dirBtn1_1.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p1 : 0;
            port.J1 |= (dirBtn1_2.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p2 : 0;
            port.J1 |= (dirBtn1_3.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p3 : 0;
            port.J1 |= (dirBtn1_4.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p4 : 0;
            port.J1 |= (dirBtn1_5.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p5 : 0;
            port.J1 |= (dirBtn1_6.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p6 : 0;
            port.J1 |= (dirBtn1_7.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p7 : 0;

            port.J2 |= (dirBtn2_0.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p0 : 0;
            port.J2 |= (dirBtn2_1.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p1 : 0;
            port.J2 |= (dirBtn2_2.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p2 : 0;
            port.J2 |= (dirBtn2_3.BackColor == InColor) ? USBGPIO.USBGPIO.IO.p3 : 0;

            SyncTimer.Start();
            usb.Device.SetPortConfiguration(port.J1, port.J2, port.Pullup, m_id);
            Thread.Sleep(SyncTimer.Interval);
            MessageBox.Show("I/O pin configuration mode change has been changed.\r\nplug out->in the USBIO device.");
            SyncTimer.Stop();
        }

        /// <summary>
        /// 出力信号に値をセットする。SyncTimerが開始状態であること。
        /// </summary>
        /// <param name="_port"></param>
        /// <param name="_pin"></param>
        /// <param name="_isInput">当該ピンが入力信号の時true</param>
        /// <param name="_val"></param>
        /// <param name="devId">未使用</param>
        /// <returns>false: 設定失敗。指定されたのが領域外か入力信号だった。</returns>
        private bool SetSignal(portEnum _port, PinName _pin, bool _isInput, bool _val, int devId = 0)
        {
            int pin = (int)_pin;
            if (_port == portEnum.J1)
            {
                if (((int)PortJ1.none < pin) && (pin < (int)PortJ1.maxEnum))  // 領域内で、
                {
                    if (!_isInput)  // 出力信号
                    {
                        m_btnDirJ1[pin].BackColor = _val ? OnColor : OffColor;
                        return true;
                    }
                    /*else if ((m_btnCmdJ1[pin].BackColor == InColor) && PseudoInputChk.Checked)    // 入力だけどエミュレート有効
                    {
                        m_btnCmdJ1[pin].BackColor = m_btnStatusJ1[pin].ForeColor = val ? OnColor : OffColor;
                        m_btnCmdJ1[pin].ForeColor = m_btnStatusJ1[pin].BackColor = val ? OffColor : OnColor;	// ここで変える
                        return true;
                    }*/
                }
                return false;
            }
            else if (_port == portEnum.J2)
            {
                if (((int)PortJ2.none < pin) && (pin < (int)PortJ2.maxEnum))
                {
                    if (!_isInput)  // 出力信号
                    {
                        m_btnDirJ2[pin].BackColor = _val ? OnColor : OffColor;
                        return true;
                    }
                    /*else if ((m_btnCmdJ2[pin].BackColor == InColor) && PseudoInputChk.Checked)    // 入力だけどエミュレート有効
                    {
                        m_btnCmdJ2[pin].BackColor = m_btnStatusJ2[pin].ForeColor = val ? OnColor : OffColor;
                        m_btnCmdJ2[pin].ForeColor = m_btnStatusJ2[pin].BackColor = val ? OffColor : OnColor;  // ここで変える
                        return true;
                    }*/
                    return false;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        private bool SetSignal(in Signal _sig, bool _val)
        {
            return SetSignal(_sig.Port, _sig.Pin, _sig.IsInput, _val, _sig.DevId);
        }

        /// <summary>
        /// 入力信号の値をとる。値は戻り値ではなくて、引数のref valであることに注意。SyncTimerが開始状態であること。
        /// </summary>
        /// <param name="_portNo"></param>
        /// <param name="_pin"></param>
        /// <param name="val"></param>
        /// <param name="_devId"></param>
        /// <returns>true if operation complete</returns>
        private bool GetSignal(portEnum _portNo, PinName _pin, ref bool val, int _devId = 0)
        {
            int pin = (int)_pin;
            if (_portNo == portEnum.J1)
            {
                if (!(((int)PortJ1.none < pin) && (pin < (int)PortJ1.maxEnum)))  // 領域外か。出力信号でも値は取れるので、それは排除しない
                {
                    return false;
                }
                val = (m_btnValueJ1[pin].BackColor == OnColor);
                return true;
            }
            else if (_portNo == portEnum.J2)
            {
                if (!(((int)PortJ2.none < pin) && (pin < (int)PortJ2.maxEnum)))
                {
                    return false;
                }
                val = (m_btnValueJ2[pin].BackColor == OnColor);
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool GetSignal(in Signal _sig, ref bool _val)
        {
            return GetSignal(_sig.Port, _sig.Pin, ref _val, _sig.DevId);
        }

        private bool On(Signal _sig)
        {
            return SetSignal(_sig.Port, _sig.Pin, _sig.IsInput, true, _sig.DevId);
        }
        private bool Off(Signal _sig)
        {
            return SetSignal(_sig.Port, _sig.Pin, _sig.IsInput, false, _sig.DevId);
        }
        private bool Is(Signal _sig, bool _default = false)
        {
            bool result = _default;
            if (GetSignal(_sig.Port, _sig.Pin, ref result, _sig.DevId))
            {
                return result;
            }
            else
            {
                LogWrite("Error: in Signal.Is()");
                return _default;
            }
        }
        /// <summary>
        /// 信号の、物理ピンへの割り当て
        /// </summary>
        [Serializable]
        private class Signal
        {
            public portEnum Port { get; set; }
            public PinName Pin { get; set; }
            public int DevId { get; set; }
            public bool IsInput { get; set; }

            USBDeviceMonitor usb;

            public Signal(ref USBDeviceMonitor _usb)
            {
                usb = _usb;
            }
            public Signal(ref USBDeviceMonitor _usb, portEnum _port = portEnum.none, PinName _pin = PinName.none, bool _isInput = false, int _devId = 0) : this(ref _usb)
            {
                Port = _port;
                Pin = _pin;
                IsInput = _isInput;
                DevId = _devId;
            }
        }
    }
}
