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


namespace Implementations.RemoteSettleTerminal
{
    public class RemoteSettleTerminalCallback : PS_Terminal_Link_RemoteSettle_Callback
    {
        public override RemoteStationList GetRemoteStationList(RemoteStationListRequest request)
        {
            RemoteStationList list = new RemoteStationList();
            try {
                FormLogUtils.getInstance().info("Terminal:" + request.TerminalId + " requested for ClerkID " + request.ClerkID.ToString());

                

                List<String> stationsList = new List<String>();
                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);
                //using (sqlConn)
                //{
                //    sqlConn.Open();
                //    SqlCommand cmd = sqlConn.CreateCommand();
                //    cmd.CommandText = "select distinct tableid from [bi_tempbill] order by tableid";
                //    cmd.CommandType = CommandType.Text;
                //    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                //    while (reader.Read())
                //    {
                //        stationsList.Add(reader.GetString(0));
                //    }
                //    if (!reader.IsClosed)
                //    {
                //        reader.Close();
                //    }
                //}
                using (sqlConn)
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "select distinct tablefullname, billID, sumToPay from [bi_tempbill] order by tablefullname";
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                String tableID = reader.GetString(0);
                                String billID = reader.GetString(1);
                                decimal sumToPay = reader.GetDecimal(2);

                                using (SqlConnection sqlConn2 = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]))
                                {
                                    sqlConn2.Open();
                                    using (SqlCommand cmd2 = sqlConn2.CreateCommand())
                                    {

                                        cmd2.CommandText = "select sum(originalAmount - unpaidamount)  from T_EPOS_PaidStatus where status in ('Authorized','PartialAuthorized') and billID = @billID";
                                        cmd2.CommandType = CommandType.Text;
                                        cmd2.Parameters.AddWithValue("@billID", billID);
                                        //cmd2.Parameters.AddWithValue("@paidAmount", Decimal.Multiply(sumToPay, 100));
                                        using (SqlDataReader reader2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection))
                                        {
                                            if (reader2.Read() && !reader2.IsDBNull(0))
                                            {


                                                decimal paidAmount = reader2.GetInt32(0);
                                                if (Decimal.Multiply(sumToPay, 100) <= paidAmount)
                                                {
                                                    continue;
                                                }

                                            }

                                            stationsList.Add(tableID);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }



                foreach (String station in stationsList)
                {

                    FormLogUtils.getInstance().debug("RemoteStations : " + station);
                }

                list.Stations = stationsList.ToArray();

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().debug(e.ToString());
            }
            return list;
        }

        public override RemoteStationBillResponse RemoteStationLock(RemoteStationLockRequest request)
        {
            Confirmation confirmation = Confirmation.NotConfirmed;
            decimal sumToPay = 0;
            try {
                

                String stationID = request.StationID;
                FormLogUtils.getInstance().debug("Terminal:" + request.TerminalId + " requested for Locked Station " + request.StationID);
            
                String clerkID = "";
                if (!String.IsNullOrWhiteSpace(request.ClerkID))
                {
                    clerkID = request.ClerkID;
                
                }

                String terminalID = "";
                if (!String.IsNullOrWhiteSpace(request.TerminalId))
                {
                    terminalID = request.TerminalId;
                
                }

                FormLogUtils.getInstance().debug("Locked By " + request.ClerkID);

                
                String billID = "";


                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);
                using (sqlConn)
                {
                    sqlConn.Open();

                


                    /**
                     * 
                     * -1 locked by other terminal
                     * 0  havent locked
                     * 1  lockd by self
                     * 
                     **/
                    int flag = 0;

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "select terminalID from [T_EPOS_Locked_Stations] where stationID =@stationID order by createtime desc";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@stationID", stationID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                String lockedTerminalID = reader.GetString(0);
                                if (!String.IsNullOrWhiteSpace(lockedTerminalID)) 
                                {

                                    // locked by self
                                    if (lockedTerminalID.Equals(terminalID))
                                    {

                                        flag = 1;
                                    
                                        break;
                                    }
                                    else
                                    {
                                        flag = -1;
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    if (flag == -1)
                    {

                        confirmation = Confirmation.NotConfirmed;
                    }
                    else 
                    {

                        confirmation = Confirmation.Confirmed;

                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {

                            cmd.CommandText = "select billID, SumToPay from [bi_tempbill] where tablefullname = @tableID";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@tableID", stationID);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.Read())
                                {
                                    billID = reader.GetString(0);
                                    sumToPay = reader.GetDecimal(1);
                                }
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(billID))
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {

                                cmd.CommandText = "select sum(originalAmount - unpaidamount)  from T_EPOS_PaidStatus where status in ('Authorized','PartialAuthorized') and billID = @billID";
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@billID", billID);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read() && !reader.IsDBNull(0))
                                    {


                                        decimal paidAmount = reader.GetInt32(0);

                                        sumToPay = sumToPay - decimal.Divide(paidAmount, 100);

                                    }


                                }
                            }
                        }
                        else
                        {
                            FormLogUtils.getInstance().info("no bill infos of stationID: " + stationID);
                            confirmation = Confirmation.NotConfirmed;
                            flag = -1;
                        }

                        if (flag == 0)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {

                                cmd.CommandText = "insert into [T_EPOS_Locked_Stations] values(@terminalID, @stationID, @clerkID, @billID, @createtime)";
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@terminalID", terminalID);
                                cmd.Parameters.AddWithValue("@stationID", stationID);
                                cmd.Parameters.AddWithValue("@clerkID", clerkID);
                                cmd.Parameters.AddWithValue("@billID", billID);
                                cmd.Parameters.AddWithValue("@createtime", DateTime.Now.ToLocalTime());
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().debug(e.ToString());
            }

            return new RemoteStationBillResponse()
            {
                Amount = GlobalProperties.decimalToInt(sumToPay),
                Confirmation = confirmation,
                Currency = "826"
            };
        }

        public override void RemoteStationUnLock(RemoteStationLockRequest request)
        {
            try{
                String stationID = request.StationID;
                String terminalID = request.TerminalId;
                FormLogUtils.getInstance().debug("Terminal:" + request.TerminalId + " requested for Unlocked stationID:" + stationID + ", terminalID:" + terminalID);

                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);
                using (sqlConn)
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "delete from [T_EPOS_Locked_Stations] where stationID = @stationID and terminalID = @terminalID";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@stationID", stationID);
                        cmd.Parameters.AddWithValue("@terminalID", terminalID);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().debug(e.ToString());
            }
        }

        public override Receipt GetReceipt(ReceiptRequest request)
        {

            var lines = new List<LineItem>();
            try {

                int lengthOfFirstCol = Convert.ToInt32(ConfigurationManager.AppSettings["LengthOfFirstCol"]);
                int lengthOfSecondCol = Convert.ToInt32(ConfigurationManager.AppSettings["LengthOfSecondCol"]);
                int lengthOfThirdCol = Convert.ToInt32(ConfigurationManager.AppSettings["LengthOfThirdCol"]);
                int lengthOfLine = Convert.ToInt32(ConfigurationManager.AppSettings["LengthOfLine"]);
                int maxCharsInBoldLine = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCharsInBoldLine"]);
                int isSubstring = Convert.ToInt32(ConfigurationManager.AppSettings["IsSubstring"]);

                int indexOfSecondCol = lengthOfFirstCol + 1;
                int maxLenOfSecondCol = lengthOfSecondCol;
                int lastIndexOfThirdCol = lengthOfLine;

            

                String companyName = GlobalProperties.getValueByKey("CompanyName");
                String phoneNumber = GlobalProperties.getValueByKey("PhoneNumber");
                String chargePersonID = "";
                String billID = "";
                decimal sumToPay = 0;
                decimal sumOfService = 0;
                decimal sumForDiscount = 0;
                String terminalID = request.TerminalId;

                String tableID = "";
                String clerkID = "";

                int peopleCount = 0;
                FormLogUtils.getInstance().debug("Terminal:" + request.TerminalId + " Receipt requested:" + tableID);

                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);

                using (sqlConn)
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "select stationID, clerkID from [T_EPOS_Locked_Stations]  where terminalID = @terminalID order by createtime desc";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@terminalID", terminalID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                tableID = reader.GetString(0);
                                clerkID = reader.GetString(1); ;
                            }
                        }
                    }



                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "select PeopleCount,ChargePersonID,SumToPay,billid,SumOfService,SumForDiscount from [bi_tempbill]  where tablefullname = @tableID";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@tableID", tableID);
                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {

                            if (reader.Read())
                            {
                                peopleCount = reader.GetInt32(0);
                                chargePersonID = reader.GetString(1);
                                sumToPay = reader.GetDecimal(2);
                                billID = reader.GetString(3);
                                sumOfService = reader.GetDecimal(4);
                                sumForDiscount = reader.GetDecimal(5);
                            }
                        }
                    }
                }

                
                int lineSeq = 0;
                //company name
                lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.DoubleHeight }, SortOrderId = 1, LineID = lineSeq++, Text = companyName, ContentType = ContentType.Text });

                //blank line
                lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.DoubleHeight }, SortOrderId = 1, LineID = lineSeq++, Text = " ", ContentType = ContentType.Text });
            
                //tel no
                //lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = phoneNumber, ContentType = ContentType.Text });

                if (GlobalProperties.additionalTopInfoOfReceipt != null)
                {
                    foreach (String additionalLine in GlobalProperties.additionalTopInfoOfReceipt)
                    {
                        lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = additionalLine, ContentType = ContentType.Text });
                    }
                }

            
                //table infos
                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = "Table:" + tableID + "(" + peopleCount + " People)", ContentType = ContentType.Text });
                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = "Staff:" + clerkID, ContentType = ContentType.Text });
                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = "Date:" + DateTime.Now.ToLocalTime().ToString("HH:mm:ss dd-MM-yyyy"), ContentType = ContentType.Text });

                //blank line
                //lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.DoubleHeight }, SortOrderId = 1, LineID = lineSeq++, Text = " ", ContentType = ContentType.Text });

                //Item, Qty, Amount
                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow("ITEM","QTY","Amount", indexOfSecondCol,maxLenOfSecondCol,lastIndexOfThirdCol), ContentType = ContentType.Text });

                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, ContentType = ContentType.Graphic, GraphicType = GraphicType.LineSeparator });

                //lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = "1234567890123456789012345678901234567890", ContentType = ContentType.Text });
                //recept details
                sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);
                using (sqlConn)
                {
                    sqlConn.Open();
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        String menuNameOnReceipt = GlobalProperties.getValueByKey("MenuNameOnReceipt");
                        if (String.IsNullOrEmpty(menuNameOnReceipt))
                        {
                            menuNameOnReceipt = "menuName1";
                        }

                        cmd.CommandText = "select  " + menuNameOnReceipt + " as menuName1 , AmountOrder - AmountCancel,MenuUnitName,SumOfConsume from [bi_tempbillitem] a left join mn_menu b on a.menuid = b.menuid where billid = @billID and  menutypename not in ('Function Menu') and  AmountOrder > AmountCancel ";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@billID", billID);
                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {

                            while (reader.Read())
                            {
                                String colFirstStr = reader.GetString(0);

                                if (colFirstStr.Length > lengthOfFirstCol)
                                {

                                    if (isSubstring == 1)
                                    {
                                        colFirstStr = colFirstStr.Substring(0, lengthOfFirstCol);
                                    }
                                    else
                                    {

                                        while (colFirstStr.Length > lengthOfLine)
                                        {
                                            lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = colFirstStr.Substring(0, lengthOfLine), ContentType = ContentType.Text });
                                            colFirstStr = colFirstStr.Substring(lengthOfLine, colFirstStr.Length - lengthOfLine);
                                        }


                                        if (colFirstStr.Length > lengthOfFirstCol)
                                        {
                                            lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = colFirstStr, ContentType = ContentType.Text });
                                            colFirstStr = "";
                                        }
                                    }

                                }
                                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow(colFirstStr, reader.GetDecimal(1).ToString("##0"), reader.GetDecimal(3).ToString("###,##0.00"), indexOfSecondCol, maxLenOfSecondCol, lastIndexOfThirdCol), ContentType = ContentType.Text });
                            }
                        }
                    }
                }

                foreach (LineItem item in lines)
                {

                    item.Text = strFilter(item.Text);
                }


                if (sumOfService != 0)
                {
                    lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow("S/C", " ", sumOfService.ToString("###,##0.00"), indexOfSecondCol, maxLenOfSecondCol, lastIndexOfThirdCol), ContentType = ContentType.Text });
               
                }

                if (sumForDiscount != 0)
                {
                    lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = GlobalProperties.colsToRow("Discount", " ", sumForDiscount.ToString("###,##0.00"), indexOfSecondCol, maxLenOfSecondCol, lastIndexOfThirdCol), ContentType = ContentType.Text });
                }

                lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, ContentType = ContentType.Graphic, GraphicType = GraphicType.LineSeparator });

                //blank line
                //lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.DoubleHeight }, SortOrderId = 1, LineID = lineSeq++, Text = " ", ContentType = ContentType.Text });

                lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.Bold }, SortOrderId = 1, LineID = lineSeq++, Text = "Amount(GBP):", ContentType = ContentType.Text });

                String sumToPayStr = sumToPay.ToString("###,##0.00"); //"\xA3 " +

                lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.Bold }, SortOrderId = 15, LineID = lineSeq++, Text = GlobalProperties.fillWithSpaces(" ", maxCharsInBoldLine - 1 - sumToPayStr.Length) + sumToPayStr, ContentType = ContentType.Text });

                //blank line
                lines.Add(new LineItem() { Format = new TextFormat[] { TextFormat.DoubleHeight }, SortOrderId = 1, LineID = lineSeq++, Text = " ", ContentType = ContentType.Text });


                //lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = "Tips:\xA3 \x5f \x5f \x5f \x5f \x5f \x5f \x5f \x5f", ContentType = ContentType.Text });


                if (GlobalProperties.additionalBottomInfoOfReceipt != null)
                {
                    foreach (String additionalLine in GlobalProperties.additionalBottomInfoOfReceipt)
                    {
                        lines.Add(new LineItem() { SortOrderId = 1, LineID = lineSeq++, Text = additionalLine, ContentType = ContentType.Text });
                    }
                }

                
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().debug(e.ToString());
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

                FormLogUtils.getInstance().debug("RemoteStationOutcome requested");

                String authCode = outcome.AuthCode;
                String currency = outcome.Currency;
                int gratuityAmount = outcome.GratuityAmount;
                String inputType = outcome.InputType.ToString();
                int isSplitBill = outcome.IsSplitBill ? 1 : 0;
                int originalAmount = outcome.OriginalAmount;
                String status = outcome.Status.ToString();
                String terminalId = outcome.TerminalId;
                int unpaidAmount = outcome.UnpaidAmount;

                /*
                FormLogUtils.getInstance().debug("AuthCode: " + authCode);
                FormLogUtils.getInstance().debug("Currency: " + currency);
                FormLogUtils.getInstance().debug("GratuityAmount: " + gratuityAmount.ToString());
                FormLogUtils.getInstance().debug("InputType: " + inputType);
                FormLogUtils.getInstance().debug("IsSplitBill: " + isSplitBill.ToString());
                FormLogUtils.getInstance().debug("OriginalAmount: " + originalAmount.ToString());
                FormLogUtils.getInstance().debug("Status: " + status);
                FormLogUtils.getInstance().debug("TerminalId: " + terminalId);
                FormLogUtils.getInstance().debug("UnpaidAmount: " + unpaidAmount.ToString());
                */


                String billID = "";
                String tableID = "";
                String clerkID = "";
                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);

                using (sqlConn)
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        cmd.CommandText = "select stationID, clerkID, billID from [T_EPOS_Locked_Stations]  where terminalID = @terminalID order by createtime desc";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@terminalID", terminalId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                tableID = reader.GetString(0);
                                clerkID = reader.GetString(1);
                                billID = reader.GetString(2);

                            }
                        }
                    }

                    FormLogUtils.getInstance().info("billID:" + billID + ",tableID:" + tableID + " ,Terminal:" + outcome.TerminalId + " : OriginalAmount:" + originalAmount + ", UnpaidAmount: " + unpaidAmount + "  :" + status);

                    SqlTransaction sqlTransaction = sqlConn.BeginTransaction();
                    if (!String.IsNullOrWhiteSpace(billID))
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {

                            cmd.CommandText = "insert into [T_EPOS_PaidStatus] values(@billID, @terminalID, @currency, @gratuityAmount, @inputType, @isSplitBill, @originalAmount, @unpaidAmount, @status, @createtime, @clerkID)";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@billID", billID);
                            cmd.Parameters.AddWithValue("@terminalID", terminalId);
                            cmd.Parameters.AddWithValue("@currency", currency);
                            cmd.Parameters.AddWithValue("@gratuityAmount", gratuityAmount);
                            cmd.Parameters.AddWithValue("@inputType", inputType);
                            cmd.Parameters.AddWithValue("@isSplitBill", isSplitBill);
                            cmd.Parameters.AddWithValue("@originalAmount", originalAmount);
                            cmd.Parameters.AddWithValue("@unpaidAmount", unpaidAmount);
                            cmd.Parameters.AddWithValue("@status", status);
                            cmd.Parameters.AddWithValue("@createtime", DateTime.Now.ToLocalTime());
                            cmd.Parameters.AddWithValue("@clerkID", String.IsNullOrWhiteSpace(clerkID) ? "":clerkID);
                            cmd.Transaction = sqlTransaction;
                            cmd.ExecuteNonQuery();
                        }

                        if (outcome.IsSplitBill && outcome.SplitBillTransactions != null)
                        {
                            for (int i = 0; i < outcome.SplitBillTransactions.Length; i++)
                            {


                                SplitBillTransaction trans = outcome.SplitBillTransactions[i];
                                int amount = trans.Amount;
                                int gratuity = trans.GratuityAmount;
                                String paymentType = (String.IsNullOrWhiteSpace(trans.PaymentType) ? "null" : trans.PaymentType);
                                String transactionType = (String.IsNullOrWhiteSpace(trans.TransactionType) ? "null" : trans.TransactionType);
                                /*
                                FormLogUtils.getInstance().debug(String.Format("Split Bill Transaction {0}: ", i));
                                FormLogUtils.getInstance().debug("     Amount: " + amount.ToString());
                                FormLogUtils.getInstance().debug("     Gratuity: " + gratuity.ToString());
                                FormLogUtils.getInstance().debug("     PaymentType: " + paymentType);
                                FormLogUtils.getInstance().debug("     TransactionType: " + transactionType);
                                */

                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {

                                    cmd.CommandText = "insert into [T_EPOS_PaidStatus_Sub] values(@billID, @seq, @amount, @gratuity, @paymentType, @transactionType, @createtime, @clerkID)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@billID", billID);
                                    cmd.Parameters.AddWithValue("@seq", i);
                                    cmd.Parameters.AddWithValue("@amount", amount);
                                    cmd.Parameters.AddWithValue("@gratuity", gratuity);
                                    cmd.Parameters.AddWithValue("@paymentType", paymentType);
                                    cmd.Parameters.AddWithValue("@transactionType", transactionType);
                                    cmd.Parameters.AddWithValue("@createtime", DateTime.Now.ToLocalTime());
                                    cmd.Parameters.AddWithValue("@clerkID", clerkID);
                                    cmd.Transaction = sqlTransaction;
                                    cmd.ExecuteNonQuery();
                                }


                            }
                        }

                    }

                    sqlTransaction.Commit();
                }
            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().debug(e.ToString());
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
            FormLogUtils.getInstance().debug(result);
            return result;
        } 


        protected override bool PingClient()
        {
            return true;
        }
    }
}
