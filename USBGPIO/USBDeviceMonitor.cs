using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;    // for Assembly.GetExecutingAssembly().Location
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBGPIO
{
    /// <summary>
    /// 
    /// </summary>
    [ToolboxBitmap(typeof(USBDeviceMonitor), "usb_icon.bmp")]
    public partial class USBDeviceMonitor : Component
    {
        internal string m_strDevicePath;
        internal uint m_nProductID;
        internal uint m_nVendorID;
        internal string m_ProductName;
        internal Guid m_guidDeviceClass;
        internal IntPtr m_pnUsbEventHandle;
        internal IntPtr m_pnHandle;
        internal USBGPIO m_hidDevice;
        internal int m_id;
        //internal string m_settingPath;
        internal string m_settingFPath;
        //internal UsbHidDevice m_usbhid;

        [Description("send messages like log, to other object")]
        [Category("Embedded Event")]
        [DisplayName("OnNotify")]
        public event EventHandler OnNotify;

        /// <summary>
        /// invoke event when specified device id discovered.
        /// </summary>
        [Description("Invoke event when specified device id discovered.")]
        [Category("Embedded Event")]
        [DisplayName("OnSpecifiedDeviceArrived")]
        public event EventHandler OnSpecifiedDeviceArrived;

        /// <summary>
        /// Invoke event when specified device is removed.
        /// </summary>
        [Description("Invoke event when specified device is removed.")]
        [Category("Embedded Event")]
        [DisplayName("OnSpecifiedDeviceRemoved")]
        public event EventHandler OnSpecifiedDeviceRemoved;

        /// <summary>
        ///	Invoke event when specified device is discovered on bus.
        /// </summary>
        [Description("Invoke event when specified device is discovered on bus.")]
        [Category("Embedded Event")]
        [DisplayName("OnDeviceArrived")]
        public event EventHandler OnDeviceArrived;

        /// <summary>
        /// Invoke event when specified device is removed on bus.
        /// </summary>
        [Description("Invoke event when specified device is removed on bus.")]
        [Category("Embedded Event")]
        [DisplayName("OnDeviceRemoved")]
        public event EventHandler OnDeviceRemoved;


        /// <summary>
        /// invoke event when receive data from spcific device
        /// </summary>
        [Description("Invoke event when receive data from device")]
        [Category("Embedded Event")]
        [DisplayName("OnDataRecieved")]
        public event OnDataReceivedEventHandler OnDataReceived;

        /// <summary>
        /// invoke event when data send
        /// </summary>
        [Description("Invoke event when data send")]
        [Category("Embedded Event")]
        [DisplayName("OnDataSend")]
        public event EventHandler OnDataSend;

        internal uint[] m_PIDCandidates;

        /// <summary>
        /// 
        /// </summary>
        public USBDeviceMonitor(int id = 0)
        {
            m_nVendorID = Settings.Default.VendorID;     //0x1352;   // Km2Net

            // VendorID = 0x1352 ProductID = 0x0100 (km2net USB-IO)
            // VendorID = 0x0BFE ProductID = 0x1003 (Morfy USB-IO)

            // add one by one
            m_PIDCandidates = new uint[] { Settings.Default.USBIO1, Settings.Default.USBIO2, Settings.Default.USBIO2AKI };
            //m_PIDCandidates.Append<uint>(Settings.Default.USBIO2);
            //m_PIDCandidates.Append<uint>(Settings.Default.USBIO2AKI);   // 0x0121	// Akizuki

            //m_nProductID = Settings.Default.USBIO2AKI;   // 0x0121	// Akizuki
            m_nProductID = 0;
            m_ProductName = "unknown device";
            //m_usbhid = new UsbHidDevice((int)m_nVendorID, (int)m_nProductID);

            m_strDevicePath = null;
            m_hidDevice = new USBGPIO();
            m_guidDeviceClass = HIDDevice.HIDGuid;
            m_id = id;

            m_settingFPath = m_hidDevice.GetSettingFilePath(m_id);
            //GetSettingFilePath();

            //InitializeComponent();
        }

        [Description("The product id from the USB device you want to use")]
        [DefaultValue("(none)")]
        [Category("Embedded Details")]
        public uint ProductId
        {
            get
            {
                return this.m_nProductID;
            }
            /*set
            {
                this.m_nProductID = value;
            }*/
        }

        [Description("The vendor id from the USB device you want to use")]
        [DefaultValue("(none)")]
        [Category("Embedded Details")]
        public uint VendorId
        {
            get
            {
                return this.m_nVendorID;
            }
            /*set
            {
                this.m_nVendorID = value;
            }*/
        }

        [Description("The product name derived from ProductId")]
        [DefaultValue("(none)")]
        [Category("Embedded Details")]
        public string ProductName
        {
            get
            {
                return this.m_ProductName;
            }
            /*set
            {
                this.m_nProductID = value;
            }*/
        }

        [Description("The Device Class the USB device belongs to")]
        [DefaultValue("(none)")]
        [Category("Embedded Details")]
        public Guid DeviceClass
        {
            get
            {
                return m_guidDeviceClass;
            }
        }

        [Description("The Device witch applies to the specifications you set")]
        [DefaultValue("(none)")]
        [Category("Embedded Details")]
        public USBGPIO Device
        {
            get
            {
                return this.m_hidDevice;
            }
        }

        [Description("where the configuration stored")]
        [DefaultValue("(none)")]
        [Category("Setting Path")]
        public string ConfigFPath
        {
            get
            {
                return m_settingFPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pnHandle"></param>
        public void RegisterHandle(IntPtr pnHandle)
        {
            m_pnUsbEventHandle = HIDDevice.RegisterWindowToReceiveDeviceNotification(pnHandle, m_guidDeviceClass);
            this.m_pnHandle = pnHandle;
        }

        /// <summary>
        /// unregistration application not to receive notification on the device  
        /// </summary>
        /// <returns>true when succeeded</returns>
        public bool UnregisterHandle()
        {
            if (this.m_pnHandle != IntPtr.Zero)
            {
                return HIDDevice.UnregisterWindowToReceiveDeviceNotification(this.m_pnHandle);
            }

            return false;
        }

        /// <summary>
        /// chech and operate messages sent to Window
        /// </summary>
        /// <param name="msg">Window message</param>
        public void ParseMessages(ref System.Windows.Forms.Message msg)
        {
            if (msg.Msg == Win32Stub.WM_DEVICECHANGE)   // 0x0219
            {
                switch (msg.WParam.ToInt32())
                {
                case Win32Stub.DBT_DEVICEARRIVAL:   // connect device
                    if (OnDeviceArrived != null)
                    {
                        OnDeviceArrived(this, new EventArgs());
                        CheckDevicePresent();
                    }
                    break;
                case Win32Stub.DBT_DEVICEREMOVECOMPLETE:    // delete device
                    if (OnDeviceRemoved != null)
                    {
                        OnDeviceRemoved(this, new EventArgs());
                        CheckDevicePresent();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Check if the device is existed and enabled, then open device path
        /// and fire OnSpecifiedDeviceArrived. OTherwise fire `removed`
        /// </summary>
        public void CheckDevicePresent()
        {
            string oldStrDevicePath = m_strDevicePath;
            try
            {
                bool fExist;

                fExist = false;
                if (Device != null)
                {
                    fExist = true;
                }
                m_strDevicePath = null;
                foreach (var dev_id in m_PIDCandidates)
                {
                    m_strDevicePath = Device.FindDevice(m_nVendorID, dev_id);
                    if (!string.IsNullOrWhiteSpace(m_strDevicePath))
                    {
                        m_nProductID = dev_id;
                        m_ProductName = GetProductName(m_nProductID);
                        Device.Open(m_strDevicePath);
                        if (OnSpecifiedDeviceArrived != null)
                        {
                            this.OnSpecifiedDeviceArrived(this, new EventArgs());
                            Device.DataRecieved += new OnDataReceivedEventHandler(OnDataReceived);
                            Device.DataSend += new OnDataSendEventHandler(OnDataSend);
                        }
                        return;
                    }
                }
                if (OnSpecifiedDeviceRemoved != null && fExist)
                {
                    this.OnSpecifiedDeviceRemoved(this, new EventArgs());
                }
                /*
                m_strDevicePath = Device.FindDevice(m_nVendorID, m_nProductID);
                if (!string.IsNullOrWhiteSpace(m_strDevicePath))
                {
                    Device.Open(m_strDevicePath);
                    if (OnSpecifiedDeviceArrived != null)
                    {
                        this.OnSpecifiedDeviceArrived(this, new EventArgs());
                        Device.DataRecieved += new OnDataReceivedEventHandler(OnDataReceived);
                        Device.DataSend += new OnDataSendEventHandler(OnDataSend);
                    }
                }
                else
                {
                    if (OnSpecifiedDeviceRemoved != null && fExist)
                    {
                        this.OnSpecifiedDeviceRemoved(this, new EventArgs());
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string GetProductName(uint pid)
        {
            switch ((USBGPIO.DeviceId)pid)
            {
            case USBGPIO.DeviceId.u121:
                return "USB-IO2.0(AKI)";
            case USBGPIO.DeviceId.u120:
                return "USB-IO2.0";
            case USBGPIO.DeviceId.u100:
                return "USB-IO1.0";
            default:
                return "unknown device";
            }
        }
    }
}
