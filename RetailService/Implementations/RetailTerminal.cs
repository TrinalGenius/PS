using PS.Terminal.Link.Bridge.Service;
using implementations.common.log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PS.Terminal.Link.Bridge;

namespace RetailService.Implementations
{
    class RetailTerminal : Terminal
    {

        /* Returns the current status of the PDQ device. */
        public DeviceStatus GetDeviceStatus()
        {
            DeviceStatus status = DeviceStatus.Unknown;

            RequestStatus rStatus = service.GetCurrentStatus(new GetCurrentStatusRequest() { TerminalId = terminalID }, out status);

            Console.WriteLine(status.ToString());

            return status;
        }


        public override void subscribe()
        {
            FormLogUtils.getInstance().info("......");
            SerialPortConfiguration terminalConfig = new SerialPortConfiguration()
            {
                TerminalId = terminalID,
                Port = terminalPort
            };

            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            appConfig.TerminalBrandType = (TerminalBrandType)Enum.Parse(typeof(TerminalBrandType), terminalModelType, true);

            appConfig.LogLevel = (Level)Enum.Parse(typeof(Level), ConfigurationManager.AppSettings["LogLevel"], true);
            appConfig.PathToLog = ConfigurationManager.AppSettings["PathToLog"];

            SubscriptionResultTypes result = service.Subscribe(new AuthenticationDetails() { SerialNumber = terminalID }, terminalConfig, appConfig);

            FormLogUtils.getInstance().info(terminalID + " IsAuthenticated - " + result.ToString());
        }


        public override void unsubscribe()
        {
            UnsubscribeResultTypes result = service.Unsubscribe();

            FormLogUtils.getInstance().info("Unsubscribe Result - " + result.ToString());

            Thread.Sleep(2000);
        }

        public TransactionOut doTransaction(int amount, int transSequence)
        {

            FormLogUtils.getInstance().info(String.Format("Transaction->{0}.amount: " + amount, transSequence));

            TransactionOut transactionResponse;

            
            TransactionType type = TransactionType.Sale;

            string pwcbAmount = "0";

            RequestStatus rStatus = service.PerformTransaction(new TransactionIn()
            {
                TerminalId = terminalID,
                TransactionID = Convert.ToString(transSequence),
                TypeRequest = type,
                Currency = "826",
                ValueIn = new ValueIn()
                {
                    Value1 = amount,
                    Value2 = Int32.Parse(Regex.Replace(pwcbAmount, "[^0-9]", "")),
                },
            }, out transactionResponse);

            FormLogUtils.getInstance().info("RequestStatus: " + rStatus.ToString());

            if (transactionResponse != null)
            {
                FormLogUtils.getInstance().info("Status: " + transactionResponse.TransactionResponse.ToString());
                if (transactionResponse.Amount > 0)
                {
                    FormLogUtils.getInstance().info("Amount: " + transactionResponse.Amount.ToString());
                }
                if (transactionResponse.AuthCode != null)
                {
                    FormLogUtils.getInstance().info("AuthCode: " + transactionResponse.AuthCode.ToString());
                }
                if (transactionResponse.AmountOfCash > 0)
                {
                    FormLogUtils.getInstance().info("Amount of Cash: " + transactionResponse.AmountOfCash.ToString());
                }
                if (transactionResponse.AmountOfGratuity > 0)
                {
                    FormLogUtils.getInstance().info("Amount of Gratuity: " + transactionResponse.AmountOfGratuity.ToString());
                }
                if (transactionResponse.TotalAmount > 0)
                {
                    FormLogUtils.getInstance().info("Total Amount: " + transactionResponse.TotalAmount.ToString());
                }
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.DateOfStart: " + transactionResponse.DateOfStart, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.MerchantID: " + transactionResponse.MerchantID, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.PrimaryAccountNumber: " + transactionResponse.PrimaryAccountNumber, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.PrimaryAccountNumberSequence: " + transactionResponse.PrimaryAccountNumberSequence, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.TransactionDate: " + transactionResponse.TransactionDate.ToString(), transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.InputType: " + transactionResponse.InputType.ToString(), transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.NameOfCardScheme: " + transactionResponse.NameOfCardScheme, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.DateOfExpiration: " + transactionResponse.DateOfExpiration, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.NameOfMerchant: " + transactionResponse.NameOfMerchant, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.ReceiptID: " + transactionResponse.ReceiptID, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.TerminalID: " + transactionResponse.TerminalID, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.CardVerificationMethod: " + transactionResponse.CardVerificationMethod.ToString(), transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.RerferenceReceipt: " + transactionResponse.ReferenceReceipt, transSequence));
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.GemsNumber: " + transactionResponse.GemsNumber, transSequence));
                
                if (transactionResponse.DiagnosticCodes != 0)
                {
                    //foreach (var item in transactionResponse.DiagnosticCodes)
                    //{
                    //    FormLogUtils.getInstance().info("Diagcode: " + item.ToString());
                    //}
                }
            }
            else
            {
                FormLogUtils.getInstance().info(String.Format("Transaction->{0}.transactionResponse is NULL", transSequence));
            }

            return transactionResponse;
        }
    }
}
