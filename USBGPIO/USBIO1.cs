using System;
using System.Collections.Generic;
using System.IO;    // for Path
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBGPIO
{
    public class USBIO1 : HIDDevice, IDisposable
    {
        //=========================================================================================
        /// <summary>
        /// pin position
        /// </summary>
        [Flags]
        public enum IO
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
            u121 = 0x121    // USB-IO2.1(AKI)	Akizuki vertion
        }
        // Vendor IDは0x1352

        //=========================================================================================
        /// <summary>
        /// command send port for USBIO1.0
        /// </summary>
        [Flags]
        public enum iCmdSendTarget : byte
        {
            Port0 = 0x01,
            Port1 = 0x02
        }

        //=========================================================================================
        /// <summary>
        /// 送受信データの並び
        /// </summary>
        public enum iOByteOrder : int
        {
            ReportID = 0,
            Command = 1,    // 0x0無出力, 0x1ポート0, 0x2ポート1
            Data = 2,       // 0x0無出力, 0x1ポート1, 0x2ポート1
            byte3 = 3,
            byte4 = 4,
            byte5 = 5,
            byte6 = 6,
            Sequence = 7,   // コマンドシーケンス番号（ローカルルール）
            byte8 = 8
        }
        public enum SequenceFixed : int
        {
            wp0 = 1,    // Write Port0
            wp1 = 2,    // Write Port1
            rp0 = 3,
            rp1 = 4
        }

        public struct GotData
        {
            public byte J1;
            public byte J2;
        }
        //========================================================================
        /// <summary>
        /// 現在のデバイスの設定値をデバイスから読み込む
        /// </summary>
        public GotData GetPortConfiguration()
        {
            byte[] sendBuffer;
            byte[] readBuffer;
            GotData port;

            port = new GotData();
            sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)iOByteOrder.Command] = (byte)SequenceFixed.rp0; // 読み込み
            sendBuffer[(int)iOByteOrder.Sequence] = (byte)0x00; // シーケンス

            Send(sendBuffer);
            readBuffer = Receive();
            port.J1 = readBuffer[(int)iOByteOrder.Data];
            /*
            port.Pullup = IsPullup;
            port.J1 = (IO)Buffer[6];
            port.J2 = (IO)Buffer[7];
			*/
            port.J2 = readBuffer[(int)iOByteOrder.Data];

            return port;
        }

        //=========================================================================================
        /// <summary>
        /// デバイスに設定値を書き込む
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="fPullUp"></param>
        public void SetPortConfiguration(byte j1, byte j2, int iD = 0)
        {
            byte[] sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)iOByteOrder.ReportID] = 0;   // 固定値
            sendBuffer[(int)iOByteOrder.Command] = (byte)SequenceFixed.wp0;// 0xf9; // システム設定: 書き込み
            sendBuffer[(int)iOByteOrder.Data] = (byte)j1;
            sendBuffer[(int)iOByteOrder.Sequence] = (byte)0x00; // シーケンス
            Send(sendBuffer);

            sendBuffer[(int)iOByteOrder.ReportID] = 0;   // 固定値
            sendBuffer[(int)iOByteOrder.Command] = (byte)SequenceFixed.wp1;// 0xf9; // システム設定: 書き込み
            sendBuffer[(int)iOByteOrder.Data] = (byte)j2;
            sendBuffer[(int)iOByteOrder.Sequence] = (byte)0x00; // シーケンス
            Send(sendBuffer);

        }

        //=========================================================================================
        /// <summary>
        /// Digital I/O 監視
        /// </summary>
        /// <param name="J1"></param>
        /// <param name="J2"></param>
        public GotData SyncStatus(byte _dJ1, byte _dJ2)
        {
            GotData port;
            byte[] sendBuffer;
            byte[] recvBuffer;

            port = new GotData();
            sendBuffer = new byte[OutputReportLength];
            sendBuffer[(int)iOByteOrder.ReportID] = 0;  //固定値
            sendBuffer[(int)iOByteOrder.Command] = (byte)iCmdSendTarget.Port0;  // 0x20;
            sendBuffer[(int)iOByteOrder.Data] = _dJ1;   // データ
            sendBuffer[(int)iOByteOrder.byte3] = 0xF3;
            sendBuffer[(int)iOByteOrder.byte4] = 0xF4;
            sendBuffer[(int)iOByteOrder.byte5] = 0xF5;
            sendBuffer[(int)iOByteOrder.byte6] = 0xF6;
            sendBuffer[(int)iOByteOrder.Sequence] = 0x00;
            sendBuffer[(int)iOByteOrder.byte8] = 0xF8;
            Send(sendBuffer);
            recvBuffer = Receive();
            port.J1 = recvBuffer[(int)iOByteOrder.Data];

            sendBuffer[(int)iOByteOrder.ReportID] = 0;  //固定値
            sendBuffer[(int)iOByteOrder.Command] = (byte)iCmdSendTarget.Port1;  // 0x20;
            sendBuffer[(int)iOByteOrder.Data] = _dJ2;   // データ
            sendBuffer[(int)iOByteOrder.byte3] = 0xF3;
            sendBuffer[(int)iOByteOrder.byte4] = 0xF4;
            sendBuffer[(int)iOByteOrder.byte5] = 0xF5;
            sendBuffer[(int)iOByteOrder.byte6] = 0xF6;
            sendBuffer[(int)iOByteOrder.Sequence] = 0x00;
            sendBuffer[(int)iOByteOrder.byte8] = 0xF8;
            Send(sendBuffer);
            recvBuffer = Receive();
            port.J2 = recvBuffer[(int)iOByteOrder.Data];

            return port;
        }
        /*
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

			GotData port = new GotData();
			port.J1 = j1;
			port.J2 = j2;
			port.Pullup = isPullUp;
			return SavePortConfiguration(setpath, port, iD);
		}

		public string SavePortConfiguration(in string _setpath, in GotData _port, int _id = 0)
		{
			System.IO.StreamWriter sw = null;
			string fpath = File.Exists(_setpath) ? _setpath : GetSettingFilePath(_id, _setpath);

			try
			{
				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GotData));
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
		public GotData LoadPortConfiguration(in string _setpath, int _id = 0)
		{
			//System.IO.StreamReader sr = null;
			GotData port;

			string fpath = File.Exists(_setpath) ? _setpath : GetSettingFilePath(_id, _setpath);
			if (!File.Exists(fpath))
			{
				throw new FileNotFoundException("Load target file, \"" + _setpath + "\" was not found.");
			}

			try
			{
				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GotData));
				//読み込むファイルを開く
				using (StreamReader sr = new System.IO.StreamReader(fpath, new System.Text.UTF8Encoding(false)))
				{
					//XMLファイルから読み込み、逆シリアル化する
					port = (GotData)serializer.Deserialize(sr);
				}
			}
			catch
			{
				throw new InvalidDataException("File, \"" + fpath + "\" exists but could not load.");
			}
			return port;
		}
		*/
    }
}
