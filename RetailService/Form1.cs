using implementations;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.COMBridge.RetailMode;
using RetailService.Implementations;
using implementations.common.httpservice;
using implementations.common.log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WebSpiderOfPostcode.sven.common.application;
using sven.common.httpservice;

namespace RetailService
{
    public partial class Form1 : Form
    {

        AbstractHttpServer httpServer;

        public Form1()
        {
            InitializeComponent();

            ApplicationUtils.exitWhileMultipleInstance();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormLogUtils.getInstance().initLog4Net(this.richTextBox_Log);

            setLog();

            FormLogUtils.getInstance().info("load terminal infos  ... " + (loadTerminalListForXML() ? "success" : "failure"));

            radioButton_start.Checked = true;

        }

        private void setLog()
        {

            FormLogUtils.getInstance().outputObj = this;
            
        }


        private Boolean loadTerminalListForXML()
        {
            Boolean result = false;


            try
            {
                Dictionary<String, RetailTerminal> terminalsList = new Dictionary<String, RetailTerminal>();
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
                        String terminalModelType = ((XmlElement)subNodes[i]).GetAttribute("terminalModelType");
                        if (!String.IsNullOrWhiteSpace(terminalID) && !String.IsNullOrWhiteSpace(terminalPort))
                        {
                            terminalsList.Add(terminalID, new RetailTerminal()
                            {
                                terminalPort = terminalPort,
                                terminalID = terminalID,
                                terminalModelType = terminalModelType

                            });
                        }

                    }
                    FormLogUtils.getInstance().info("Terminals.Count:" + terminalsList.Count);
                    dynamic d = new PropertiesSettingWrapper();
                    d.terminalsList = terminalsList;
                }

                result = true;
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().info(e.ToString());
            }
            return result;

        }

        private void startTerminalService()
        {

            dynamic d = new PropertiesSettingWrapper();
            Dictionary<String, RetailTerminal> terminalsList = d.terminalsList;

            RetailTerminalCallback _callback = new RetailTerminalCallback();
            PS_Terminal_Link_Bridge ecrService = PS_Terminal_Link_Bridge.Instance;
            ecrService.SetCallback(_callback);

            foreach (RetailTerminal terminal in terminalsList.Values)
            {
                terminal.service = ecrService;
                terminal.subscribe();
            }

        }

        private void richTextBox_Log_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton_start_CheckedChanged(object sender, EventArgs e)
        {
            startTerminalService();

            StatusTimer.Enabled = true;

            startEPOSHttpServer();
        }

        private void startEPOSHttpServer()
        {


            httpServer = new EPOSHttpServer("/retail", Convert.ToInt16(ConfigurationManager.AppSettings["httpServerPort"]));

            /*
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.IsBackground = true;
            thread.Start();
             */

            FormLogUtils.getInstance().info("eposHttpServer start... success");
        }
        private void stopEPOSHttpServer()
        {
            StatusTimer.Enabled = false;
            if (httpServer != null)
            {
                httpServer.Stop() ;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            String terminalID = maskedTextBox2.Text;
            String amount = maskedTextBox1.Text;
            String sequence = maskedTextBox3.Text;


            dynamic d = new PropertiesSettingWrapper();
            Dictionary<String, RetailTerminal> terminalsList = d.terminalsList;

            RetailTerminal terminal = null;
            if (terminalsList.TryGetValue(terminalID, out terminal)) 
            {
                FormLogUtils.getInstance().info("start transaction");
                Thread transactionThread = new Thread(delegate()
                {

                    terminal.doTransaction(Convert.ToInt32(Regex.Replace(amount, "[^0-9]", "")), Convert.ToInt32(Regex.Replace(sequence, "[^0-9]", "")));
                });
                transactionThread.IsBackground = true;
                transactionThread.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            StatementsListBox.Items.Clear();

            dynamic d = new PropertiesSettingWrapper();
            Dictionary<String, RetailTerminal> terminalsList = d.terminalsList;

            foreach (var terminal in terminalsList)
            {

                var status = terminal.Value.GetDeviceStatus();
                StatementsListBox.Items.Add(string.Format("Terminal : {0} -  Status : {1}", terminal.Key, status.ToString()));
            }
        }
    }
}
