using sven.common.log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sven.common.task
{
    class SimpleTaskScheduler : Queue
    {

        public ITaskLoader taskLoader { get; set; }
        public int start { get; set; }
        public int count { get; set; }
        public int delayTask { get; set; }


        public void loadAllTasks()
        {
            if (taskLoader != null)
            {
                foreach (ITask task in taskLoader.loadAllTasks())
                {
                    this.Enqueue(task);
                }
            }
        }

        

        public void loadCurrTask() 
        {

            FormLogUtils.getInstance().info(String.Format("loadCurrTask->  from {0} load total {1} tasks", start, count));
            if (taskLoader != null)
            {

                foreach (ITask task in taskLoader.loadCurrTasks(start, count))
                {
                    this.Enqueue(task);
                    
                    
                }

            }
        }
    }
}
