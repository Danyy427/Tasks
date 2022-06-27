using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class TaskMetrics
    {
        public int UID { get; set; }
        public int Penalty { get; set; }
        public int Reward { get; set; }
        public double AverageResponseTime => ResponseTimeSum / (double)ResponseTimeCounter;
        public int ResponseTimeCounter { get; set; }
        public int ResponseTimeSum { get; set; }
        public int TasksDropped { get; set; }
        public int TimesExecuted { get; set; }

        public TaskMetrics(int uID)
        {
            UID = uID;
        }
    }
}
