using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.Bridge.Service;
using System.Configuration;
using implementations.common.log;

namespace implementations
{
    public class RetailTerminalCallback : PS_Terminal_Link_Callback
    {
        private static string _terminalId;
        public DeviceStatus LastStatus { get; set; }

        public RetailTerminalCallback()
        {
            _terminalId = ConfigurationManager.AppSettings["TerminalId"];
        }

        public override SetSignatureState VerifySignature(SignatureDetails signatureDetails)
        {
            SignatureState type = SignatureState.NotAccepted;

            FormLogUtils.getInstance().info("Specify signature verification status:");
            FormLogUtils.getInstance().info("0 - Accepted");
            FormLogUtils.getInstance().info("1 - Cancelled");
            FormLogUtils.getInstance().info("2 - NotAccepted");

            string keyedType = "0";

            switch (keyedType)
            {
                case "0":
                    type = SignatureState.Accepted;
                    break;
                case "1":
                    type = SignatureState.Cancelled;
                    break;
                case "2":
                    type = SignatureState.NotAccepted;
                    break;
            }

            FormLogUtils.getInstance().info("Call SetSignatureState...");
            return new SetSignatureState() { State = type };
        }

        public override SetVoiceState VerifyVoiceService(VoiceServiceDetails voiceDetails)
        {
            VoiceState type = VoiceState.Declined;
            FormLogUtils.getInstance().info("Specify transaction code:");
            string transCode = Console.ReadLine();

            FormLogUtils.getInstance().info("Specify voice verification status:");
            FormLogUtils.getInstance().info("0 - Accepted");
            FormLogUtils.getInstance().info("1 - Cancelled");
            FormLogUtils.getInstance().info("2 - Declined");

            string keyedType = Console.ReadLine();

            switch (keyedType)
            {
                case "0":
                    type = VoiceState.Accepted;
                    break;
                case "1":
                    type = VoiceState.Cancelled;
                    break;
                case "2":
                    type = VoiceState.Declined;
                    break;
            }

            return new SetVoiceState() { State = type, Code = transCode };
        }

        public override void ReceivePrintRequest(PrintIn request)
        {
            //FormLogUtils.getInstance().info(@"Writing print request to C:\DoPrint.txt");
            System.IO.StreamWriter writer = System.IO.File.AppendText(@"C:\DoPrint.txt");

            foreach (var receipt in request.Receipts)
            {
                foreach (var line in receipt.Lines)
                {
                    writer.WriteLine(line.Text);
                }
            }

            writer.Close();
        }

        public override void TerminalStatusReceived(DeviceStatus status)
        {
            this.LastStatus = status;
            //FormLogUtils.getInstance().info(String.Format("Terminal Status callback - {0}", status.ToString()));
        }

        public override void SubscribeStatus(string comPort)
        {
            //FormLogUtils.getInstance().info(String.Format("Terminal SubscribeStatus callback - Detecting Paymentsense terminal on {0} Port. Please wait...", comPort));
        }
    }
}
