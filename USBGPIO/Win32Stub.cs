using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace USBGPIO
{
    /// <summary>
    /// helper class to interop with Win32 API
    /// </summary>
    public class Win32Stub
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            //public uint			nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
            //public uint			bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct OVERLAPPED
        {
            public uint Internal;
            public uint InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public IntPtr Event;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved; // should correspond to ULONG_PTR but was an int
        }
        /*[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		protected struct SP_DEVICE_INTERFACE_DATA
        {
            //public int			cbSize;
			public uint			cbSize;
			public Guid			InterfaceClassGuid;
            //public int			Flags;
			public uint			Flags;
			//public int			Reserved;
			public IntPtr		Reserved;
		}*/

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct HIDP_CAPS
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;
        }

        // not used?
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            //public uint			cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DEV_BROADCAST_HDR
        {
            public int cbSize;
            //public UInt32 cbSize;
            public int DeviceType;
            //public UInt32 DeviceType;
            public int Reserved;
            //public UInt32 Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }

        public const int WM_DEVICECHANGE = 0x0219;
        // event type of WM_DEVICECHANGE
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x8007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xffff;

        // use at Flats of SetupDiGetClassDevs()
        public const int DIGCF_DEFAULT = 0x00000001;
        public const int DIGCF_PRESENT = 0x00000002;
        public const int DIGCF_ALLCLASSES = 0x00000004;
        public const int DIGCF_PROFILE = 0x00000008;
        public const int DIGCF_DEVICEINTERFACE = 0x00000010;

        // DEV_BROADCAST_HDR のdbch_devicetype
        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        public const int DBT_DEVTYP_HANDLE = 0x00000006;
        public const int DBT_DEVTYP_OEM = 0x00000000;
        public const int DBT_DEVTYP_PORT = 0x00000003;
        public const int DBT_DEVTYP_VOLUME = 0x00000002;

        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        // use at PurgeComm()
        public const uint PURGE_TXABORT = 0x00000001;
        public const uint PURGE_RXABORT = 0x00000002;
        public const uint PURGE_TXCLEAR = 0x00000004;
        public const uint PURGE_RXCLEAR = 0x00000008;

        // use at CreateFile()
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_SHARE_DELETE = 0x00000004;

        public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        public const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        public const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        public const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        public const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        public const uint FILE_FLAG_RANDOM_ACCES = 0x10000000;
        public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;

        public const uint FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
        public const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
        public const uint FILE_FLAG_OPEN_NO_RECALL = 0x00100000;

        public const uint OPEN_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        public const uint ERROR_IO_PENDING = 997;
        public const uint ERROR_HANDLE_EOF = 415;
        public const uint ERROR_MORE_DATA = 1665;
        public const uint ERROR_INSUFFICIENT_BUFFER = 913;
        public const uint ERROR_INVALID_USER_BUFFER = 5262;
        public const uint ERROR_NOT_ENOUGH_MEMORY = 155;

        public const uint INFINITE = 0xFFFFFFFF;

        public static SafeFileHandle NullHandle = null;

        public const uint HIDP_STATUS_SUCCESS = 0;

        #region Win32_Functions
        /// <summary>
        /// The HidD_GetHidGuid routine returns the device interfaceGUID for HIDClass devices.
        /// </summary>
        /// <param name="HidGuid">Pointer to a caller-allocated GUID buffer that the routine uses to return the device interface GUID for HIDClass devices.</param>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern void HidD_GetHidGuid(out Guid HidGuid);

        /// <summary>
        /// return a device information set that include all devices belong to specified class. Use `SetupDiGetClassDevsEx()` to get device information set on remote computer
        /// </summary>
        /// <param name="ClassGuid">specify pointer to class GUID of setup class or interface class</param>
        /// <param name="Emulator">返されたデバイスをフィルタ処理する文字列へのポインタを指定します。</param>
        /// <param name="hwndParent">このセットのメンバに関連するすべてのユーザーインターフェイスが利用する、トップレベルウィンドウのハンドルを指定します。</param>
        /// <param name="Flags">デバイス情報セットの構築に使われる制御オプションを指定します。このパラメータには、次に示す1つまたは複数の値を指定することができます。 </param>
        /// <returns>関数が成功すると、指定されたパラメータを満たすすべてのインストール済みデバイスを含む、1つのデバイス情報セットのハンドルが返ります。関数が失敗すると、0が返ります。拡張エラー情報を取得するには、関数を使います。</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPStr)] string Emulator,
            IntPtr hwndParent,
            uint Flags);

        /// <summary>
        /// デバイス情報セットを破棄し、関連付けられていたすべてのメモリを解放します。
        /// </summary>
        /// <param name="DeviceInfoSet">破棄するデバイス情報セットのハンドルを指定します。このハンドルは、関数SetupDiCreateDeviceInfoListまたはSetupDiCreateDeviceInfoListExによって作成されたものです。</param>
        /// <returns>関数が成功すると、0以外の値が返ります。関数が失敗すると、0が返ります。拡張エラー情報を取得するには、関数を使います。</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        /// <summary>
        /// デバイス情報セットのデバイスインターフェイスを表すコンテキスト構造体を返します。呼び出しを行うたびに、1つのデバイスインターフェイスに関する情報が返されます。この関数を繰り返し呼び出すと、1つまたは複数のデバイスによって公開されているいくつかのインターフェイスに関する情報を取得できます。
        /// </summary>
        /// <param name="DeviceInfoSet">インターフェイス情報を取得するデバイスが含まれているデバイス情報セットへのポインタを指定します。通常、関数SetupDiGetClassDevsまたはSetupDiGetClassDevsExが返したハンドルを使います。</param>
        /// <param name="DeviceInfoData">デバイス情報セット内のただ1つのデバイスに関するインターフェイスだけを検索するよう制限を加える構造体へのポインタを指定します。このパラメータはオプションです。</param>
        /// <param name="InterfaceClassGuid">要求するインターフェイスのデバイスインターフェイスクラスを指定する、1つのGUIDへのポインタを指定します。</param>
        /// <param name="MemberIndex">デバイス情報セット内のインターフェイスリストに対して、0で始まるインデックス番号を指定します。最初にMemberIndexパラメータを0に設定してこの関数を呼び出し、最初のインターフェイスを取得します。次にMemberIndexを順にインクリメント（値を1増やす）し、インターフェイスを1つずつ取得します。この関数が失敗して関数がERROR_NO_MORE_ITEMSを返すまで、この作業を続けます。</param>
        /// <param name="DeviceInterfaceData">関数が成功した場合に、書き込みの完了した構造体を保持するバッファへのポインタを指定します。この構造体は、検索パラメータを満たす1つのインターフェイスを識別します。この関数を呼び出す前に、cbSizeメンバをsizeof（SP_DEVICE_INTERFACE_DATA)に設定しなければなりません。</param>
        /// <returns>関数が成功すると、0以外の値が返ります。関数が失敗すると、0が返ります。拡張エラー情報を取得するには、関数を使います。</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr DeviceInfoSet,
            uint DeviceInfoData,
            ref Guid InterfaceClassGuid,
            uint MemberIndex,
            ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        /// <summary>
        /// 指定されたデバイスインターフェイスに関する詳細情報を返します。
        /// </summary>
        /// <param name="DeviceInfoSet">インターフェイスとその基になるデバイスが含まれるデバイス情報セットへのポインタを指定します。</param>
        /// <param name="DeviceInterfaceData">インターフェイスを識別する1つの構造体へのポインタを指定します。</param>
        /// <param name="DeviceInterfaceDetailData">指定されたインターフェイスに関する情報を受け取る1つの構造体へのポインタを指定します。</param>
        /// <param name="DeviceInterfaceDetailDataSize">DeviceInterfaceDetailDataが指すバッファのサイズを指定します。</param>
        /// <param name="RequiredSize">DeviceInterfaceDetailDataが指すバッファが必要とするサイズを受け取る、1つの変数へのポインタを指定します。</param>
        /// <param name="DeviceInfoData">要求されたインターフェイスを公開しているデバイスに関する情報を受け取る、1つの構造体へのポインタを指定します。</param>
        /// <returns>関数が成功すると、0以外の値が返ります。関数が失敗すると、0が返ります。拡張エラー情報を取得するには、関数を使います。</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
            IntPtr DeviceInterfaceDetailData,   // refがいる？
            uint DeviceInterfaceDetailDataSize,
            ref uint RequiredSize,
            IntPtr DeviceInfoData
            );
        /*[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern Boolean SetupDiGetDeviceInterfaceDetail(
		   IntPtr hDevInfo,
		   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		   ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
		   UInt32 deviceInterfaceDetailDataSize,
		   ref UInt32 requiredSize,
		   ref SP_DEVINFO_DATA deviceInfoData
		);*/
        /// <summary>
        /// 指定されたデバイスインターフェイスに関する詳細情報を返します。
        /// </summary>
        /// <param name="DeviceInfoSet">インターフェイスとその基になるデバイスが含まれるデバイス情報セットへのポインタを指定します。</param>
        /// <param name="DeviceInterfaceData">インターフェイスを識別する1つの構造体へのポインタを指定します。</param>
        /// <param name="DeviceInterfaceDetailData">指定されたインターフェイスに関する情報を受け取る1つの構造体へのポインタを指定します。</param>
        /// <param name="DeviceInterfaceDetailDataSize">DeviceInterfaceDetailDataが指すバッファのサイズを指定します。</param>
        /// <param name="RequiredSize">DeviceInterfaceDetailDataが指すバッファが必要とするサイズを受け取る、1つの変数へのポインタを指定します。</param>
        /// <param name="DeviceInfoData">要求されたインターフェイスを公開しているデバイスに関する情報を受け取る、1つの構造体へのポインタを指定します。</param>
        /// <returns>関数が成功すると、0以外の値が返ります。関数が失敗すると、0が返ります。拡張エラー情報を取得するには、関数を使います。</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        protected static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData,
            uint DeviceInterfaceDetailDataSize,
            ref uint RequiredSize,
            IntPtr DeviceInfoData);

        /// <summary>
        /// 通知を受け取るデバイスまたはデバイスタイプを指定できるようにします。
        /// </summary>
        /// <param name="hRecipient">NotificationFilter パラメータで指定されたデバイスに関係するデバイスイベントを受け取るウィンドウのハンドルを指定します。</param>
        /// <param name="NotificationFilter">通知の送信先となるデバイスタイプを指定するデータブロックへのポインタを指定します</param>
        /// <param name="Flags">ハンドルタイプを指定します。次の値のいずれかを指定します。 </param>
        /// <returns>関数が成功すると、デバイス通知ハンドルが返ります。関数が失敗すると、NULL が返ります。拡張エラー情報を取得するには、 関数を使います。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr RegisterDeviceNotification(
            IntPtr hRecipient,
            DEV_BROADCAST_HDR NotificationFilter,
            uint Flags);

        /// <summary>
        /// 指定されたデバイス通知ハンドルを閉じます。
        /// </summary>
        /// <param name="Handle">RegisterDeviceNotification 関数が返したデバイス通知ハンドルです。</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、 関数を使います。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool UnregisterDeviceNotification(IntPtr Handle);

        /// <summary>
        /// The HidD_GetPreparsedData routine returns a top-level collection's preparsed data.
        /// </summary>
        /// <param name="HidDeviceObject">Specifies an open handle to a top-level collection.</param>
        /// <param name="PreparsedData">Pointer to the address of a routine-allocated buffer that contains a collection's preparsed data in a _HIDP_PREPARSED_DATA structure.</param>
        /// <returns>HidD_GetPreparsedData returns TRUE if it succeeds; otherwise, it returns FALSE.</returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_GetPreparsedData(
            SafeFileHandle HidDeviceObject,
            out IntPtr PreparsedData);

        /// <summary>
        /// The HidD_FreePreparsedData routine releases the resources that the HID class driver allocated to hold a top-level collection's preparsed data.
        /// </summary>
        /// <param name="PreparsedData">Pointer to the buffer, returned by HidD_GetPreparsedData, that is freed.</param>
        /// <returns>HidD_FreePreparsedData returns TRUE if it succeeds. Otherwise, it returns FALSE if the buffer was not a preparsed data buffer.</returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern bool HidD_FreePreparsedData(ref IntPtr PreparsedData);

        /// <summary>
        /// The HidP_GetCaps routine returns a top-level collection's HIDP_CAPS structure.
        /// </summary>
        /// <param name="PreparsedData">Pointer to a top-level collection's preparsed data.</param>
        /// <param name="Capabilities">Pointer to a caller-allocated buffer that the routine uses to return a collection's HIDP_CAPS structure.</param>
        /// <returns>HIDP_STATUS_SUCCESS  if successful</returns>
        [DllImport("hid.dll", SetLastError = true)]
        protected static extern int HidP_GetCaps(IntPtr PreparsedData, out HIDP_CAPS Capabilities);

        /// <summary>
        /// オブジェクトを作成するか開き、そのオブジェクトをアクセスするために利用できるハンドルを返します。
        /// </summary>
        /// <param name="lpFileName">作成または開く対象のオブジェクトの名前を保持している、NULL で終わる文字列へのポインタを指定します。 </param>
        /// <param name="dwDesiredAccess">オブジェクトへのアクセスのタイプを指定します。アプリケーションは、読み取りアクセス、書き込みアクセス、読み書きアクセス、デバイス問い合わせアクセスのいずれかを取得できます。</param>
        /// <param name="dwShareMode">オブジェクトの共有方法を指定します。</param>
        /// <param name="lpSecurityAttributes">取得したハンドルを子プロセスへ継承することを許可するかどうかを決定する、1 個の 構造体へのポインタを指定します。</param>
        /// <param name="dwCreationDisposition">ファイルが存在する場合、または存在しない場合のファイルの扱い方を指定します。</param>
        /// <param name="dwFlagsAndAttributes">ファイルの属性とフラグを指定します。 </param>
        /// <param name="hTemplateFile">テンプレートファイルに対して GENERIC_READ アクセス権を備えているハンドルを指定します。</param>
        /// <returns>関数が成功すると、指定したファイルに対する、開いているハンドルが返ります。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern SafeFileHandle CreateFile(
            [MarshalAs(UnmanagedType.LPStr)] string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// ファイルにデータを書き込みます。
        /// </summary>
        /// <param name="hFile">書き込み対象のファイルのハンドルを指定します。</param>
        /// <param name="lpBuffer">ファイルに書き込むべきデータを保持しているバッファへのポインタを指定します。</param>
        /// <param name="nNumberOfBytesToWrite">ファイルに書き込むべきバイト数を指定します。 </param>
        /// <param name="lpNumberOfBytesWritten">1 個の変数へのポインタを指定します。関数から制御が返ると、この変数に、実際に書き込まれたバイト数が格納されます。何らかの作業やエラーのチェックを行う前に、WriteFile はこの値を 0 に設定します。 </param>
        /// <param name="lpOverlapped">1 個の 構造体へのポインタを指定します。hFile パラメータが、FILE_FLAG_OVERLAPPED を指定して開いたファイルのハンドルを指している場合、この構造体は必須です。 </param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(
            SafeFileHandle hFile,
            Byte[] lpBuffer,
            UInt32 nNumberOfBytesToWrite,
            ref UInt32 lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        /// <summary>
        /// ファイルからデータを読み取ります。
        /// </summary>
        /// <param name="hFile">読み取り対象のファイルのハンドルを指定します。</param>
        /// <param name="lpBuffer">1 個のバッファへのポインタを指定します。関数から制御が返ると、このバッファに、ファイルから読み取ったデータが格納されます。</param>
        /// <param name="nNumberOfBytesToRead">読み取り対象のバイト数を指定します。</param>
        /// <param name="lpNumberOfBytesRead">1 個の変数へのポインタを指定します。関数から制御が返ると、この変数に、実際に読み取ったバイト数が格納されます。</param>
        /// <param name="lpOverlapped">1 個の 構造体へのポインタを指定します。hFile パラメータが、FILE_FLAG_OVERLAPPED を指定して作成したファイルのハンドルを指している場合、この構造体は必須です。</param>
        /// <returns>ReadFile 関数は、次のいずれかが成立すると制御を返します。パイプの書き込み側で書き込みが完了するか、指定されたバイト数の読み取りが終わるか、エラーが発生した場合です。</returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool ReadFile(
            SafeFileHandle hFile,
            Byte[] lpBuffer,
            UInt32 nNumberOfBytesToRead,
            ref UInt32 lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        /// 指定されたファイルのサイズをバイト単位で取得します。
        /// </summary>
        /// <param name="hFile">サイズを取得するファイルのオープンハンドルを指定します。</param>
        /// <param name="lpFileSize">ファイルサイズを受け取る LARGE_INTEGER 変数へのポインタを指定します。</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、GetLastError 関数を使います｡</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetFileSizeEx(SafeFileHandle hFile, ref long lpFileSize);

        /// <summary>
        /// 開いているオブジェクトハンドルを閉じます。
        /// </summary>
        /// <param name="hFile">開いているオブジェクトのハンドルを指定します。</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern int CloseHandle(SafeFileHandle hFile);

        #endregion

    }
}
