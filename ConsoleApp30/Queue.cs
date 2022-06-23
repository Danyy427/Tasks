﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class TaskQueue
    {
        public List<PeriodicTask> Tasks { get; set; }

        public TaskQueue()
        {
            Tasks = new List<PeriodicTask>();
        }

        public void Dispatch(PeriodicTask task)
        {
            var t = task.Clone();
            t.Dispatch();
            Tasks.Add(t);
        }

        public void Drop()
        {
            Tasks.RemoveAt(0);
        }
    }
}