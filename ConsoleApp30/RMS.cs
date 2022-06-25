﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class RMS
    {
        public Schedule TSchedule { get; set; }

        public RMS()
        {
            TSchedule = new Schedule();
        }

        public void AddTask(int uID, int period, int executionTime)
        {
            TSchedule.AddTask(uID, period, executionTime);
        }

        public void CreateSchedule()
        {
            string output = "./RMS.txt";
            File.Delete(output);
            TSchedule.IOTaskInfo(output);

            long length = Util.LCM(TSchedule.Tasks.Select(x => x.Period).ToArray());
            

            for (long i = 0; i < length; i++)
            {
                TSchedule.TickWithAction(x => x.Priority = x.Period);
                TSchedule.CompletedTasks.ForEach(x => TSchedule.TQueue.RemoveTask(x));
                TSchedule.Execute();
            }

            // Order by ID so the outputs are in the right order.
            TSchedule.Tasks = TSchedule.Tasks.OrderBy(x => x.UID).ToList();

            // Output the schedule time diagram
            TSchedule.IOScheduleDiagram(output);

            // Output the schedule 
            TSchedule.IOSchedule(output);
        }
    }
}
