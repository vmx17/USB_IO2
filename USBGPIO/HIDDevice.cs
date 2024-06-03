using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Notify;

namespace USBGPIO
{
    public delegate void OnDataReceivedEventHandler(object sender, OnDataReceivedEventArgs args);
    public delegate void OnDataSendEventHandler(object sender, OnDataSendEventArgs args);

    //=========================================================================================
    /// <summary>
    /// 
    /// </summary>
    public class OnDataReceivedEventArgs : EventArgs
    {
        public readonly byte[] data;

        public OnDataReceivedEventArgs(byte[] data)
        {
            this.data = data;
        }
    }

    //=========================================================================================
    /// <summary>
    ///
    /// </summary>
    public class OnDataSendEventArgs : EventArgs
    {
        public readonly byte[] data;

        public OnDataSendEventArgs(byte[] data)
        {
            this.data = data;
        }
    }

    //=========================================================================================
    /// <summary>
    /// HIDで発生した例外処理用
    /// </summary>
    public class HIDDeviceException : ApplicationException
    {
        public HIDDeviceException(string strMessage) : base(strMessage) { }

        public static HIDDeviceException GenerateWithWinError(string strMessage)
        {
            return new HIDDeviceException(string.Format("Message:{0} Windows Error[:{1:X8}", strMessage, Marshal.GetLastWin32Error()));
        }

        public static HIDDeviceException GenerateError(string strMessage)
        {
            return new HIDDeviceException(string.Format("Message:{0}", strMessage));
        }
    }

    //=========================================================================================
    /// <summary>
    /// HIDの抽象化クラス
    /// </summary>
    public class HIDDevice : Win32Stub, IDisposable
    {
        [Description("an event to transfer lo information.")]
        [Category("MesssageEvent")]
        [DisplayName("OnNotify")]
        public event OnNotifyEventHandler OnNotify;
        public event OnDataReceivedEventHandler DataRecieved;
        public event OnDataSendEventHandler DataSend;

        private int m_nInputReportLength;
        private int m_nOutputReportLength;
        private SafeFileHandle m_hHandle;
        private byte[] m_ReceiveBuffer;

        //=========================================================================================
        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //=========================================================================================
        /// <summary>
        /// クラス破棄の前の処理。常に最後に呼ばれる。
        /// </summary>
        /// <param name="fDisposing">TRUEの場合破棄</param>
        protected virtual void Dispose(bool fDisposing)
        {
            try
            {
                if (fDisposing)
                {
                    // ハンドルをクローズする前に必要な処理があれば、実行する。
                }
                if (m_hHandle != null)
                {
                    // Win32 API経由で作成した全てのオブジェクトをクローズする。
                    CloseHandle(m_hHandle);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //=========================================================================================
        /// <summary>
        /// HIDを初期化、オープンする。
        /// </summary>
        /// <param name="strPath">デバイスへのパス</param>
        public bool Open(string strPath)
        {
            int nLastError;
            bool fResult;


            Close();

            fResult = false;

            // パイプアクセスの為のハンドルを作成する（オープン）
            m_hHandle = CreateFile(strPath,
                                        GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
                                        IntPtr.Zero,
                                        OPEN_EXISTING,
                                        0,
                                        IntPtr.Zero);
            nLastError = Marshal.GetLastWin32Error();
            if (!m_hHandle.IsInvalid || m_hHandle == null)
            {
                IntPtr lpData;

                // Windowsが管理するバッファーへ、デバイスの情報を読み込み取得する。
                if (HidD_GetPreparsedData(m_hHandle, out lpData))
                {
                    try
                    {
                        HIDP_CAPS objCaps;

                        // デバイスの情報を取得
                        HidP_GetCaps(lpData, out objCaps);

                        // デバイスがサポートするReportの読出し、書き出しのバッファーサイズを取得します。
                        // USBデバイスでは、このサイズが固定です。
                        m_nInputReportLength = objCaps.InputReportByteLength;
                        m_nOutputReportLength = objCaps.OutputReportByteLength;

                        m_ReceiveBuffer = new byte[m_nInputReportLength];
                    }
                    catch
                    {
                        throw HIDDeviceException.GenerateWithWinError("HIDデバイスの詳細情報取得に失敗しました。");
                    }
                    finally
                    {
                        // HidD_GetPreparsedData() で取得したデータ領域を開放します。
                        // マネージドコードと異なり、明示的に開放しないとメモリーリークとなります。
                        HidD_FreePreparsedData(ref lpData);

                        fResult = true;
                    }
                }
                else
                {
                    throw HIDDeviceException.GenerateWithWinError("HidD_GetPreparsedData()の実行に失敗しました。");
                }
            }
            else
            {
                m_hHandle = null;
                throw HIDDeviceException.GenerateWithWinError("デバイスとのパイプ作成に失敗しました。");
            }

            return fResult;
        }

        //=========================================================================================
        /// <summary>
        /// デバイスへのパイプをクローズします。
        /// </summary>
        public void Close()
        {
            if (m_hHandle != null && !m_hHandle.IsInvalid)
            {
                CloseHandle(m_hHandle);
                m_hHandle = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setpath"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal string GetSettingFilePath(int id, string _path = "")
        {
            if (!Directory.Exists(_path))
            {
                _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location));
                // C:\\ProgramData\USBGPIO
                if (!Directory.Exists(_path))
                {
                    try
                    {
                        Directory.CreateDirectory(_path);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
            return Path.Combine(_path, Settings.Default.StoreFileBody + "_" + id.ToString() + ".xml");
        }

        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        protected byte[] Buffer
        {
            get
            {
                return m_ReceiveBuffer;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Write an output report to the device.
        /// </summary>
        /// <param name="oOutRep">Output report to write</param>
        public void Send(byte[] sendData)
        {
            try
            {
                uint nWritten;

                nWritten = 0;
                WriteFile(m_hHandle, sendData, (uint)sendData.Length, ref nWritten, IntPtr.Zero);
                if (DataSend != null)
                {
                    DataSend(this, new OnDataSendEventArgs(sendData));
                }
            }
            catch (IOException e)
            {
                throw new HIDDeviceException("IOException. HID removed. " + e.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        //=========================================================================================
        /// <summary>
        /// デバイスからのデータを取得する。
        /// </summary>
        /// <returns></returns>
        public byte[] Receive()
        {
            bool fResult;
            uint nRead;

            nRead = 0;
            fResult = ReadFile(m_hHandle, m_ReceiveBuffer, (UInt32)OutputReportLength, ref nRead, IntPtr.Zero);
            try
            {
                if (fResult)
                {

                    DataRecieved?.Invoke(this, new OnDataReceivedEventArgs(m_ReceiveBuffer));

#if DEBUG
                    string str = "";
                    for (int nIndex = 0; nIndex < 8; nIndex++)
                    {
                        str += string.Format("{0}:{1:x}, ", nIndex, m_ReceiveBuffer[nIndex]);
                    }
                    System.Diagnostics.Debug.WriteLine(str);
#endif
                    return m_ReceiveBuffer;
                }
                else
                {
                    return null;
                }
            }
            catch (IOException e)
            {
                throw new HIDDeviceException("Error: failed to get value from device\nMsg: " + e.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        /// <param name="hInfoSet"></param>
        /// <param name="oDeviceInterfaceData"></param>
        /// <returns></returns>
        private static string GetDevicePath(IntPtr hInfoSet, ref SP_DEVICE_INTERFACE_DATA oDeviceInterfaceData)
        {
            uint nRequiredSize;

            nRequiredSize = 0;

            // デバイスインターフェースの詳細情報を取得します。
            if (!SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oDeviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero))
            {
                SP_DEVICE_INTERFACE_DETAIL_DATA oDeviceInterfaceDetail;

                oDeviceInterfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                //oDeviceInterfaceDetail.cbSize = 5;
                //if (IntPtr.Size == 8)
                if (System.Environment.Is64BitOperatingSystem)
                    oDeviceInterfaceDetail.cbSize = 8;
                else
                    oDeviceInterfaceDetail.cbSize = 5;

                /*oDeviceInterfaceDetail.cbSize = Marshal.SizeOf(oDeviceInterfaceDetail);
				if (SetupDiGetDeviceInterfaceDetail(
						hInfoSet,
						ref oDeviceInterfaceData,
						ref oDeviceInterfaceDetail,
						nRequiredSize,
						ref nRequiredSize,
						IntPtr.Zero))
				{
					return oDeviceInterfaceDetail.DevicePath;
				}*/
                nRequiredSize = 0;

                // Error 0
                if (!SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oDeviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero))
                    // Error 122 - ERROR_INSUFFICIENT_BUFFER (not a problem, just used to set nRequiredSize)
                    if (SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oDeviceInterfaceData, ref oDeviceInterfaceDetail, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
                        return oDeviceInterfaceDetail.DevicePath;
                // Error 1784 - ERROR_INVALID_USER_BUFFER (unless size=5 on 32bit, size=8 on 64bit)
            }
            return null;
        }

        //=========================================================================================
        /// <summary>
        /// ベンダーID、プロダクトIDからデバイスを検索します。
        /// </summary>
        /// <param name="nVid"></param>
        /// <param name="nPid"></param>
        /// <returns></returns>
        public string FindDevice(uint nVid, uint nPid)
        {
            string strPath;
            string strSearch;
            Guid GuidHid;
            IntPtr hInformationSet;

            strPath = string.Empty;

            // ベンダーID、プロダクトIDからデバイス・パスを作成します。
            strSearch = string.Format("vid_{0:x4}&pid_{1:x4}", nVid, nPid);
            GuidHid = HIDGuid;

            // デバイスに接続されているHIDデバイスの一覧を取得します。
            hInformationSet = SetupDiGetClassDevs(ref GuidHid, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);
            try
            {
                SP_DEVICE_INTERFACE_DATA oDeviceInterfaceData;
                int nIndex;

                // PSP_DEVICE_INTERFACE_DATA 構造体分のメモリーを確保します。
                oDeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                oDeviceInterfaceData.cbSize = Marshal.SizeOf(oDeviceInterfaceData); // cbSize����O�Ɋ��蓖��
                                                                                    //oDeviceInterfaceData.cbSize = (uint)Marshal.SizeOf(oDeviceInterfaceData); // cbSize����O�Ɋ��蓖��

                nIndex = 0;
                // 順に登録されているデバイスを確認してきます。
                while (SetupDiEnumDeviceInterfaces(hInformationSet, 0, ref GuidHid, (uint)nIndex, ref oDeviceInterfaceData))
                {
                    string strDevicePath;

                    // デバイスのパス情報を取得します。
                    strDevicePath = GetDevicePath(hInformationSet, ref oDeviceInterfaceData);
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", nIndex, strDevicePath));
                    // 取得したデバイスパスが目的としているベンダーID、プロダクトIDに該当するものか
                    // 確認します。
                    if (strDevicePath.IndexOf(strSearch) >= 0)
                    {
                        return strDevicePath;
                    }
                    nIndex++;
                }
            }
            catch (Exception e)
            {
                throw HIDDeviceException.GenerateError(e.ToString());
            }
            finally
            {
                // Win32 APIを通して作成されているハンドル
                SetupDiDestroyDeviceInfoList(hInformationSet);
            }
            return null;
        }

        //=========================================================================================
        /// <summary>
        /// デバイスが取り外された際のイベント
        /// </summary>
        public event EventHandler OnDeviceRemoved;

        //=========================================================================================
        /// <summary>
        /// Reportの出力バッファ長の取得
        /// </summary>
        public int OutputReportLength
        {
            get
            {
                return m_nOutputReportLength;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Reportの入力バッファ長の取得
        /// </summary>
        public int InputReportLength
        {
            get
            {
                return m_nInputReportLength;
            }
        }

        //=========================================================================================
        /// <summary>
        /// デバイス変更メッセージを受け取るWindowの登録
        /// </summary>
        /// <param name="hWnd">Window メッセージを受け取るWindow Handle</param>
        /// <param name="GuidClass">対象のデバイスクラスのGUID</param>
        /// <returns>登録を削除する際に使用するＩＤ</returns>
        public static IntPtr RegisterWindowToReceiveDeviceNotification(IntPtr hWnd, Guid GuidClass)
        {
            DEV_BROADCAST_HDR objDeviceBroadcastInterface;

            objDeviceBroadcastInterface = new DEV_BROADCAST_HDR();
            objDeviceBroadcastInterface.cbSize = Marshal.SizeOf(objDeviceBroadcastInterface);
            objDeviceBroadcastInterface.ClassGuid = GuidClass;
            objDeviceBroadcastInterface.DeviceType = DBT_DEVTYP_DEVICEINTERFACE;
            objDeviceBroadcastInterface.Reserved = 0;
            return RegisterDeviceNotification(hWnd, objDeviceBroadcastInterface, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        //=========================================================================================
        /// <summary>
        /// 登録したWindowハンドルの削除
        /// </summary>
        /// <param name="hHandle">登録したWindow ハンドル</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。</returns>
        public static bool UnregisterWindowToReceiveDeviceNotification(IntPtr hHandle)
        {
            return UnregisterDeviceNotification(hHandle);
        }

        //=========================================================================================
        /// <summary>
        /// HID GUIDの生成
        /// </summary>
        public static Guid HIDGuid
        {
            get
            {
                Guid GuidHid;

                HidD_GetHidGuid(out GuidHid);
                return GuidHid;
            }
        }
    }
}
