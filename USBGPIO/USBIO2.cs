using System;
using System.Collections.Generic;
using System.IO;    // for Path
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBGPIO
{
    public class USBIO2 : HIDDevice, IDisposable
    {
        // setting path
        internal static string m_settingPath;

        //=========================================================================================
        /// <summary>
        /// pin position
        /// </summary>
        [Flags]
        public enum IO : int
        {
            p0 = 0b00000001,    //0x00000001,
            p1 = 0b00000010,    //0x00000002,
            p2 = 0b00000100,    //0x00000004,
            p3 = 0b00001000,    //0x00000008,
            p4 = 0b00010000,    //0x00000010,
            p5 = 0b00100000,    //0x00000020,
            p6 = 0b01000000,    //0x00000040,
            p7 = 0b10000000     //0x00000080
        }

        //=========================================================================================
        /// <summary>
        /// device type of USBIO
        /// </summary>
        [Flags]
        public enum DevType : int
        {
            u100 = 0x100,   // USB-IO1.0
            u120 = 0x120,   // USB-IO2.0        Kmnet version
            u121 = 0x121    // USB-IO2.1(AKI)	Akizuki version
        }
        // Vendor ID is 0x1352

        //=========================================================================================
        /// <summary>
        /// HID送信レイアウト(65byte)における、各バイトの指定:Commandが0x20の時に使う
        /// </summary>
        public enum SendIO : int
        {
            ReportID = 0,
            Command = 1,
            Target1 = 2,    // 0x0無出力, 0x1ポート1, 0x2ポート2
            Data1 = 3,      // Target1で指定したポートに出力するデータ
            Target2 = 4,    // 0x0無出力, 0x1ポート1, 0x2ポート2
            Data2 = 5,      // Target2で指定したポートに出力するデータ
            Sequence = 64   // コマンドシーケンス番号（ローカルルール）
        }
        //=========================================================================================
        /// <summary>
        /// HID送信レイアウト(65byte)における、各バイトの指定:Commandが0xF9の時に使う
        /// </summary>
        public enum SendConfig : int
        {
            ReportID = 0,
            Command = 1,
            PullUp = 3,     // J2のプルアップ 0x0:有効, 0x1:無効
            Port1 = 6,      // ポート1 I/O設定 bitが1なら入力ピン
            Port2 = 7,      // ポート2 I/O設定 bitが1なら入力ピン
            Sequence = 64   // コマンドシーケンス番号（ローカルルール）
        }
        //=========================================================================================
        /// <summary>
        /// HID I/O受信レイアウト(65byte)における、各バイトの指定
        /// </summary>
        public enum RecvIO : int
        {
            ReportID = 0,
            Command = 1,
            Port1 = 2,      // ポート1入力値
            Port2 = 3,      // ポート2入力値
            Sequence = 64   // コマンドシーケンス番号（ローカルルール）
        }
        //=========================================================================================
        /// <summary>
        /// HID 設定受信レイアウト(65byte)における、各バイトの指定
        /// </summary>
        public enum RecvConfig : int
        {
            ReportID = 0,
            Command = 1,
            PullUp = 3,
            Port1 = 6,      // J1入力ピン設定ビット 1:入力, 0:出力
            Port2 = 7,      // J2入力ピン設定ビット 1:入力, 0:出力
            Sequence = 64   // コマンドシーケンス番号（ローカルルール）
        }
        //=========================================================================================
        /// <summary>
        /// USB-IOへの命令番号（交錯せずに使える場合のみ使用）
        /// </summary>
        [Flags]
        public enum SequenceFixed : byte
        {
            ReadSet = 0xF8,
            WriteSet = 0xF9,
            InOut = 0x20
        }

        //=========================================================================================
        /// <summary>
        /// 設定値
        /// </summary>
        [Serializable]
        public class Port
        {
            public bool Pullup;
            public IO J1;
            public IO J2;
        }

        //=========================================================================================
        /// <summary>
        /// J2 pin pullup when mode input. 0:pullup enable, 1:disable
        /// </summary>
        public bool IsPullup
        {
            get
            {
                if ((Buffer[3] & (byte)0x01) > 0)
                    return false;
                else
                    return true;
            }
        }

        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        public IO J1
        {
            get
            {
                return (IO)Buffer[3];
            }
        }

        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        public IO J2
        {
            get
            {
                return (IO)Buffer[2];
            }
        }

        //========================================================================
        /// <summary>
        /// 現在のデバイスの設定値をデバイスから読み込む
        /// </summary>
        public Port GetPortConfiguration()
        {
            byte[] sendBuffer;
            byte[] readBuffer;
            Port port;

            port = new Port();
            sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)SendIO.Command] = (byte)SequenceFixed.ReadSet; // 0xf8;  // システム設定: 読み込み

            sendBuffer[(int)SendIO.Sequence] = 0x00; // シーケンス

            Send(sendBuffer);
            readBuffer = Receive(); // HIDはsend後、待ち時間の指定なしで値が得られるのか！？
            /*
            port.Pullup = IsPullup;
            port.J1 = (IO)Buffer[6];
            port.J2 = (IO)Buffer[7];
			*/
            port.Pullup = (readBuffer[(int)RecvConfig.PullUp] == 0x0);
            port.J1 = (IO)readBuffer[(int)RecvConfig.Port1];
            port.J2 = (IO)readBuffer[(int)RecvConfig.Port2];

            return port;
        }

        //=========================================================================================
        /// <summary>
        /// デバイスに設定値を書き込む
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="fPullUp"></param>
        public void SetPortConfiguration(IO j1, IO j2, bool fPullUp = true, int iD = 0)
        {
            byte[] sendBuffer;

            sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)SendConfig.ReportID] = 0;   // 固定値
            sendBuffer[(int)SendConfig.Command] = (byte)SequenceFixed.WriteSet;// 0xf9; // システム設定: 書き込み

            sendBuffer[(int)SendConfig.Sequence] = (byte)0x00; // シーケンス

            sendBuffer[(int)SendConfig.PullUp] = (byte)(fPullUp ? 0x00 : 0x01);

            //SendBuffer[4] = (byte)0x00;
            //SendBuffer[5] = (byte)0x00;
            sendBuffer[(int)SendConfig.Port1] = (byte)j1;
            sendBuffer[(int)SendConfig.Port2] = (byte)j2;

            Send(sendBuffer);
            Port aPort = new Port();
            aPort.J1 = j1;
            aPort.J2 = j2;
            aPort.Pullup = fPullUp;
            m_settingPath = SavePortConfiguration(m_settingPath, aPort, iD);
        }

        //=========================================================================================
        /// <summary>
        /// Digital I/O 監視
        /// </summary>
        /// <param name="J1"></param>
        /// <param name="J2"></param>
        public Port SyncStatus(IO _iJ1, IO _iJ2)
        {
            Port port;
            byte[] sendBuffer;

            port = new Port();
            sendBuffer = new byte[OutputReportLength];
            sendBuffer[(int)SendIO.ReportID] = 0;   //固定値
            sendBuffer[(int)SendIO.Command] = (byte)SequenceFixed.InOut;    // 0x20;

            sendBuffer[(int)SendIO.Target1] = 0x01; // ポート1
            sendBuffer[(int)SendIO.Data1] = (byte)_iJ1;

            sendBuffer[(int)SendIO.Target2] = 0x02; // ポート2
            sendBuffer[(int)SendIO.Data2] = (byte)_iJ2;

            sendBuffer[(int)SendIO.Sequence] = 0x00;

            Send(sendBuffer);
            Receive();

            port.J1 = J1;
            port.J2 = J2;

            return port;
        }

        /// <summary>
        /// データ保存
        /// ファイル名は強制的に置き換えられる
        /// </summary>
        /// <param name="setpath"></param>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="isPullUp"></param>
        /// <param name="iD"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string SavePortConfiguration(in string setpath, IO j1, IO j2, bool isPullUp, int iD = 0)
        {
            SetPortConfiguration(j1, j2, isPullUp, iD);

            Port port = new Port();
            port.J1 = j1;
            port.J2 = j2;
            port.Pullup = isPullUp;
            return SavePortConfiguration(setpath, port, iD);
        }

        public string SavePortConfiguration(in string _setpath, in Port _port, int _id = 0)
        {
            System.IO.StreamWriter sw = null;
            string fpath = File.Exists(_setpath) ? _setpath : GetSettingFilePath(_id, _setpath);

            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Port));
                // UTF-8 BOM無し
                using (sw = new System.IO.StreamWriter(fpath, false, new System.Text.UTF8Encoding(false)))
                {
                    //シリアル化し、XMLファイルに保存する
                    serializer.Serialize(sw, _port);
                }
            }
            catch
            {
                throw new InvalidOperationException("Could not Save to \"" + fpath + "\".");
            }
            return fpath;
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="_setpath"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public Port LoadPortConfiguration(in string _setpath, int _id = 0)
        {
            //System.IO.StreamReader sr = null;
            Port port;

            string fpath = File.Exists(_setpath) ? _setpath : GetSettingFilePath(_id, _setpath);
            if (!File.Exists(fpath))
            {
                throw new FileNotFoundException("Load target file, \"" + _setpath + "\" was not found.");
            }

            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Port));
                //読み込むファイルを開く
                using (StreamReader sr = new System.IO.StreamReader(fpath, new System.Text.UTF8Encoding(false)))
                {
                    //XMLファイルから読み込み、逆シリアル化する
                    port = (Port)serializer.Deserialize(sr);
                }
            }
            catch
            {
                throw new InvalidDataException("File, \"" + fpath + "\" exists but could not load.");
            }
            return port;
        }
    }
}
