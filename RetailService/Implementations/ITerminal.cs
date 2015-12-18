using PS.Terminal.Link.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailService.Implementations
{
    abstract class Terminal
    {

        public String terminalID { get; set; }
        public String terminalPort { get; set; }
        public String terminalModelType { get; set; }
        public PS_Terminal_Link_Bridge service { get; set; }
        public abstract void subscribe();
        public abstract void unsubscribe();

    }
}
