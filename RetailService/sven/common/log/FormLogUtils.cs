using log4net;
using log4net.Config;
using log4net.Filter;
using implementations.common.appenders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace implementations.common.log
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
                /** else if (outputObj is MainForm)
                {
                    MainForm lb = (MainForm)outputObj;
                    //lb.logOnListBox(str);
                }*/

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

        public Boolean initLog4Net(RichTextBox rtb)
        {
            Boolean result = false;
            try
            {
                XmlConfigurator.ConfigureAndWatch(
                    new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

                ColoredRichTextBoxAppender coloredAppender = new ColoredRichTextBoxAppender(rtb, 1024, 100);
                coloredAppender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level  - %message%newline");
                coloredAppender.Name = "coloredRichTextBoxAppender";


                //coloredAppender.AddFilter
                LevelRangeFilter filter = new LevelRangeFilter();
                filter.LevelMin = log4net.Core.Level.Debug;
                filter.LevelMax = log4net.Core.Level.Error;
                coloredAppender.AddFilter(filter);

                BasicConfigurator.Configure(coloredAppender);
                result = true;
            }
            catch
            {
            }
            return result;


        }


    }
}
