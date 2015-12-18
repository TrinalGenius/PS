using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sven.common.application
{
    class ApplicationUtils
    {

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        private const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void exitWhileMultipleInstance()
        {

            if (getMultipleInstance() != null)
            {
                MessageBox.Show("Multiple Instance");
                System.Environment.Exit(0);
            }


        }

        public static void focusWhilMultipleInsatance()
        {

            Process instance = getMultipleInstance();
            if (instance != null)
            {

                focusOnProcess(instance);
                System.Environment.Exit(0);
            }
        }

        private static void focusOnProcess (Process process)
        {

            if (process != null)
            {

                //ShowWindowAsync(process.MainWindowHandle, WS_SHOWNORMAL);
                SetForegroundWindow(process.MainWindowHandle);      
            }

        }

        private static Process getMultipleInstance()
        {
            Process result = null;
            Process pCurrent = Process.GetCurrentProcess();
            Process[] pList = Process.GetProcessesByName(pCurrent.ProcessName);

            foreach (Process process in pList)
            {
                if (process.Id != pCurrent.Id)
                {

                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == pCurrent.MainModule.FileName)
                    {
                        result = process;
                    }
                }
            }

            return result;
        }
    }
}
