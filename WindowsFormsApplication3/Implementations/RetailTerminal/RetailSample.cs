using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.Bridge.Service;

namespace SamplePointOfSaleClient
{
    public class RetailSample
    {
        private static string _terminalId;
        private static int _transCounter = 0;
        private static TerminalBrandType _terminalBrandType;

        private TerminalCallback _callback;

        public void Run()
        {
            bool enablePrintCallback = false;
            _callback = new TerminalCallback();

            PS_Terminal_Link_Bridge ecrService = PS_Terminal_Link_Bridge.Instance;
            ecrService.SetCallback(_callback);

            ConsoleKey key = ConsoleKey.Attention;

            _terminalId = ConfigurationManager.AppSettings["TerminalId"];
            Console.WriteLine("Executing Subscribe.");
            Subscribe(ecrService);

            Console.WriteLine("The client has started. Press a key to send a message to the terminal.");
            Console.WriteLine();

            while (key != ConsoleKey.Escape)
            {
                Console.WriteLine("Available Commands: (Some commands not supported on all devices)");

                if (_terminalBrandType == TerminalBrandType.Ingenico_CA_005)
                {
                    Console.WriteLine("     A - Cancel transaction.");
                }
                if (_terminalBrandType == TerminalBrandType.Ingenico_TLV_16)
                {
                    Console.WriteLine("     C - Check Eligiblity.");
                }
                Console.WriteLine("     L - Get Last Transaction.");
                Console.WriteLine("     P - Configure Print.");
                Console.WriteLine("     Q - Recycle COM Port.");
                if (_terminalBrandType == TerminalBrandType.Ingenico_TLV_16)
                {
                    Console.WriteLine("     R - Send Print Request.");
                }
                Console.WriteLine("     S - Get Status.");
                Console.WriteLine("     T - Perform Transaction.");
                Console.WriteLine("     U - Unsubscribe.");
                Console.WriteLine("     V - Subscribe.");
                Console.WriteLine("     G - Get Report.");
                Console.WriteLine("     Z - Message Throughput Stress Test (1000 iterations).");
                Console.WriteLine("     Escape - Exit client.");
                Console.WriteLine();

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.WriteLine();
                key = keyInfo.Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.WriteLine("Executing Regression Test Suite 1.");
                            RegressionTest_01(ecrService);
                        }
                        break;
                    case ConsoleKey.A:
                        {
                            if (_terminalBrandType == TerminalBrandType.Ingenico_CA_005)
                            {
                                Console.WriteLine("Executing Cancel Transaction.");
                                CancelTransaction(ecrService);
                            }
                        }
                        break;
                    case ConsoleKey.C:
                        {
                            if (_terminalBrandType == TerminalBrandType.Ingenico_TLV_16)
                            {
                                Console.WriteLine("Executing CheckEligiblity.");
                                CheckEligibility(ecrService);
                            }
                        }
                        break;
                    case ConsoleKey.L:
                        {
                            Console.WriteLine("Executing GetLastTransaction.");
                            GetLastTransaction(ecrService);
                        }
                        break;
                    case ConsoleKey.P:
                        {
                            enablePrintCallback = !enablePrintCallback;
                            Console.WriteLine("Executing ConfigurePrint: " + enablePrintCallback);
                            ConfigurePrint(ecrService, enablePrintCallback);
                        }
                        break;
                    case ConsoleKey.Q:
                        {
                            Console.WriteLine("Executing ResetDevice.");
                            ResetDevice(ecrService);
                        }
                        break;
                    case ConsoleKey.R:
                        {
                            if (_terminalBrandType == TerminalBrandType.Ingenico_TLV_16)
                            {
                                Console.WriteLine("Executing SendPrintRequest.");
                                SendPrintRequest(ecrService);
                            }
                        }
                        break;
                    case ConsoleKey.S:
                        {
                            Console.WriteLine("Executing GetStatus.");
                            GetDeviceStatus(ecrService);
                        }
                        break;
                    case ConsoleKey.T:
                        {
                            Console.WriteLine("Executing PerformTransaction.");
                            DoTransaction(ecrService);
                        }
                        break;
                    case ConsoleKey.U:
                        {
                            Console.WriteLine("Executing Unsubscribe.");
                            Unsubscribe(ecrService);
                        }
                        break;
                    case ConsoleKey.V:
                        {
                            Console.WriteLine("Executing Subscribe.");
                            Subscribe(ecrService);
                        }
                        break;
                    case ConsoleKey.G:
                        {
                            Console.WriteLine("Executing GetReport.");
                            this.GetReport(ecrService);
                        }
                        break;
                    case ConsoleKey.Z:
                        {
                            Console.WriteLine("Executing Message Throughput Stress Test");
                            Stopwatch sw = new Stopwatch();
                            for (int i = 1; i <= 10000; i++)
                            {
                                sw.Start();
                                GetDeviceStatus(ecrService);
                                sw.Stop();
                                Console.WriteLine(string.Format("Test #{0} took {1} seconds", i, sw.Elapsed.ToString()));
                                sw.Reset();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            Unsubscribe(ecrService);
        }

        private void RegressionTest_01(PS_Terminal_Link_Bridge ecrService)
        {

            Stopwatch sw = new Stopwatch();
            int errors = 0;
            int notIdle = 0;
            for (int i = 1; i <= 257; i++)
            {
                {
                    sw.Start();
                    DeviceStatus status = DeviceStatus.Unknown;

                    RequestStatus rStatus = ecrService.GetCurrentStatus(new GetCurrentStatusRequest() { TerminalId = _terminalId }, out status);

                    Console.WriteLine(status.ToString());

                    if (rStatus != RequestStatus.RequestSent)
                    {
                        errors++;
                    }
                    else if (status != DeviceStatus.Idle)
                    {
                        notIdle++;
                    }

                    sw.Stop();
                    Console.WriteLine(string.Format("Test #{0} A took {1} seconds", i, sw.Elapsed.ToString()));
                    sw.Reset();
                }
                {
                    sw.Start();
                    int transId = _transCounter++;
                    Console.WriteLine("Transaction Number: " + transId.ToString());

                    var task = Task.Factory.StartNew(() =>
                    {
                        TransactionOut transactionResponse;
                        RequestStatus rStatus = ecrService.PerformTransaction(new TransactionIn()
                        {
                            TerminalId = _terminalId,
                            TransactionID = transId.ToString(),
                            TypeRequest = TransactionType.Refund,
                            Currency = "826",
                            ValueIn = new ValueIn()
                            {
                                Value1 = i,
                            }
                        }, out transactionResponse);

                        if ((rStatus != RequestStatus.RequestSent) ||
                        (transactionResponse != null && transactionResponse.TransactionResponse != TransactionResponse.Cancelled))
                        {
                            errors++;
                        }
                    });

                    while (true)
                    {
                        if (this._callback.LastStatus == DeviceStatus.InsertCard || this._callback.LastStatus == DeviceStatus.PresentCard)
                        {
                            break;
                        }
                        Thread.Sleep(5);

                        if (sw.ElapsedMilliseconds > 20000)
                        {
                            Console.WriteLine(string.Format("20 seconds elapsed, perform transaction message must have errored."));
                            errors++;
                            break;
                        }
                    }

                RetryAfterConnectionFail:
                    bool cancelled = false;
                    RequestStatus rStatus2 = ecrService.CancelTransaction(new CancelTransactionRequest() { TerminalId = _terminalId, TransactionID = transId }, out cancelled);
                    if (rStatus2 != RequestStatus.RequestSent)
                    {
                        errors++;
                    }

                    Console.WriteLine(rStatus2.ToString());

                    if (rStatus2 == RequestStatus.SendFail)
                    {
                        goto RetryAfterConnectionFail;
                    }

                    Task.WaitAll(task);

                    this._callback.LastStatus = DeviceStatus.Cancelled;
                    sw.Stop();
                    Console.WriteLine(string.Format("Test #{0} B took {1} seconds", i, sw.Elapsed.ToString()));
                    sw.Reset();

                }
                Console.WriteLine(string.Format("Total tests {0} with total errors {1}", i, errors));

            }
        }

        private void ResetDevice(PS_Terminal_Link_Bridge ecrService)
        {
            SerialPortConfiguration terminalConfig = new SerialPortConfiguration();

            terminalConfig.TerminalId = _terminalId;
            terminalConfig.Port = ConfigurationManager.AppSettings["COMPort"];

            bool result = ecrService.ResetDevice(terminalConfig);
            Console.WriteLine(String.Format("Device Reset : {0}", result));
        }

        private void GetReport(PS_Terminal_Link_Bridge ecrService)
        {
            ReportIn reportIn = new ReportIn(); 
            ReportOut reportOut = null;

            Console.WriteLine("Specify Report type:");
            Console.WriteLine("0 - Banking");
            Console.WriteLine("1 - EOD (End of Day)");
            Console.WriteLine("2 - X Balance");
            Console.WriteLine("3 - Z Balance");
            string keyedType = Console.ReadLine();

            switch (keyedType)
            {
                case "0":
                    reportIn.TypeRequest = ReportType.Banking;
                    break;
                case "1":
                    reportIn.TypeRequest = ReportType.Eod;
                    break;
                case "2":
                    reportIn.TypeRequest = ReportType.Xbal;
                    break;
                case "3":
                    reportIn.TypeRequest = ReportType.Zbal;
                    break;
                default:
                    reportIn.TypeRequest = ReportType.Eod;
                    break;
            }

            reportIn.TerminalId = _terminalId;
            RequestStatus result = ecrService.GetReport(reportIn, out reportOut);
            Console.WriteLine(String.Format("Get Report : {0}", result));
        }

        private void CancelTransaction(PS_Terminal_Link_Bridge ecrService)
        {
            Stopwatch sw = new Stopwatch();

            int errors = 0;

            for (int i = 0; i <= 1000; i++)
            {
                sw.Start();
                int transId = _transCounter++;
                Console.WriteLine("Transaction Number: " + transId.ToString());

                var task = Task.Factory.StartNew(() =>
                {
                    TransactionOut transactionResponse;
                    RequestStatus rStatus = ecrService.PerformTransaction(new TransactionIn()
                    {
                        TerminalId = _terminalId,
                        TransactionID = transId.ToString(),
                        TypeRequest = TransactionType.Sale,
                        Currency = "826",
                        ValueIn = new ValueIn()
                        {
                            Value1 = 4500,
                        }
                    }, out transactionResponse);

                    if (rStatus != RequestStatus.RequestSent)
                    {
                        errors++;
                    }
                });

                while (this._callback.LastStatus != DeviceStatus.InsertCard)
                {
                    Thread.Sleep(5);
                }

                //Console.WriteLine("Press any key to send the cancel");
                //Console.ReadKey();
            RetryAfterConnectionFail:
                bool cancelled = false;
                RequestStatus rStatus2 = ecrService.CancelTransaction(new CancelTransactionRequest() { TerminalId = _terminalId, TransactionID = transId }, out cancelled);
                if (rStatus2 != RequestStatus.RequestSent)
                {
                    errors++;
                }

                Console.WriteLine(rStatus2.ToString());

                if(rStatus2 == RequestStatus.ConnectionFail)
                {
                    goto RetryAfterConnectionFail;
                }

                Task.WaitAll(task);

                this._callback.LastStatus = DeviceStatus.Cancelled;
                sw.Stop();
                Console.WriteLine(string.Format("Test #{0} took {1} seconds", i, sw.Elapsed.ToString()));
                Console.WriteLine(string.Format("Total tests {0} with total errors {1}", i, errors));


                sw.Reset();
            }
        }

        private static void GetDeviceStatus(PS_Terminal_Link_Bridge service)
        {
            DeviceStatus status = DeviceStatus.Unknown;

            RequestStatus rStatus = service.GetCurrentStatus(new GetCurrentStatusRequest() { TerminalId = _terminalId }, out status);

            Console.WriteLine(status.ToString());
        }

        private static void CheckEligibility(PS_Terminal_Link_Bridge service)
        {
            TransactionDetails transactionDetails;
            TransactionType type = GetTransactionType(false);
            CheckTransactionRequest req = new CheckTransactionRequest();
            RequestStatus rStatus = service.CheckTransactionType(new CheckTransactionRequest() { TerminalId = _terminalId, TransactionType = type }, out transactionDetails);
            Console.WriteLine("RequestStatus: " + rStatus.ToString());
            if (transactionDetails != null)
            {
                Console.WriteLine("ReturnCashStatus: " + transactionDetails.ReturnCashStatus);
                Console.WriteLine("Status: " + transactionDetails.Status);
                Console.WriteLine("CashMax: " + transactionDetails.CashMax);
                Console.WriteLine("Max: " + transactionDetails.Max);
                Console.WriteLine("MediumOfExchangeCode: " + transactionDetails.MediumOfExchangeCode);
            }

            Console.WriteLine();
            Console.WriteLine("Calling PerformTransaction...");
            DoTransaction(service);
        }

        private static void DoTransaction(PS_Terminal_Link_Bridge service)
        {
            TransactionOut transactionResponse;

            Console.WriteLine("Please enter a transaction amount in lowest denomination (in pence/pennies).");
            string keyedAmount = Console.ReadLine();
            TransactionType type = GetTransactionType(true);

            string pwcbAmount = "0";
            if (type == TransactionType.PWCB)
            {
                Console.WriteLine("Please enter a PWCB amount in lowest denomination (in pence/pennies).");
                pwcbAmount = Console.ReadLine();                
            }

            if (!(type == TransactionType.Reversal))
            {
                _transCounter++;
            }

            Console.WriteLine("Transaction Number: " + _transCounter.ToString());

            RequestStatus rStatus = service.PerformTransaction(new TransactionIn()
            {
                TerminalId = _terminalId,
                TransactionID = _transCounter.ToString(),
                TypeRequest = type,
                Currency = "826",
                ValueIn = new ValueIn()
                {
                    Value1 = Int32.Parse(Regex.Replace(keyedAmount, "[^0-9]", "")),
                    Value2 = Int32.Parse(Regex.Replace(pwcbAmount, "[^0-9]", "")),
                },
            }, out transactionResponse);

            Console.WriteLine("RequestStatus: " + rStatus.ToString());

            if (transactionResponse != null)
            {
                Console.WriteLine("Status: " + transactionResponse.TransactionResponse.ToString());
                if (transactionResponse.Amount > 0)
                {
                    Console.WriteLine("Amount: " + transactionResponse.Amount.ToString());
                }
                if (transactionResponse.AuthCode != null)
                {
                    Console.WriteLine("AuthCode: " + transactionResponse.AuthCode.ToString());
                }
                if (transactionResponse.AmountOfCash > 0)
                {
                    Console.WriteLine("Amount of Cash: " + transactionResponse.AmountOfCash.ToString());
                }
                if (transactionResponse.AmountOfGratuity > 0)
                {
                    Console.WriteLine("Amount of Gratuity: " + transactionResponse.AmountOfGratuity.ToString());
                }
                if (transactionResponse.TotalAmount > 0)
                {
                    Console.WriteLine("Total Amount: " + transactionResponse.TotalAmount.ToString());
                }
                Console.WriteLine("DateOfStart: " + transactionResponse.DateOfStart);
                Console.WriteLine("MerchantID: " + transactionResponse.MerchantID);
                Console.WriteLine("PrimaryAccountNumber: " + transactionResponse.PrimaryAccountNumber);
                Console.WriteLine("PrimaryAccountNumberSequence: " + transactionResponse.PrimaryAccountNumberSequence);
                Console.WriteLine("TransactionDate: " + transactionResponse.TransactionDate.ToString());
                Console.WriteLine("InputType: " + transactionResponse.InputType.ToString());
                Console.WriteLine("NameOfCardScheme: " + transactionResponse.NameOfCardScheme);
                Console.WriteLine("DateOfExpiration: " + transactionResponse.DateOfExpiration);
                Console.WriteLine("NameOfMerchant: " + transactionResponse.NameOfMerchant);
                Console.WriteLine("ReceiptID: " + transactionResponse.ReceiptID);
                Console.WriteLine("TerminalID: " + transactionResponse.TerminalID);
                Console.WriteLine("CardVerificationMethod: " + transactionResponse.CardVerificationMethod.ToString());
                Console.WriteLine("RerferenceReceipt: " + transactionResponse.ReferenceReceipt);

                if (transactionResponse.DiagnosticCodes != 0)
                {
                    //foreach (var item in transactionResponse.DiagnosticCodes)
                    //{
                    //    Console.WriteLine("Diagcode: " + item.ToString());
                    //}
                }
            }
            else
            {
                Console.WriteLine("transactionResponse is NULL");
            }
        }

        private static void GetLastTransaction(IPS_Terminal_Link_Bridge ecrService)
        {
            TransactionInOut transactionResponse;
            GetLastTransactionRequest request = new GetLastTransactionRequest();
            RequestStatus rStatus = ecrService.GetLastTransactionDetails(new GetLastTransactionRequest() { TerminalId = _terminalId }, out transactionResponse);

            Console.WriteLine("RequestStatus: " + rStatus.ToString());

            if (transactionResponse != null)
            {
                Console.WriteLine("Status: " + transactionResponse.TransactionResponse.ToString());
                Console.WriteLine("Amount: " + transactionResponse.Amount.ToString());
                if (transactionResponse.AuthCode != null)
                {
                    Console.WriteLine("AuthCode: " + transactionResponse.AuthCode.ToString());
                }
                if (transactionResponse.AmountOfCash > 0)
                {
                    Console.WriteLine("Amount of Cash: " + transactionResponse.AmountOfCash.ToString());
                }
                if (transactionResponse.AmountOfGratuity > 0)
                {
                    Console.WriteLine("Amount of Gratuity: " + transactionResponse.AmountOfGratuity.ToString());
                }
                if (transactionResponse.TotalAmount > 0)
                {
                    Console.WriteLine("Total Amount: " + transactionResponse.TotalAmount.ToString());
                }
                Console.WriteLine("DateOfStart: " + transactionResponse.DateOfStart);
                Console.WriteLine("MerchantID: " + transactionResponse.MerchantID);
                Console.WriteLine("PrimaryAccountNumber: " + transactionResponse.PrimaryAccountNumber);
                Console.WriteLine("PrimaryAccountNumberSequence: " + transactionResponse.PrimaryAccountNumberSequence);
                Console.WriteLine("TransactionDate: " + transactionResponse.TransactionDate.ToString());
                Console.WriteLine("InputType: " + transactionResponse.InputType.ToString());
                Console.WriteLine("NameOfCardScheme: " + transactionResponse.NameOfCardScheme);
                Console.WriteLine("DateOfExpiration: " + transactionResponse.DateOfExpiration);
                Console.WriteLine("NameOfMerchant: " + transactionResponse.NameOfMerchant);
                Console.WriteLine("ReceiptID: " + transactionResponse.ReceiptID);
                Console.WriteLine("TerminalID: " + transactionResponse.TerminalID);
                Console.WriteLine("CardVerificationMethod: " + transactionResponse.CardVerificationMethod.ToString());
            }
            else
            {
                Console.WriteLine("transactionResponse is NULL");
            }
        }

        private static void ConfigurePrint(IPS_Terminal_Link_Bridge service, bool isEnabled)
        {
            RequestStatus rStatus = service.ConfigurePrint(new ConfigurePrintRequest()
            {
                TerminalId = _terminalId,
                IsEnabled = isEnabled
            });
            Console.WriteLine("RequestStatus: " + rStatus.ToString());
        }

        private static TransactionType GetTransactionType(bool isDoTransaction)
        {
            TransactionType type;

            Console.WriteLine("Specify transaction type:");
            Console.WriteLine("0 - Sale");
            Console.WriteLine("1 - Refund");
            Console.WriteLine("2 - Void");
            Console.WriteLine("3 - Duplicate");
            Console.WriteLine("4 - Cash Advance");
            Console.WriteLine("5 - PWCB");
            Console.WriteLine("6 - Pre-Auth");
            Console.WriteLine("7 - Completion");
            Console.WriteLine("8 - Verify Cardholder");
            Console.WriteLine("9 - Verify Account");
            if (isDoTransaction)
            {
                Console.WriteLine("10 - Reversal");
            }
            string keyedType = Console.ReadLine();

            switch (keyedType)
            {
                case "0":
                    type = TransactionType.Sale;
                    break;
                case "1":
                    type = TransactionType.Refund;
                    break;
                case "2":
                    type = TransactionType.Void;
                    break;
                case "3":
                    type = TransactionType.Duplicate;
                    break;
                case "4":
                    type = TransactionType.CashAdvance;
                    break;
                case "5":
                    type = TransactionType.PWCB;
                    break;
                case "6":
                    type = TransactionType.PreAuth;
                    break;
                case "7":
                    type = TransactionType.Completion;
                    break;
                case "8":
                    type = TransactionType.VerifyCardHolder;
                    break;
                case "9":
                    type = TransactionType.VerifyAccount;
                    break;
                case "10":
                    if (isDoTransaction)
                    {
                        type = TransactionType.Reversal;
                    }
                    else
                    {
                        type = TransactionType.PreAuth;
                    }
                    break;
                default:
                    type = TransactionType.PreAuth;
                    break;
            }

            return type;
        }

        private static void SendPrintRequest(IPS_Terminal_Link_Bridge ecrService)
        {
            SendPrintRequest request = new SendPrintRequest();
            bool status;

            request.Buffer = "Testing SendPrintRequest";
            request.TerminalId = _terminalId;
            RequestStatus rStatus = ecrService.SendPrintRequest(request, out status);

            Console.WriteLine("RequestStatus: " + rStatus.ToString());
        }

        private static void Subscribe(PS_Terminal_Link_Bridge service)
        {
            SerialPortConfiguration terminalConfig = new SerialPortConfiguration();

            terminalConfig.TerminalId = _terminalId;
            terminalConfig.Port = ConfigurationManager.AppSettings["COMPort"];

            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            appConfig.TerminalBrandType = (TerminalBrandType)Enum.Parse(typeof(TerminalBrandType), ConfigurationManager.AppSettings["TerminalModelType"], true);
            _terminalBrandType = appConfig.TerminalBrandType;
            appConfig.LogLevel = (Level)Enum.Parse(typeof(Level), ConfigurationManager.AppSettings["LogLevel"], true);
            appConfig.PathToLog = ConfigurationManager.AppSettings["PathToLog"];
            appConfig.PingClientInterval = Int32.Parse(ConfigurationManager.AppSettings["PingIntervalSeconds"]);

            SubscriptionResultTypes result = service.Subscribe(new AuthenticationDetails() { SerialNumber = _terminalId }, terminalConfig, appConfig);

            Console.WriteLine("IsAuthenticated - " + result.ToString());
        }

        private static void Unsubscribe(PS_Terminal_Link_Bridge service)
        {
            UnsubscribeResultTypes result = service.Unsubscribe();

            Console.WriteLine("Unsubscribe Result - " + result.ToString());

            Thread.Sleep(2000);
        }
    }
}
