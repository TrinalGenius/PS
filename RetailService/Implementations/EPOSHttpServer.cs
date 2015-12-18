using PS.Terminal.Link.Bridge.Service;
using RetailService.Implementations;
using implementations.common.httpservice;
using implementations.common.log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebSpiderOfPostcode.sven.common.application;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Collections.Specialized;
using RetailService.sven.common.json;
using System.Runtime.Serialization;
using sven.common.httpservice;
using System.Net;

namespace implementations
{
    public class EPOSHttpServer : AbstractHttpServer
    {

        public EPOSHttpServer(string path, int port) : base(path, port)
        {
            base.Initialize(path, port);
        }


        protected override void Process(HttpListenerContext context)
        {
            
            String json = "";

            try
            {
                String path = context.Request.Url.AbsolutePath;
                String query = context.Request.Url.Query;
                FormLogUtils.getInstance().debug(path);
                FormLogUtils.getInstance().debug(query);

                FormLogUtils.getInstance().debug(String.Format("Request->{0}",
                        query));

                if (path.StartsWith("/retail/doTransaction") && query.IndexOf('?') >= 0)
                {

                    NameValueCollection parameters = HttpUtility.ParseQueryString(query);
                    String terminalID = parameters["terminalID"];
                    String amount = parameters["amount"];
                    String sequence = parameters["sequence"];
                    String asynchModel = parameters["asynchModel"];

                    FormLogUtils.getInstance().debug(String.Format("doStransaction request:sequence->{0}, terminalID->{1}, amount->{2}, asynchModel->{3}",
                        sequence, terminalID, amount, asynchModel));

                    if (!String.IsNullOrEmpty(terminalID) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(sequence))
                    {

                        dynamic d = new PropertiesSettingWrapper();
                        if (sequence != d.sequence)
                        {

                            if (!String.IsNullOrEmpty(asynchModel) && "TRUE".Equals(asynchModel, StringComparison.OrdinalIgnoreCase))
                            {
                                Thread asynchThread = new Thread(delegate()
                                {

                                    String result = doTransaction(terminalID, amount, sequence);

                                    FormLogUtils.getInstance().debug(String.Format("doStransaction request:sequence->{0}, result->{1}",
                                                            sequence, result));
                                });
                                asynchThread.IsBackground = true;
                                asynchThread.Start();
                                json = "SUCCESS";
                            }
                            else
                            {

                                json = doTransaction(terminalID, amount, sequence);
                                json = string.Format("PayCard_CallBack({0})", json);

                            }
                        }
                    }


                    FormLogUtils.getInstance().debug(String.Format("doStransaction request:sequence->{0}, result->{1}",
                                                            sequence, json));
                } else if (path.StartsWith("/retail/test"))
                {

                    //TEST
                    JsonData jsonData = new JsonData()
                    {
                        sequence = "1",
                        terminalID = "2",
                        amount = "3"
                    };

                    json = JsonUtils.code(jsonData);
                    json = string.Format("PayCard_CallBack({0})", json);
                }


            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error(e.ToString(), e);
            }



            context.Response.ContentType = "text/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Headers", "*");
            context.Response.AddHeader("Access-Control-Allow-Method", "*");
            context.Response.ContentEncoding = Encoding.UTF8;
            using (var sw = new StreamWriter(context.Response.OutputStream))
            {
                sw.Write(json);

            }

        }

        String doTransaction(String terminalID, String amount, String sequence)
        {
            JsonData jsonData = new JsonData()
            {
                sequence = sequence,
                terminalID = terminalID,
                amount = amount
            };

            if (amount.IndexOf(".") >= 0)
            {

                
            }

            dynamic d = new PropertiesSettingWrapper();
            Dictionary<String, RetailTerminal> terminalsList = d.terminalsList;

            RetailTerminal terminal = null;

            TransactionOut transactionResponse = null;
            if (terminalsList.TryGetValue(terminalID, out terminal))
            {
                FormLogUtils.getInstance().info("start transaction");

                transactionResponse = terminal.doTransaction(Convert.ToInt32(Math.Floor(Convert.ToDecimal(amount))), Convert.ToInt32(Regex.Replace(sequence, "[^0-9]", "")));
                
            }

            if (transactionResponse != null)
            {
                jsonData.status = transactionResponse.TransactionResponse.ToString();
                jsonData.amount = transactionResponse.Amount.ToString();
                jsonData.authCode = transactionResponse.AuthCode;
                jsonData.totalAmount = transactionResponse.TotalAmount.ToString();
                jsonData.merchantID = transactionResponse.MerchantID;
                jsonData.transactionDate = transactionResponse.TransactionDate.ToString();
                jsonData.terminalID = transactionResponse.TerminalID;
            }

            if ("Authorized".Equals(jsonData.authCode, StringComparison.InvariantCultureIgnoreCase))
            {
                d.sequence = sequence;
            }

            return JsonUtils.code(jsonData);
        }
    }

    [DataContract]
    class JsonData 
    {
        [DataMember]
        public String status { get; set; }

        [DataMember]
        public String amount { get; set; }

        [DataMember]
        public String authCode { get; set; }

        [DataMember]
        public String totalAmount { get; set; }

        public String merchantID { get; set; }
        public String transactionDate { get; set; }

        [DataMember]
        public String terminalID { get; set; }

        [DataMember]
        public String sequence { get; set; }

    }
}
