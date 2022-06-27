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
        public List<TaskMetrics> Metrics { get; set; }
        public GlobalTime GTime;

        public Schedule()
        {
            Tasks = new List<PeriodicTask>();
            CompletedTasks = new List<PeriodicTask>();
            Slices = new List<TimeSlice>();
            TQueue = new TaskQueue();
            Metrics = new List<TaskMetrics>();
            GTime = new GlobalTime();
        }

        public void AddTask(int uID, int period, int executionTime)
        {
            Tasks.Add(new PeriodicTask(uID, period, executionTime, ref GTime));
            Metrics.Add(new TaskMetrics(uID));
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
                else if(GTime.Time % x.Period == 0)
                {
                    var metric = Metrics.Single(y => y.UID == x.UID);
                    metric.TasksDropped += x.ExecutionTime;
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
                    var metric = Metrics.Single(x => x.UID == task.UID);
                    metric.ResponseTimeCounter++;
                    metric.ResponseTimeSum += task.ResponseTime;
                    metric.Penalty += task.Penalty;
                    metric.Reward += task.Reward;
                    metric.TimesExecuted += task.TimesExecuted;

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

        public void IOTaskMetrics(string filename)
        {
            File.AppendAllText(filename, $"{Util.nwl}{Util.sp} Task Metrics {Util.sp}{Util.nwl}");
            foreach (var task in Tasks)
            {
                var metric = Metrics.Single(x => x.UID == task.UID);
                File.AppendAllText(filename, $"The task {metric.UID} has been executed {metric.TimesExecuted} times, has a total penalty of {metric.Penalty}, a total reward of {metric.Reward}, an average response time of {metric.AverageResponseTime}, and in total it has been dropped for {metric.TasksDropped} time slices{Util.nwl}");
            }
            File.AppendAllText(filename, $"The system of length {Metrics.Sum(x => x.TimesExecuted)} has a total penalty of {Metrics.Sum(x => x.Penalty)}, a total reward of {Metrics.Sum(x => x.Reward)}, an average response time of {Metrics.Sum(x => x.AverageResponseTime) /(double)Metrics.Count}, and in total {Metrics.Sum(x => x.TasksDropped)} time slices of tasks has been dropped{Util.nwl}");
        }
    }
}
