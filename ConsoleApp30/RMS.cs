using System;
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
            IOTaskInfo(output);

            long length = Util.LCM(TSchedule.Tasks.Select(x => x.Period).ToArray());
            

            for (long i = 0; i < length; i++)
            {
                TSchedule.Tick();
                TSchedule.TQueue.Tasks.ForEach(x => x.Priority = x.Period);
                TSchedule.Execute();
            }

            // Order by ID so the outputs are in the right order.
            TSchedule.Tasks = TSchedule.Tasks.OrderBy(x => x.UID).ToList();

            // Output the schedule time diagram
            File.AppendAllText(output, $"{Util.nwl}{Util.sp} Time Diagram {Util.sp}{Util.nwl}");
            IOScheduleDiagram(output);

            // Output the schedule 
            File.AppendAllText(output, $"{Util.nwl}{Util.sp} Schedule {Util.sp}{Util.nwl}");
            IOSchedule(output);
        }

        public void IOSchedule(string filename)
        {
            foreach (var timeSlice in TSchedule.Slices)
            {
                File.AppendAllText(filename, $"At time {timeSlice.Time}, the task {timeSlice.task.UID} is executed.{Util.nwl}");
            }
        }

        public void IOScheduleDiagram(string filename)
        {
            foreach (var task in TSchedule.Tasks)
            {
                string str = "";
                for (int i = 0; i < TSchedule.Slices.Count; i++)
                {
                    if (i % task.Period == 0) str += "|";
                    TSchedule.Tasks.Where(x => x != task).ToList().ForEach(x =>
                    {
                        if (i % x.Period == 0)
                        {
                            str += " ";
                        }
                    });
                    str += TSchedule.Slices[i].task.UID == task.UID ? $" {task.UID}  " : $" {"_".Multiply(2)} ";

                }
                File.AppendAllText(filename, $"{str}{Util.nwl}");
            }
        }

        public void IOTaskInfo(string filename)
        {
            double TUtilization = 0;
            foreach (var task in TSchedule.Tasks)
            {
                File.AppendAllText(filename, $"The task {task.UID} has a period of {task.Period} and an execution time of {task.ExecutionTime}{Util.nwl}");
                TUtilization += (double)task.ExecutionTime / (double)task.Period;
            }
            File.AppendAllText(filename, $"For the given tasks the total CPU Utilization is {TUtilization * 100}%{Util.nwl}");
        }
    }
}
