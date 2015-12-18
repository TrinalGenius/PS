using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSpiderOfPostcode.sven.common.application
{
    class ApplicationUtils
    {

        public static void exitWhileMultipleInstance()
        {

            if (checkMultipleInstance())
            {
                MessageBox.Show("Multiple Instance");
                System.Environment.Exit(0);
            }


        }

        public static Boolean checkMultipleInstance()
        {
            Boolean result = false;
            Process pCurrent = Process.GetCurrentProcess();
            Process[] pList = Process.GetProcessesByName(pCurrent.ProcessName);
            if (pList.Length >= 2)
            {
                result = true;
            }

            return result;

        }
    }
}
