using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal class GlobalTime
    {
        public int Time { get; set; }

        public GlobalTime()
        {
            Time = 0;
        }

        public void Tick()
        {
            Time++;
        }
    }
}
