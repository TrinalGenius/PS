using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sven.common.task
{
    abstract class TaskController
    {
        public int count { get; set;}
        public int current { get; set; }
        public abstract void runScheduler();
    }
}
