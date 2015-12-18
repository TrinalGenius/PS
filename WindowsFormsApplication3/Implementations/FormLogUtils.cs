using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication3;
namespace EPOSServer.Implementations
{

    public class FormLogUtils
    {
        private static FormLogUtils instance;
        ILog _log = null;
        public Object outputObj { get; set; }
        public bool infoSw  { get; set; }
        public bool debugSw { get; set; }



        private FormLogUtils()
        {

        }

        public static FormLogUtils getInstance()
        {
            if (instance == null)
            {
                instance = new FormLogUtils();
                instance._log = log4net.LogManager.GetLogger("coloredRichTextBoxAppender");
            }
            return instance;
        }

        void log(String str)
        {

            if (outputObj != null)
            {

                if (outputObj is ListBox)
                {

                    ListBox lb = (ListBox)outputObj;
                    lb.Items.Add(str);

                }
                else if (outputObj is MainForm)
                {
                    MainForm lb = (MainForm)outputObj;
                    //lb.logOnListBox(str);
                }

            }
        }

        public void info(String str)
        {
            if (_log != null)
            {
                _log.Info(str);
            }
        }
        public void debug(String str)
        {
            if (_log != null)
            {
                _log.Debug(str);
            }
        }

        public void error(String str)
        {
            if (_log != null)
            {
                _log.Error(str);
            }
        }

        public void error(String str, Exception e)
        {
            if (_log != null)
            {
                _log.Error(str, e);
            }
        }


    }
}
