using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            RMS rms = new RMS();
            rms.AddTask(1, 5, 2);
            rms.AddTask(2, 6, 2);
            rms.AddTask(3, 7, 2);
            rms.AddTask(4, 8, 2);
            rms.CreateSchedule();

            EDF edf = new EDF();
            edf.AddTask(1, 5, 2);
            edf.AddTask(2, 6, 2);
            edf.AddTask(3, 7, 2);
            edf.AddTask(4, 8, 2);
            edf.CreateSchedule();

            LLF llf = new LLF();
            llf.AddTask(1, 5, 2);
            llf.AddTask(2, 6, 2);
            llf.AddTask(3, 7, 2);
            llf.AddTask(4, 8, 2);
            llf.CreateSchedule();
        }
    }
}
