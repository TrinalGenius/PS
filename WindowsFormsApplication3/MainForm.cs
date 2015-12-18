using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using SamplePointOfSaleClient.Implementations;
using Implementations.RemoteSettleTerminal;
using EPOSServer.Implementations;
using System.Xml;
using System.IO;
using PS.Terminal.Link.Bridge.RemoteSettleTerminalService;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using sven.common.image;
using log4net.Config;
using sven.common.appenders;
using log4net.Filter;
using log4net.Repository;
using log4net;
using log4net.Appender;
using System.Diagnostics;

namespace WindowsFormsApplication3
{
    public partial class MainForm : Form
    {

        RemoteSettleServer remoteSettleServer;
        public MainForm()
        {
            InitializeComponent();

            if (checkMultipleInstance())
            {
                MessageBox.Show("Multiple Instance");
                System.Environment.Exit(0);
            }




            CheckForIllegalCrossThreadCalls = false;
            tabPage2.Parent = null;

            setLog();

            
            FormLogUtils.getInstance().info("inti log4net   ... " + (initLog4Net() ? "success" : "failure"));
            FormLogUtils.getInstance().info("load app infos  ... " + (loadAppInfos() ? "success" : "failure"));

            FormLogUtils.getInstance().info("load additional infos of Receipt ... " + (loadAdditionalInfosOfReceipt() ? "success" : "failure"));

            FormLogUtils.getInstance().info("load terminal infos  ... " + (loadTerminalListForXML() ? "success" : "failure"));

            FormLogUtils.getInstance().info("load server infos  ... " + (loadServerInfos() ? "success" : "failure"));

            FormLogUtils.getInstance().info("load receipt image  ... " + (loadReceiptImage() ? "success" : "failure"));

            this.Text = this.Text + GlobalProperties.getValueByKey("ServiceModel");

            if ("DatabaseModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
            {

                FormLogUtils.getInstance().info("connecting to database ... " + (checkDatabaseConnection() ? "success" : "failure"));

            }

            if ("InterfaceModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
            {

                FormLogUtils.getInstance().info("load Interface Infos ... " + (loadInterfaceInfos() ? "success" : "failure"));
            }
            

        }

        private Boolean checkMultipleInstance()
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


        private Boolean loadReceiptImage()
        {

            Boolean result = false;
            try
            {
                FormLogUtils.getInstance().info("load receipt image from file ... " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalProperties.getValueByKey("ImageURL")));
                Image image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalProperties.getValueByKey("ImageURL")));
                
                String base64ImageString = ImageHelper.ImageToBase64(image, ImageFormat.Bmp);

                GlobalProperties.setKeyAndValue("base64ImageString", base64ImageString);

                result = true;
            }
            catch (Exception e)
            {

            }


            return result;

        }


        private Boolean loadTerminalListForXML()
        {
            Boolean result = false;

            
            try
            {

                List<TCPPortConfiguration> terminalsList = new List<TCPPortConfiguration>();
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Terminals.xml"));

                XmlNodeList elem = doc.GetElementsByTagName("terminals");
                foreach (XmlNode node in elem)
                {
                    XmlNodeList subNodes = node.ChildNodes;

                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        String terminalID = subNodes[i].InnerText;
                        String terminalPort = ((XmlElement)subNodes[i]).GetAttribute("port");
                        if (!String.IsNullOrWhiteSpace(terminalID) &&  !String.IsNullOrWhiteSpace(terminalPort)) {
                            terminalsList.Add(new TCPPortConfiguration()
                            {
                                Port = Convert.ToInt32(terminalPort),
                                TerminalId = terminalID

                            });
                        }
                        
                    }
                    FormLogUtils.getInstance().info("Terminals.Count:" + terminalsList.Count);
                    GlobalProperties.terminalsList = terminalsList;
                }
                
                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }
            return result;

        }


        private Boolean initLog4Net()
        {
            Boolean result = false;
            try
            {
                XmlConfigurator.ConfigureAndWatch(
                    new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

                ColoredRichTextBoxAppender coloredAppender = new ColoredRichTextBoxAppender(this.richTextBox_Log, 1024, 100);
                coloredAppender.Layout = new log4net.Layout.PatternLayout(" %message%newline");
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

        private Boolean checkDatabaseConnection()
        {
            Boolean result = false;


            try
            {
                SqlConnection sqlConn = new SqlConnection(GlobalProperties.getValueByKey("SQLString"));
                using (sqlConn)
                {
                    sqlConn.Open();

                    SqlCommand cmd = sqlConn.CreateCommand();
                    cmd.CommandText = "select 1";
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                    if (reader.Read())
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }

            return result;
        }

        private Boolean loadServerInfos()
        {
            Boolean result = false;

            try
            {
                GlobalProperties.setKeyAndValue("ImageURL", ConfigurationManager.AppSettings["ImageURL"]);
               



                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }

            return result;
        }

        private Boolean loadAppInfos()
        {
            Boolean result = false;

            try
            {
                GlobalProperties.setKeyAndValue("SQLString", ConfigurationManager.AppSettings["SQLString"]);
                GlobalProperties.setKeyAndValue("LengthOfFirstCol", ConfigurationManager.AppSettings["LengthOfFirstCol"]);
                GlobalProperties.setKeyAndValue("LengthOfSecondCol", ConfigurationManager.AppSettings["LengthOfSecondCol"]);
                GlobalProperties.setKeyAndValue("LengthOfThirdCol", ConfigurationManager.AppSettings["LengthOfThirdCol"]);
                GlobalProperties.setKeyAndValue("LengthOfLine", ConfigurationManager.AppSettings["LengthOfLine"]);
                GlobalProperties.setKeyAndValue("IsSubstring", ConfigurationManager.AppSettings["IsSubstring"]);
                GlobalProperties.setKeyAndValue("MaxCharsInBoldLine", ConfigurationManager.AppSettings["MaxCharsInBoldLine"]);
                GlobalProperties.setKeyAndValue("CompanyName", ConfigurationManager.AppSettings["CompanyName"]);
                GlobalProperties.setKeyAndValue("ServiceModel", ConfigurationManager.AppSettings["ServiceModel"]);
                GlobalProperties.setKeyAndValue("MenuNameOnReceipt", ConfigurationManager.AppSettings["MenuNameOnReceipt"]);

                //Test
                GlobalProperties.setKeyAndValue("IsTestClerkID", ConfigurationManager.AppSettings["IsTestClerkID"]);

                

                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }

            return result;
        }

        private Boolean loadInterfaceInfos()
        {
            Boolean result = false;

            try
            {
                GlobalProperties.setKeyAndValue("GetRemoteStationList_URL", ConfigurationManager.AppSettings["GetRemoteStationList_URL"]);
                GlobalProperties.setKeyAndValue("GetStationInfo_URL", ConfigurationManager.AppSettings["GetStationInfo_URL"]);
                GlobalProperties.setKeyAndValue("StationUnLock_URL", ConfigurationManager.AppSettings["StationUnLock_URL"]);
                GlobalProperties.setKeyAndValue("Receipt_URL", ConfigurationManager.AppSettings["Receipt_URL"]);
                GlobalProperties.setKeyAndValue("PaidInfo_URL", ConfigurationManager.AppSettings["PaidInfo_URL"]);

                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }

            return result;
        }

        private Boolean loadAdditionalInfosOfReceipt()
        {
            Boolean result = false;

            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AdditionalInfoInReceipt.xml"));

                XmlNodeList elem = doc.GetElementsByTagName("top");
                foreach (XmlNode node in elem)
                {
                    XmlNodeList subNodes = node.ChildNodes;
                    String[] additionalTopInfoOfReceipt = new String[subNodes.Count];

                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        additionalTopInfoOfReceipt[i] = subNodes[i].InnerText;
                    }
                    FormLogUtils.getInstance().info("additionalTopInfoOfReceipt.Count:" + subNodes.Count);
                    GlobalProperties.additionalTopInfoOfReceipt = additionalTopInfoOfReceipt;
                }
                elem = doc.GetElementsByTagName("bottom");
                foreach (XmlNode node in elem)
                {
                    XmlNodeList subNodes = node.ChildNodes;
                    String[] additionalBottomInfoOfReceipt = new String[subNodes.Count];
                    for (int i = 0; i < subNodes.Count; i++)
                    {
                        additionalBottomInfoOfReceipt[i] = subNodes[i].InnerText;
                    }
                    FormLogUtils.getInstance().info("additionalBottomInfoOfReceipt.Count:" + subNodes.Count);
                    GlobalProperties.additionalBottomInfoOfReceipt = additionalBottomInfoOfReceipt;
                }
                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }
            return result;
        }

        private void setLog()
        {

            FormLogUtils.getInstance().outputObj = this;
            FormLogUtils.getInstance().debugSw = this.checkBox_log_debug.Checked;
            FormLogUtils.getInstance().infoSw = this.checkBox_log_Info.Checked;
        }

        private void radioButton_start_CheckedChanged(object sender, EventArgs e)
        {

            if (((RadioButton)sender).Checked)
            {
                if (remoteSettleServer == null)
                {
                    remoteSettleServer = new RemoteSettleServer();
                }
                remoteSettleServer.Run();
                FormLogUtils.getInstance().info("start server ... success");
            }
            
            
        }

        private void radioButton_stop_CheckedChanged(object sender, EventArgs e)
        {

            if (((RadioButton)sender).Checked)
            {
                if (remoteSettleServer != null)
                {

                    remoteSettleServer.shutDown();

                }
                FormLogUtils.getInstance().info("shut down server ... success");
            }
            
        }

        private void radioButton_stop_Click(object sender, EventArgs e)
        {


        }



        private void checkBox_log_Info_CheckedChanged(object sender, EventArgs e)
        {
            FormLogUtils.getInstance().infoSw = this.checkBox_log_Info.Checked;
        }

        private void checkBox_log_debug_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ILoggerRepository[] ress = LogManager.GetAllRepositories();
                foreach (ILoggerRepository res in ress)
                {

                    IAppender[] appenders = res.GetAppenders();
                    foreach (IAppender ap in appenders)
                    {
                        if ("coloredRichTextBoxAppender".Equals(ap.Name))
                        {
                            ColoredRichTextBoxAppender coloredAp = (ColoredRichTextBoxAppender)ap;
                            coloredAp.ClearFilters();
                            coloredAp.AddFilter(new LevelRangeFilter()
                            {

                                LevelMin = this.checkBox_log_Info.Checked ? log4net.Core.Level.Debug : log4net.Core.Level.Info,
                                LevelMax = log4net.Core.Level.Error


                            });

                            FormLogUtils.getInstance().info("set log level success");
                        }
                    }
                }
            }
            catch (Exception excetpion)
            {
                FormLogUtils.getInstance().error(" set Log level failure", excetpion);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            
        }



        private void button_unlock_Click(object sender, EventArgs e)
        {
            String tableID = this.maskedTextBox_tableID.Text;
            if(String.IsNullOrWhiteSpace(tableID)) {
                
                
                //msgbox("invalid tableID");
                MessageBox.Show("invalid tableID", "Infos", MessageBoxButtons.OK, MessageBoxIcon.Question);
            } else {
                tableID = tableID.Trim();
                // unlocktablke
                DialogResult a = MessageBox.Show("unlock talbe " + tableID + "?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (a == DialogResult.No)
                {
                    return;
                }
                try
                {


                    FormLogUtils.getInstance().info("EPOS Server requested for Unlocked stationID:" + tableID);


                    if ("DatabaseModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
                    {
                        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);
                        using (sqlConn)
                        {
                            sqlConn.Open();

                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {

                                cmd.CommandText = "delete from [T_EPOS_Locked_Stations] where stationID = @stationID";
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@stationID", tableID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        
                    }
                    else if ("InterfaceModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
                    {

                        RemoteStationLockRequest request = new RemoteStationLockRequest();
                        request.TerminalId = "";
                        request.StationID = tableID;

                        new TerminalForInterfaceCallback().RemoteStationUnLock(request);

                    }




                    

                }
                catch (Exception exception)
                {
                    FormLogUtils.getInstance().debug(exception.ToString());
                }

            }

        }

        private void timer_paidinfoXml_Tick(object sender, EventArgs e)
        {

        }


    }
}
