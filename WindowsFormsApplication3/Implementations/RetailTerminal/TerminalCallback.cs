using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.Bridge.Service;
using System.Configuration;

namespace SamplePointOfSaleClient
{
    public class TerminalCallback : PS_Terminal_Link_Callback
    {
        private static string _terminalId;
        public DeviceStatus LastStatus { get; set; }

        public TerminalCallback()
        {
            _terminalId = ConfigurationManager.AppSettings["TerminalId"];
        }

        public override SetSignatureState VerifySignature(SignatureDetails signatureDetails)
        {
            SignatureState type = SignatureState.NotAccepted;

            Console.WriteLine("Specify signature verification status:");
            Console.WriteLine("0 - Accepted");
            Console.WriteLine("1 - Cancelled");
            Console.WriteLine("2 - NotAccepted");

            string keyedType = Console.ReadLine();

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

            Console.WriteLine("Call SetSignatureState...");
            return new SetSignatureState() { State = type };
        }

        public override SetVoiceState VerifyVoiceService(VoiceServiceDetails voiceDetails)
        {
            VoiceState type = VoiceState.Declined;
            Console.WriteLine("Specify transaction code:");
            string transCode = Console.ReadLine();

            Console.WriteLine("Specify voice verification status:");
            Console.WriteLine("0 - Accepted");
            Console.WriteLine("1 - Cancelled");
            Console.WriteLine("2 - Declined");

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
            Console.WriteLine(@"Writing print request to C:\DoPrint.txt");
            System.IO.StreamWriter writer = System.IO.File.AppendText(@"C:\DoPrint.txt");

            foreach (var receipt in request.Receipts)
            {
                foreach (var line in receipt.Lines)
                {
                    writer.Write(line.Text);
                }
                
            }

            writer.Close();
        }


        public override void TerminalStatusReceived(DeviceStatus status)
        {
            this.LastStatus = status;
            Console.WriteLine(String.Format("Terminal Status callback - {0}", status.ToString()));
        }

        protected override bool PingClient()
        {
            Console.WriteLine("Ping was called at " + DateTime.Now.ToString());
            return true;
        }
    }
}
