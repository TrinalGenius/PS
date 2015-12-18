using sven.common.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sven.common.task
{
    class SimpleTaskController : TaskController
    {
        private SimpleTaskScheduler taskScheduler { get; set; }
        public int delayTask { get; set; }
        public Boolean isRun { get; set; }

        public SimpleTaskController(SimpleTaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;

            if (taskScheduler != null)
            {
                
                this.count = taskScheduler.Count;
                this.current = 0;
            }
        }

        public override void runScheduler()
        {

        }

        public void runSchedulerWithCallBack(Delegate process)
        {

            if (taskScheduler != null)
            {

                isRun = true;
                Thread t = new Thread(delegate()
                {


                    while (taskScheduler.Count > 0 && isRun)
                    {

                        ITask task = (ITask)taskScheduler.Dequeue();

                        try
                        {

                            current++;

                            try
                            {
                                task.run();
                            }
                            catch (Exception e)
                            {
                                FormLogUtils.getInstance().error(e.ToString(), e);

                                try
                                {
                                    FormLogUtils.getInstance().info("waiting for retry in 60s");
                                    Thread.Sleep(60 * 1000);
                                    task.run();
                                }
                                catch (Exception f)
                                {
                                    FormLogUtils.getInstance().error(e.ToString(), f);
                                    FormLogUtils.getInstance().info(String.Format("skip task->{0}", task.ToString()));
                                }
                            }

                            //process();

                            Thread.Sleep(delayTask);

                        }
                        catch (Exception e)
                        {
                            FormLogUtils.getInstance().error(e.ToString(), e);
                        }
                    }

                });
                t.IsBackground = true;
                t.Start();

                
            }
        }

        void updateFormStateAfterPerTask()
        {

        }

    }
}
