using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutdownScheduler
{
    public class ScheduleConfig
    {
        public string Daily { get; set; }
        public List<WeeklySchedule> Weekly { get; set; } = new();
        public List<OneTimeSchedule> OneTime { get; set; } = new();
    }

    public class WeeklySchedule
    {
        public string Day { get; set; }               // "MON", "TUE", ...
        public string Time { get; set; }              // "18:00"
    }

    public class OneTimeSchedule
    {
        public string Date { get; set; }              // "2025-09-20"
        public string Time { get; set; }              // "21:00"
    }

}
