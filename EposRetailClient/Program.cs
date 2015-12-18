using EposRetailClient.implement;
using sven.common.application;
using sven.common.log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EposRetailClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 
        
        [STAThread]
        static void Main()
        {

            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationUtils.focusWhilMultipleInsatance();

            

            try
            {

                FormLogUtils.getInstance().initLog4Net(null);

                String EPOSRetailServiceURL_test = ConfigurationManager.AppSettings["EPOSRetailServiceURL_test"];

                String currencySymbol = ConfigurationManager.AppSettings["currencySymbol"];


                String transactionURL = ConfigurationManager.AppSettings["EPOSRetailServiceURL_transaction"];
                String terminalID = ConfigurationManager.AppSettings["TerminalID"];
                String retailTableList = ConfigurationManager.AppSettings["RetailTableList"];
                Boolean showMessage = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowMessage"]);


                decimal currencyScale = 1;
                String currencyStr = ConfigurationManager.AppSettings["currencyScale"];
                if (!String.IsNullOrEmpty(currencyStr))
                {
                    currencyScale = Convert.ToDecimal(currencyStr);
                }


                FormLogUtils.getInstance().debug(String.Format("request tableList with currencyScale -> {0}, retailTableList-> {1}", currencyScale, retailTableList));

                List<KeyValuePair<string, decimal>> tableList = RetailClient.getCurrTables(currencyScale, retailTableList);


                FormLogUtils.getInstance().debug(String.Format("receive {0} tables info", tableList == null ? 0 : tableList.Count));



                if (tableList == null || tableList.Count == 0)
                {

                    String tip = "No TABLE INFO，PLEASE CHECK CONFIG->'RetailTableList'";
                    MessageBox.Show(tip);
                    FormLogUtils.getInstance().info(tip);

                }
                else if (tableList.Count == 1)
                {

                    KeyValuePair<string, decimal> pair = tableList[0];



                    transaction(transactionURL, terminalID, pair.Key, pair.Value, getTipStr(currencySymbol), showMessage);

                }
                else if (tableList.Count > 1)
                {
                    //display winform

                    int btWidth = Convert.ToInt32(ConfigurationManager.AppSettings["btWidth"]);
                    int btHeight = Convert.ToInt32(ConfigurationManager.AppSettings["btHeight"]);

                    int tablesEachRow = Convert.ToInt32(ConfigurationManager.AppSettings["tablesEachRow"]);
                    int tablesEachColumn = Convert.ToInt32(ConfigurationManager.AppSettings["tablesEachColumn"]);
                    int formHeight = Convert.ToInt32(ConfigurationManager.AppSettings["formHeight"]);

                    int formTop = Convert.ToInt32(ConfigurationManager.AppSettings["formTop"]);
                    int formLeft = Convert.ToInt32(ConfigurationManager.AppSettings["formLeft"]);






                    Control panel = addPanel(btWidth, btHeight, tablesEachRow, tablesEachColumn, tableList, transactionURL, terminalID, currencySymbol, showMessage);
                    Form1 mainForm = new Form1()
                    {

                        Width = panel.Width + 30,
                        Height = Math.Min(formHeight, panel.Height),
                        StartPosition = FormStartPosition.Manual,
                        Left = formLeft,
                        Top = formTop

                    };

                    mainForm.Controls.Add(panel);



                    Application.Run(mainForm);
                }
            }
            catch (Exception e)
            {

                FormLogUtils.getInstance().error(e.ToString(), e);
            }
           
        }

        static Control addPanel(int btWidth, int btHeight, int tablesEachRow, int tablesEachColumn, List<KeyValuePair<string, decimal>> list, String transactionURL, String terminalID, String currencySymbol, Boolean showMessage)
        {



            FlowLayoutPanel flowLayoutPanel1 = new FlowLayoutPanel();
            foreach (KeyValuePair<string, decimal> pair in list)
            {
                Button btn = new Button();
                btn.Height = btHeight;
                btn.Width = btWidth;
                btn.Cursor = Cursors.Hand;
                btn.Name = Convert.ToString(pair.Value);
                btn.Text = pair.Key + Environment.NewLine + currencySymbol + Decimal.Divide(pair.Value, 100).ToString("###,##0.00");

                btn.Click += delegate
                {

                    transaction(transactionURL, terminalID, pair.Key, pair.Value, getTipStr(currencySymbol), showMessage);


                    Application.Exit();

                };


                flowLayoutPanel1.Controls.Add(btn);
            }

            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Width = tablesEachRow * btWidth + 30;
            flowLayoutPanel1.Height = tablesEachColumn * btHeight;
            flowLayoutPanel1.AutoSize = true;

            flowLayoutPanel1.AutoScroll = true;

            return flowLayoutPanel1;

        }


        static String getTipStr(String currencySymbol)
        {
            return "SEND REQUEST " + Environment.NewLine + Environment.NewLine + "      Table:  {0}, " + Environment.NewLine + "      Amount:  " + currencySymbol + "{1} "
                        + Environment.NewLine + "TO " + Environment.NewLine + "     POS:   {2} " + Environment.NewLine + Environment.NewLine;
        }

        static String getSequence()
        {
            return DateTime.Now.ToString("HHmmss");
        }

        static void transaction(String transactionURL, String terminalID, String table, decimal amount, String tipStr, Boolean showMessage)
        {

            String sequence = getSequence();

            FormLogUtils.getInstance().debug(String.Format("transaction request, transactionURL->{0}", transactionURL));
            FormLogUtils.getInstance().debug(String.Format("transaction request, name->{0}, amount->{1}, terminalID->{2}, sequence->{3}", table, amount, terminalID, sequence));

            bool transactionResult = RetailClient.doTransaction(transactionURL, terminalID, amount, getSequence());

            FormLogUtils.getInstance().debug(String.Format("transaction result -> {0}", transactionResult));

            if (transactionResult)
            {
                if (showMessage)
                {

                    MessageBox.Show(String.Format(tipStr, table, Decimal.Divide(amount, 100).ToString("###,##0.00"), terminalID) + (transactionResult ? " SUCCESS" : " FAIL"));
                }
            }
            else
            {
                if (MessageBox.Show(String.Format(tipStr, table, Decimal.Divide(amount, 100).ToString("###,##0.00"), terminalID) + (transactionResult ? " SUCCESS" : " FAIL"), "FAILURE", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                {
                    transaction(transactionURL, terminalID, table, amount, tipStr, showMessage);
                }
            }

            

        }
        
    }
}
