using System;
using System.Windows.Forms;

namespace HDMI_Power_Manager_Tray
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HDMIPowerManagerForm());
        }
    }
}
