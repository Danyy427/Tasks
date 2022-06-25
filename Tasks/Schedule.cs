using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class TimeSlice
    {
        public PeriodicTask task { get; set; }
        public int Time { get; set; }
        public TimeSlice(PeriodicTask task, int time)
        {
            this.task = task;
            Time = time;
        }
    }

    internal class Schedule
    {
        public List<PeriodicTask> Tasks { get; set; }
        public List<PeriodicTask> CompletedTasks { get; set; }

        public List<TimeSlice> Slices { get; set; }
        public TaskQueue TQueue { get; set; }
        public GlobalTime GTime;

        public Schedule()
        {
            Tasks = new List<PeriodicTask>();
            CompletedTasks = new List<PeriodicTask>();
            Slices = new List<TimeSlice>();
            TQueue = new TaskQueue();
            GTime = new GlobalTime();
        }

        public void AddTask(int uID, int period, int executionTime)
        {
            Tasks.Add(new PeriodicTask(uID, period, executionTime, ref GTime));
        }

        public void Tick()
        {
            Tasks.ForEach(x =>
            {
                if(GTime.Time % x.Period == 0)
                {
                    TQueue.Dispatch(x);
                }
            });

            GTime.Tick();
        }

        public void TickWithAction(Action<PeriodicTask> action)
        {
            Tasks.ForEach(x =>
            {
                if (GTime.Time % x.Period == 0 && !(TQueue.Tasks.Where(y => y.UID == x.UID && y.Lock == 0 && y.IsCompleted == false).ToList().Count > 0))
                {
                    TQueue.Dispatch(x);
                }
            });

            TQueue.Tasks.ForEach(action);

            GTime.Tick();
        }

        public void Execute()
        {
            TQueue.Tasks = TQueue.Tasks.OrderBy(x => x.Priority).ToList();  //.ThenBy(x => x.Period).ToList();

            PeriodicTask task;
            try
            {
                task = TQueue.Tasks.First(x => x.IsCompleted == false && x.Lock == 0);
                task.Execute();
                if(task.IsCompleted && task.Lock == 1)
                {
                    CompletedTasks.Add(task);
                    TQueue.Tasks.Remove(task);
                }
            }
            catch
            {
                task = PeriodicTask.NaN;
            }
            Slices.Add(new TimeSlice(task, GTime.Time));
        }

        public void Drop()
        {
            TQueue.Drop();
        }


        public void IOSchedule(string filename)
        {
            File.AppendAllText(filename, $"{Util.nwl}{Util.sp} Schedule {Util.sp}{Util.nwl}");
            foreach (var timeSlice in Slices)
            {
                File.AppendAllText(filename, $"At time {timeSlice.Time}, the task {timeSlice.task.UID} is executed.{Util.nwl}");
            }
        }

        public void IOScheduleDiagram(string filename)
        {
            File.AppendAllText(filename, $"{Util.nwl}{Util.sp} Time Diagram {Util.sp}{Util.nwl}");
            foreach (var task in Tasks)
            {
                string str = "";
                for (int i = 0; i < Slices.Count; i++)
                {
                    if (i % task.Period == 0) str += "|";
                    Tasks.Where(x => x != task).ToList().ForEach(x =>
                    {
                        if (i % x.Period == 0)
                        {
                            str += " ";
                        }
                    });
                    str += Slices[i].task.UID == task.UID ? $" {task.UID}  " : $" {"_".Multiply(2)} ";

                }
                File.AppendAllText(filename, $"{str}{Util.nwl}");
            }
        }

        public void IOTaskInfo(string filename)
        {
            File.AppendAllText(filename, $"{Util.nwl}{Util.sp} Task Info {Util.sp}{Util.nwl}");
            double TUtilization = 0;
            foreach (var task in Tasks)
            {
                File.AppendAllText(filename, $"The task {task.UID} has a period of {task.Period} and an execution time of {task.ExecutionTime}{Util.nwl}");
                TUtilization += (double)task.ExecutionTime / (double)task.Period;
            }
            File.AppendAllText(filename, $"For the given tasks the total CPU Utilization is {TUtilization * 100}%{Util.nwl}");
        }
    }
}
