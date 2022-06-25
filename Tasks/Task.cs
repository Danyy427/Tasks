using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class PeriodicTask
    {
        private static GlobalTime nanTime = new GlobalTime();
        public static PeriodicTask NaN = new PeriodicTask(0, 0, 0, ref nanTime);

        private GlobalTime _time;
        private int _lock;

        // Constant properties
        public int UID { get; set; }
        public int Period { get; set; }
        public int ExecutionTime { get; set; }
        public int TimeCreated { get; set; }

        // Properties
        public int RemainingTime { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
        public int Penalty { get; set; }
        public int TimeDispatched { get; set; }
        public int TimesExecuted { get; set; }
        public int TimeCompleted { get; set; }
        public int TTD => (Period + TimeCreated) - _time.Time;
        public int Deadline { get; set; }
        public bool IsDispatched { get; set; }

        // Oscilators
        public int ResponseTime { get; set; }
        public int PeriodOsc => (_time.Time - TimeDispatched) % Period;

        // Lock
        public int Lock => _lock;

        public PeriodicTask(int uID, int period, int executionTime, ref GlobalTime time)
        {
            UID = uID;
            Period = period;
            ExecutionTime = executionTime;
            _time = time;
            TimeCreated = _time.Time;
            Deadline = TimeCreated + Period;
            ResetTask();
        }

        public void ResetTask()
        {
            if (_lock == 1) throw new Exception("The task is done and therefore locked");
            _lock = 0;

            RemainingTime = 0;
            IsCompleted = false;
            Priority = 0;
            Penalty = 0;
            TimeDispatched = 0;
            TimesExecuted = 0;
            TimeCompleted = 0;
            IsDispatched = false;

            ResponseTime = 0;
        }

        public void Dispatch()
        {
            if (_lock == 1) throw new Exception("The task is done and therefore locked");
            ResetTask();
            RemainingTime = ExecutionTime;
            TimeDispatched = _time.Time;
            IsDispatched = true;
        }

        public void Execute()
        {
            if (_lock == 1) throw new Exception("The task is done and therefore locked");
            RemainingTime--;
            TimesExecuted++;

            if(RemainingTime == 0)
            {
                TimeCompleted = _time.Time;
                IsCompleted = true;
                ResponseTime = _time.Time - TimeCreated;
                _lock = 1;
            }
        }

        public void Drop()
        {
            IsCompleted = true;
            _lock = 1;
        }

        public PeriodicTask Clone()
        {
            return new PeriodicTask(UID, Period, ExecutionTime, ref _time);
        }
    }
}
