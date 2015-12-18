using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using PS.Terminal.Link.Bridge;
using PS.Terminal.Link.Bridge.RemoteSettleTerminalService;
using System.Data.SqlClient;
using System.Data;
using Implementations.RemoteSettleTerminal;
using EPOSServer.Implementations;
using SamplePointOfSaleClient.Implementations;


namespace Implementations.RemoteSettleTerminal
{
    public class RemoteSettleServer
    {
        PS_Terminal_Link_RemoteSettleBridge ecrService = null;
        public void Run()
        {
            ecrService = PS_Terminal_Link_RemoteSettleBridge.Instance;

            if ("DatabaseModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
            {

                ecrService.SetCallback(new RemoteSettleTerminalCallback());
            }
            else if ("InterfaceModel".Equals(GlobalProperties.getValueByKey("ServiceModel")))
            {
                ecrService.SetCallback(new TerminalForInterfaceCallback());
            }

            

            FormLogUtils.getInstance().debug("Executing Subscribe.");

            DateTime subscribeTime = DateTime.Now;
            Subscribe(ecrService);
            FormLogUtils.getInstance().debug("That took " + DateTime.Now.Subtract(subscribeTime).ToString());
        }

        public void shutDown()
        {
            if (ecrService != null)
            {
                Unsubscribe(ecrService);
            }
        }

        private static void Subscribe(IPS_Terminal_Link_RemoteSettleBridge service)
        {

            foreach (TCPPortConfiguration terminal in GlobalProperties.terminalsList)
            {
                SubscriptionResultTypes result = service.Subscribe(
                    new AuthenticationDetails(),
                    terminal,
                    new ApplicationConfiguration()
                    {
                        TerminalBrandType = TerminalBrandType.Ingenico_CA_005_RemoteSettle,
                        PingClientInterval = Int32.Parse(ConfigurationManager.AppSettings["PingIntervalSeconds"])
                    }
                );

                FormLogUtils.getInstance().debug("Port:" + terminal.Port + "; Terminalid:" + terminal.TerminalId + " ... " + result.ToString());

            }
            FormLogUtils.getInstance().debug("SubscribeResult End");

                
        }

        private static void Unsubscribe(IPS_Terminal_Link_RemoteSettleBridge service)
        {
            UnsubscribeResultTypes result = service.Unsubscribe();

            FormLogUtils.getInstance().debug("Unsubscribe Result - " + result.ToString());
        }
    }
}
