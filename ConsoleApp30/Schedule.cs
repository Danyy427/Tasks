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
        public List<TimeSlice> Slices { get; set; }
        public TaskQueue TQueue { get; set; }
        public GlobalTime GTime;

        public Schedule()
        {
            Tasks = new List<PeriodicTask>();
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

        public void Execute()
        {
            TQueue.Tasks = TQueue.Tasks.OrderBy(x => x.Priority).ThenBy(x => x.TimeCreated).ToList();

            PeriodicTask task;
            try
            {
                task = TQueue.Tasks.First(x => x.IsCompleted == false && x.Lock == 0);
                task.Execute();
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
    }
}
