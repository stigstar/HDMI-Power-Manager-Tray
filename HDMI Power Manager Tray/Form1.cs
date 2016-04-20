using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HDMI_Power_Manager_Tray
{
    public partial class HDMIPowerManagerForm : Form
    {
        private IntPtr _hConsoleDisplayState;

        Guid GUID_CONSOLE_DISPLAY_STATE = new Guid("6fe69556-704a-47a0-8f24-c28d936fda47");
        const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        const int WM_POWERBROADCAST = 0x0218;
        const int PBT_POWERSETTINGCHANGE = 0x8013;
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public int DataLength;
        }

        [DllImport(@"User32", EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        public HDMIPowerManagerForm()
        {
            InitializeComponent();
            RegisterForPowerNotifications();
            this.FormClosing += new FormClosingEventHandler(Form1FormClosing);
        }


        void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
            //todo: move to application exit. form closing should just hide the form.
            UnregisterForPowerNotifications();
        }

        private void RegisterForPowerNotifications()
        {
            _hConsoleDisplayState = RegisterPowerSettingNotification(this.Handle, ref GUID_CONSOLE_DISPLAY_STATE, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        private void UnregisterForPowerNotifications()
        {
            UnregisterPowerSettingNotification(_hConsoleDisplayState);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_POWERBROADCAST)
            {
                OnPowerBroadcast(ref m);
            }

            base.WndProc(ref m);
        }

        private void OnPowerBroadcast(ref Message m)
        {
            if ((int)m.WParam == PBT_POWERSETTINGCHANGE)
            {
                PowerSettingChange(m);
            }
        }

        private void PowerSettingChange(Message m)
        {
            var ps = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(m.LParam, typeof(POWERBROADCAST_SETTING));

            var pData = (IntPtr)((int)m.LParam + Marshal.SizeOf(ps));

            if (ps.DataLength == Marshal.SizeOf(typeof(int)))
            {
                var iData = (int)Marshal.PtrToStructure(pData, typeof(int));
                if (ps.PowerSetting == GUID_CONSOLE_DISPLAY_STATE)
                {
                    SetConsoleDisplayState(iData);
                }
            }
        }

        private void SetConsoleDisplayState(int iData)
        {
            string eventText;
            switch (iData)
            {
                case 0:
                    eventText = "The display is off.";
                    textBox1.Text = textBox1.Text + "\n" + eventText;
                    break;

                case 1:
                    eventText = "The display is on.";
                    textBox1.Text = textBox1.Text + "\n" + eventText;
                    break;

                case 2:
                    eventText = " The display is dimmed.";
                    textBox1.Text = textBox1.Text + "\n" + eventText;
                    break;

                default:
                    eventText = "Unknown console display event";
                    textBox1.Text = textBox1.Text + "\n" + eventText;
                    break;
            }
        }
    }
}
