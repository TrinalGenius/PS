﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace implementations.common.task
{
    interface ITaskLoader
    {

        List<ITask> loadAllTasks();
        List<ITask> loadCurrTasks(int start, int count);
    }
}
