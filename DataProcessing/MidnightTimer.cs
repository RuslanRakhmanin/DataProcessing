using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DataProcessing
{
    public class MidnightTimer
    {
        private System.Timers.Timer timer;
        private LinesCounter counter;

        public MidnightTimer(LinesCounter counter)
        {
            this.counter = counter;
            Start();

        }

        public void Start()
        {
            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = DateTime.Today.AddDays(1);

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            //tickTime = 2000;
            timer = new System.Timers.Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            timer.Stop();
            counter.LogToFile();
            counter.Reset();
            Start();
        }

        public void Stop()
        {
            timer?.Stop();
        }

    }
}
