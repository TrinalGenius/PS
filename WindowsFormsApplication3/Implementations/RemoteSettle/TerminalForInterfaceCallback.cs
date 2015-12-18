using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.Bridge.RemoteSettleTerminalService;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using SamplePointOfSaleClient.Implementations;
using EPOSServer.Implementations;
using System.Xml;
using System.IO;
using sven.common.http;


namespace Implementations.RemoteSettleTerminal
{
    public class TerminalForInterfaceCallback : PS_Terminal_Link_RemoteSettle_Callback
    {
        public override RemoteStationList GetRemoteStationList(RemoteStationListRequest request)
        {
            RemoteStationList list = new RemoteStationList();
            try {
                FormLogUtils.getInstance().debug("Terminal:" + request.TerminalId + " requested for ClerkID " + request.ClerkID.ToString());


                String clerkID = request.ClerkID;
                String terminalID = request.TerminalId;

                List<String> stationsList = new List<String>();

                String url = GlobalProperties.getValueByKey("GetRemoteStationList_URL");
                String postText = "clerkID=" + clerkID + "&terminalID=" + terminalID ;
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;
                client.OpenRead(url, postText);
                String xml = client.RespHtml;
                FormLogUtils.getInstance().debug("GetRemoteStationList:" + terminalID + " response xml->" + xml);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList elem = doc.GetElementsByTagName("Error");
                if (elem.Count > 0)
                {
                    foreach (XmlNode node in elem)
                    {
                        FormLogUtils.getInstance().info("GetRemoteStationList: " + terminalID + " " + node.InnerText);
                    }
                }
                else
                {
                    elem = doc.GetElementsByTagName("stationList");
                    foreach (XmlNode node in elem)
                    {
                        XmlNodeList subNodes = node.ChildNodes;

                        for (int i = 0; i < subNodes.Count; i++)
                        {
                            String station = subNodes[i].InnerText;

                            if (!String.IsNullOrWhiteSpace(station))
                            {
                                stationsList.Add(station);
                            }

                        }
                    }
                }


                FormLogUtils.getInstance().info("RemoteStations : " + terminalID  + " " + String.Join(",", stationsList));

                list.Stations = stationsList.ToArray();

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error("RemoteStations : " + request.TerminalId, e);
            }
            return list;
        }

        public override RemoteStationBillResponse RemoteStationLock(RemoteStationLockRequest request)
        {
            Confirmation confirmation = Confirmation.NotConfirmed;
            String currency = "826";
            int sumToPay = 0;
            try {
                

                String stationID = request.StationID;

                FormLogUtils.getInstance().info("RemoteStationLock:" + request.TerminalId + " " + request.ClerkID + " requested for Locked Station " + request.StationID);
                String clerkID = "";
                if (!String.IsNullOrWhiteSpace(request.ClerkID))
                {
                    clerkID = request.ClerkID;

                }
                else if (!String.IsNullOrWhiteSpace(GlobalProperties.getValueByKey("IsTestClerkID")))
                {
                    clerkID = GlobalProperties.getValueByKey("IsTestClerkID");
                }

                String terminalID = "";
                if (!String.IsNullOrWhiteSpace(request.TerminalId))
                {
                    terminalID = request.TerminalId;
                
                }

                String url = GlobalProperties.getValueByKey("GetStationInfo_URL");
                String postText = "clerkID=" + clerkID + "&terminalID=" + terminalID + "&stationID=" + stationID;
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;
                client.OpenRead(url, postText);
                String xml = client.RespHtml;
                FormLogUtils.getInstance().debug("RemoteStationLock:" + request.TerminalId + " response xml->" + xml);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList elem = doc.GetElementsByTagName("Error");
                if (elem.Count > 0)
                {
                    foreach (XmlNode node in elem)
                    {
                        FormLogUtils.getInstance().info("GetRemoteStationList: " + terminalID + " " + node.InnerText);
                    }
                }
                else
                {
                    elem = doc.GetElementsByTagName("stationInfos");
                    foreach (XmlNode node in elem)
                    {

                        XmlNodeList subNodes = node.ChildNodes;
                        foreach (XmlElement subNode in subNodes)
                        {
                            if ("amount".Equals(subNode.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                sumToPay = Convert.ToInt32(subNode.InnerText);
                            }
                            else if ("confirmation".Equals(subNode.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if ("YES".Equals(subNode.InnerText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    confirmation = Confirmation.Confirmed;
                                }

                            }
                            else if ("currency".Equals(subNode.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if ("GBP".Equals(subNode.InnerText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    currency = "826";
                                }

                            }
                        }
                    }

                    FormLogUtils.getInstance().info("GetRemoteStationList: " + terminalID + " " + confirmation + " " + sumToPay );
                }

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error("GetRemoteStationList : " + request.TerminalId, e);
            }

            return new RemoteStationBillResponse()
            {
                Amount = sumToPay,
                Confirmation = confirmation,
                Currency = currency
            };
        }

        public override void RemoteStationUnLock(RemoteStationLockRequest request)
        {
            try
            {

                Confirmation confirmation = Confirmation.NotConfirmed;
                String stationID = request.StationID;
                String terminalID = request.TerminalId;
                FormLogUtils.getInstance().info("RemoteStationUnLock:" + request.TerminalId + " requested for Unlocked stationID:" + stationID);

                String url = GlobalProperties.getValueByKey("StationUnLock_URL");
                String postText = "stationID=" + stationID + "&terminalID=" + terminalID;
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;
                client.OpenRead(url, postText);
                String xml = client.RespHtml;

                FormLogUtils.getInstance().debug("RemoteStationUnLock:" + request.TerminalId + " response xml->" + xml);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList elem = doc.GetElementsByTagName("Error");
                if (elem.Count > 0)
                {
                    foreach (XmlNode node in elem)
                    {
                        FormLogUtils.getInstance().info("GetRemoteStationList: " + terminalID + " " + node.InnerText);
                    }
                }
                else
                {
                    elem = doc.GetElementsByTagName("stationUnlock");
                    foreach (XmlNode node in elem)
                    {

                        XmlNodeList subNodes = node.ChildNodes;
                        foreach (XmlElement subNode in subNodes)
                        {
                            if ("confirmation".Equals(subNode.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if ("YES".Equals(subNode.InnerText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    confirmation = Confirmation.Confirmed;
                                }

                            }
                        }
                    }
                    FormLogUtils.getInstance().info("GetRemoteStationList: " + terminalID + " " + confirmation);
                }


            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error("RemoteStationUnLock : " + request.TerminalId, e);
            }
        }

        public override Receipt GetReceipt(ReceiptRequest request)
        {

            var lines = new List<LineItem>();
            try {

                int lengthOfFirstCol = Convert.ToInt32(GlobalProperties.getValueByKey("LengthOfFirstCol"));
                int lengthOfSecondCol = Convert.ToInt32(GlobalProperties.getValueByKey("LengthOfSecondCol"));
                int lengthOfThirdCol = Convert.ToInt32(GlobalProperties.getValueByKey("LengthOfThirdCol"));
                int lengthOfLine = Convert.ToInt32(GlobalProperties.getValueByKey("LengthOfLine"));
                int maxCharsInBoldLine = Convert.ToInt32(GlobalProperties.getValueByKey("MaxCharsInBoldLine"));
                int isSubstring = Convert.ToInt32(GlobalProperties.getValueByKey("IsSubstring"));

                int indexOfSecondCol = lengthOfFirstCol + 1;
                int maxLenOfSecondCol = lengthOfSecondCol;
                int lastIndexOfThirdCol = lengthOfLine;

            

                String companyName = GlobalProperties.getValueByKey("CompanyName");
                String phoneNumber = GlobalProperties.getValueByKey("PhoneNumber");
                String terminalID = request.TerminalId;

                FormLogUtils.getInstance().info("GetReceipt:" + request.TerminalId + " request for receipt");

                String url = GlobalProperties.getValueByKey("Receipt_URL");
                String postText = "terminalID=" + terminalID;
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;
                client.OpenRead(url, postText);
                String xml = client.RespHtml;
                FormLogUtils.getInstance().debug("GetReceipt:" + request.TerminalId + " response xml->" + xml);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList elem = doc.GetElementsByTagName("Error");
                if (elem.Count > 0)
                {
                    foreach (XmlNode node in elem)
                    {
                        FormLogUtils.getInstance().info("GetReceipt: " + terminalID + " " + node.InnerText);
                    }
                }
                else
                {
                    elem = doc.GetElementsByTagName("receipt");
                    foreach (XmlNode node in elem)
                    {

                        int lineSeq = 0;


                        lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.getValueByKey("base64ImageString"), ContentType = ContentType.Image });

                        XmlNodeList subNodes = node.ChildNodes;
                        foreach (XmlElement subNode in subNodes)
                        {

                            Console.WriteLine(subNode.GetAttribute("doubleHeight"));
                            Console.WriteLine(subNode.InnerText);
                            List<TextFormat> formatList = new List<TextFormat>();

                            String doubleHeight = subNode.GetAttribute("doubleHeight");
                            String lineSeparator = subNode.GetAttribute("LineSeparator");
                            String bold = subNode.GetAttribute("bold");
                            String subItems = subNode.GetAttribute("subItems");
                            String subTitle = subNode.GetAttribute("subTitle");
                            String secondCol = subNode.GetAttribute("secondCol");
                            String thirdCol = subNode.GetAttribute("thirdCol");
                            String isCurrency = subNode.GetAttribute("isCurrency");

                            String firstCol = subNode.InnerText;

                            
                            //LineSeparator
                            if (!String.IsNullOrWhiteSpace(lineSeparator) && "YES".Equals(lineSeparator, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, ContentType = ContentType.Graphic, GraphicType = GraphicType.LineSeparator });
                            }
                            else
                            {
                                //doubleHeight
                                if (!String.IsNullOrWhiteSpace(doubleHeight) && "YES".Equals(doubleHeight, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    formatList.Add(TextFormat.DoubleHeight);
                                }

                                //bold
                                if (!String.IsNullOrWhiteSpace(bold) && "YES".Equals(bold, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    formatList.Add(TextFormat.Bold);
                                }

                                //subTitle
                                if (!String.IsNullOrWhiteSpace(subTitle) && "YES".Equals(subTitle, StringComparison.InvariantCultureIgnoreCase))
                                {

                                    lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow(firstCol, secondCol, thirdCol, indexOfSecondCol, maxLenOfSecondCol, lastIndexOfThirdCol), ContentType = ContentType.Text });
                                }

                                    //subItems
                                else if (!String.IsNullOrWhiteSpace(subItems) && "YES".Equals(subItems, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (firstCol.Length > lengthOfFirstCol)
                                    {

                                        if (isSubstring == 1)
                                        {
                                            firstCol = firstCol.Substring(0, lengthOfFirstCol);
                                        }
                                        else
                                        {

                                            while (firstCol.Length > lengthOfLine)
                                            {
                                                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = firstCol.Substring(0, lengthOfLine), ContentType = ContentType.Text });
                                                firstCol = firstCol.Substring(lengthOfLine, firstCol.Length - lengthOfLine);
                                            }


                                            if (firstCol.Length > lengthOfFirstCol)
                                            {
                                                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = firstCol, ContentType = ContentType.Text });
                                                firstCol = "";
                                            }
                                        }

                                    }

                                    if (!String.IsNullOrWhiteSpace(isCurrency) && "YES".Equals(isCurrency, StringComparison.InvariantCultureIgnoreCase))
                                    {

                                        thirdCol = Decimal.Divide(Convert.ToDecimal(thirdCol), 100).ToString();

                                    }

                                    lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow(firstCol, Convert.ToDecimal(secondCol).ToString("##0"), Convert.ToDecimal(thirdCol).ToString("###,##0.00"), indexOfSecondCol, maxLenOfSecondCol, lastIndexOfThirdCol), ContentType = ContentType.Text });
                                }
                                else
                                {

                                    if (!String.IsNullOrWhiteSpace(isCurrency) && "YES".Equals(isCurrency, StringComparison.InvariantCultureIgnoreCase))
                                    {

                                        firstCol = Decimal.Divide(Convert.ToDecimal(firstCol), 100).ToString("###,##0.00");

                                    }

                                    lines.Add(new LineItem() { Format = formatList.ToArray(), SortOrderId = 1, LineID = lineSeq++, Text = firstCol, ContentType = ContentType.Text });
                                }

                            }

                        }
                    }

                }

                
                                

                foreach (LineItem item in lines)
                {

                    if (item.ContentType == ContentType.Image)
                    {
                        continue;
                    }

                    item.Text = strFilter(item.Text);
                    FormLogUtils.getInstance().debug(item.Text);
                }
                
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error("GetReceipt : " + request.TerminalId, e);
            }

           

            return new Receipt()
            {
                Type = ReceiptType.Customer,
                Lines = lines.ToArray()
            };
        }

        public override void RemoteStationOutcome(TransactionFinalize outcome)
        {

            try
            {
                if (outcome == null)
                {
                    return;
                }

                FormLogUtils.getInstance().debug("RemoteStationOutcome requested" + outcome.TerminalId);

                String authCode = outcome.AuthCode;
                String currency = outcome.Currency;
                int gratuityAmount = outcome.GratuityAmount;
                String inputType = outcome.InputType.ToString();
                int isSplitBill = outcome.IsSplitBill ? 1 : 0;
                int originalAmount = outcome.OriginalAmount;
                String status = outcome.Status.ToString();
                String terminalID = outcome.TerminalId;
                int unpaidAmount = outcome.UnpaidAmount;


                XmlDocument xml = new XmlDocument();
                XmlElement rootElement = xml.CreateElement("result");
                xml.AppendChild(rootElement);

                XmlElement terminalIDElement = xml.CreateElement("terminalID");
                terminalIDElement.InnerText = terminalID;
                rootElement.AppendChild(terminalIDElement);

                XmlElement authCodeElement = xml.CreateElement("authCode");
                authCodeElement.InnerText = authCode;
                rootElement.AppendChild(authCodeElement);

                XmlElement currencyElement = xml.CreateElement("currency");
                currencyElement.InnerText = currency;
                rootElement.AppendChild(currencyElement);

                XmlElement gratuityAmountElement = xml.CreateElement("gratuityAmount");
                gratuityAmountElement.InnerText = Convert.ToString(gratuityAmount);
                rootElement.AppendChild(gratuityAmountElement);

                XmlElement inputTypeElement = xml.CreateElement("inputType");
                inputTypeElement.InnerText = inputType;
                rootElement.AppendChild(inputTypeElement);

                XmlElement isSplitBillElement = xml.CreateElement("isSplitBill");
                isSplitBillElement.InnerText = Convert.ToString(isSplitBill);
                rootElement.AppendChild(isSplitBillElement);

                XmlElement originalAmountElement = xml.CreateElement("originalAmount");
                originalAmountElement.InnerText = Convert.ToString(originalAmount);
                rootElement.AppendChild(originalAmountElement);

                XmlElement unpaidAmountElement = xml.CreateElement("unpaidAmount");
                unpaidAmountElement.InnerText = Convert.ToString(unpaidAmount);
                rootElement.AppendChild(unpaidAmountElement);

                XmlElement statusElement = xml.CreateElement("status");
                statusElement.InnerText = status;
                rootElement.AppendChild(statusElement);

                XmlElement splitBillTransactionsElement = xml.CreateElement("splitBillTransactions");
                rootElement.AppendChild(splitBillTransactionsElement);


                if (outcome.IsSplitBill && outcome.SplitBillTransactions != null)
                {
                    for (int i = 0; i < outcome.SplitBillTransactions.Length; i++)
                    {
                        SplitBillTransaction trans = outcome.SplitBillTransactions[i];
                        int amount = trans.Amount;
                        int gratuity = trans.GratuityAmount;
                        String paymentType = (String.IsNullOrWhiteSpace(trans.PaymentType) ? "null" : trans.PaymentType);
                        String transactionType = (String.IsNullOrWhiteSpace(trans.TransactionType) ? "null" : trans.TransactionType);

                        XmlElement splitBillTransactionElement = xml.CreateElement("splitBillTransaction");
                        splitBillTransactionsElement.AppendChild(splitBillTransactionElement);

                        XmlElement amountElement = xml.CreateElement("Amount");
                        amountElement.InnerText = Convert.ToString(amount);
                        splitBillTransactionElement.AppendChild(amountElement);

                        XmlElement gratuityElement = xml.CreateElement("Gratuity");
                        gratuityElement.InnerText = Convert.ToString(gratuity);
                        splitBillTransactionElement.AppendChild(gratuityElement);

                        XmlElement paymentTypeElement = xml.CreateElement("PaymentType");
                        paymentTypeElement.InnerText = paymentType;
                        splitBillTransactionElement.AppendChild(paymentTypeElement);

                        XmlElement transactionTypeElement = xml.CreateElement("TransactionType");
                        transactionTypeElement.InnerText = transactionType;
                        splitBillTransactionElement.AppendChild(transactionTypeElement);

                    }
                }

                FormLogUtils.getInstance().debug("RemoteStationOutcome:" + terminalID + " xml->" + xml.InnerXml);

                String url = GlobalProperties.getValueByKey("PaidInfo_URL");
                String postText = "paidinfo=" + xml.InnerXml;
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.Default;

                try
                {
                    
                    client.OpenRead(url, postText);
                    FormLogUtils.getInstance().debug("RemoteStationOutcome:" + terminalID + " xml->" + client.RespHtml);
                    
                }
                catch (Exception e)
                {

                    /*
                    String fileName = DateTime.Now.ToFileTimeUtc().ToString() + "error.xml";
                    FormLogUtils.getInstance().info("remote service error, save to" + fileName + "  and try later ");
                    xml.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
                     **/
                    FormLogUtils.getInstance().error("RemoteStationOutcome : " + terminalID, e);
                }

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error("RemoteStationOutcome : " + outcome.TerminalId, e);
            }
        }



        public static String strFilter(String s)
        {
            String result = String.Empty;
            if (!String.IsNullOrWhiteSpace(s))
            {
                byte[] arrByte = Encoding.GetEncoding("GB2312").GetBytes(s);
                for (int i = 0; i < arrByte.Length; i++)
                {
                    if (arrByte[i] >= 0x20 && arrByte[i] <= 0x7E)
                    {
                        result += Convert.ToString((char)arrByte[i]);
                    }
                    else
                    {
                        result += "";
                    }

                }
            }
           
            return result;
        } 


        protected override bool PingClient()
        {
            return true;
        }
    }
}
