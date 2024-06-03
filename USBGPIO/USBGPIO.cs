using System;
using System.Collections.Generic;
using System.IO;    // for Path
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBGPIO
{
    public class USBGPIO : HIDDevice, IDisposable
    {
        // Setting path
        internal static string m_settingPath;

        //=========================================================================================
        /// <summary>
        /// pin position
        /// </summary>
        [Flags]
        public enum IO : int
        {
            p0 = 0b00000001,	//0x00000001,
            p1 = 0b00000010,	//0x00000002,
            p2 = 0b00000100,	//0x00000004,
            p3 = 0b00001000,	//0x00000008,
            p4 = 0b00010000,	//0x00000010,
            p5 = 0b00100000,	//0x00000020,
            p6 = 0b01000000,	//0x00000040,
            p7 = 0b10000000		//0x00000080
        }

        //=========================================================================================
        /// <summary>
        /// Device ID
        /// </summary>
        [Flags]
        public enum DeviceId : uint
        {
            u100 = 0x100,	// USB-IO1.0
            u120 = 0x120,	// USB-IO2.0        Kmnet
            u121 = 0x121	// USB-IO2.1(AKI)	Akizuki
        }
        // Vendor IDは0x1352

        //=========================================================================================
        /// <summary>
        /// byte setting in HID Send layout(65byte): use when Command is 0x20
        /// </summary>
        public enum SendIO : int
        {
            ReportID = 0,
            Command = 1,
            Target1 = 2,    // 0x0 no output, 0x1 port1, 0x2 port2
            Data1 = 3,      // data send to the port specified by Target1
            Target2 = 4,    // 0x0 no output, 0x1 port1, 0x2 port2
            Data2 = 5,      // data output to the port specified by Target2
            Sequence = 64   // command sequence number (local rule)
        }
        //=========================================================================================
        /// <summary>
        /// byte setting in HID Send Layout (65byte): use when Command is 0xF9
        /// </summary>
        public enum SendConfig : int
        {
            ReportID = 0,
            Command = 1,
            PullUp = 3,     // pullup J2 0x0:on, 0x1:off
            Port1 = 6,      // I/O setting for port1 1:input 0:output
            Port2 = 7,      // I/O setting for port2 1:input 0:output
            Sequence = 64   // command sequence number (local rule)
        }
        //=========================================================================================
        /// <summary>
        /// byte setting in HID I/O receiving layout (65byte):use when Command is 0x20
        /// </summary>
        public enum RecvIO : int
        {
            ReportID = 0,
            Command = 1,
            Port1 = 2,      // input value of port 1
            Port2 = 3,      // input value of port 2
            Sequence = 64   // command sequence number (local rule)
        }
        //=========================================================================================
        /// <summary>
        /// byte setting in HID receive layout (65byte)
        /// </summary>
        public enum RecvConfig : int
        {
            ReportID = 0,
            Command = 1,
            PullUp = 3,
            Port1 = 6,      // J1 pin setting bit 1:input 0:output
            Port2 = 7,      // J2 pin setting bit 1:input 0:output
            Sequence = 64   // command sequence number (local rule)
        }
        //=========================================================================================
        /// <summary>
        /// command number for USB-IOへ (use only when multiple thead does not interfare)
        /// </summary>
        [Flags]
        public enum CommandFixed : byte
        {
            ReadSet = 0xF8,     // read settting
            WriteSet = 0xF9,    // write setting
            InOut = 0x20        // read value
        }

        //=========================================================================================
        /// <summary>
        /// setting value
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
        /// read current device setting from the device
        /// </summary>
        public Port GetPortConfiguration()
        {
            byte[] sendBuffer;
            byte[] readBuffer;
            Port port;

            port = new Port();
            sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)SendIO.Command] = (byte)CommandFixed.ReadSet; // 0xf8;  // system setting: read

            sendBuffer[(int)SendIO.Sequence] = 0x00; // sequennce

            Send(sendBuffer);
            readBuffer = Receive();
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
        /// write setting to the device
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="fPullUp"></param>
        public void SetPortConfiguration(IO j1, IO j2, bool fPullUp = true, int iD = 0)
        {
            byte[] sendBuffer;

            sendBuffer = new byte[OutputReportLength];

            sendBuffer[(int)SendConfig.ReportID] = 0;   // fixed value
            sendBuffer[(int)SendConfig.Command] = (byte)CommandFixed.WriteSet;// 0xf9; // System setting: write

            sendBuffer[(int)SendConfig.Sequence] = (byte)0x00; // sequence

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
        /// Digital I/O watching
        /// </summary>
        /// <param name="J1"></param>
        /// <param name="J2"></param>
        public Port SyncStatus(IO _iJ1, IO _iJ2)
        {
            Port port;
            byte[] sendBuffer;

            port = new Port();
            sendBuffer = new byte[OutputReportLength];
            sendBuffer[(int)SendIO.ReportID] = 0;   // fixed value
            sendBuffer[(int)SendIO.Command] = (byte)CommandFixed.InOut;    // 0x20;

            sendBuffer[(int)SendIO.Target1] = 0x01; // port 1
            sendBuffer[(int)SendIO.Data1] = (byte)_iJ1;

            sendBuffer[(int)SendIO.Target2] = 0x02; // port 2
            sendBuffer[(int)SendIO.Data2] = (byte)_iJ2;

            sendBuffer[(int)SendIO.Sequence] = 0x00;

            Send(sendBuffer);
            Receive();

            port.J1 = J1;
            port.J2 = J2;

            return port;
        }

        /// <summary>
        /// store data
        /// filename is replaced
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
                // UTF-8 no BOM
                using (sw = new System.IO.StreamWriter(fpath, false, new System.Text.UTF8Encoding(false)))
                {
                    // serialize to save in XML
                    serializer.Serialize(sw, _port);
                }
            }
            catch
            {
                throw new InvalidOperationException($"Could not Save to \"{fpath}\".");
            }
            return fpath;
        }

        /// <summary>
        /// read file
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
                // open file
                using (StreamReader sr = new System.IO.StreamReader(fpath, new System.Text.UTF8Encoding(false)))
                {
                    // read XML and deserialize
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

    /// <summary>
    /// helper class to check Flag
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="withFlags"></param>
        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not enum type", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' is not set 'Flags' attribute", typeof(T).FullName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }
    }
}
