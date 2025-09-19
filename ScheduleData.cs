using System;
using System.Collections.Generic;

namespace ShutdownScheduler
{
    /// <summary>
    /// Root object for storing all scheduled shutdown configurations.
    /// </summary>
    public class ScheduleData
    {
        public List<ScheduleItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Represents a single shutdown schedule item.
    /// </summary>
    public class ScheduleItem
    {
        /// <summary>
        /// Internal name for the task (e.g., ShutdownScheduler_MON_2200).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of schedule: Daily, Weekly, or OneTime.
        /// </summary>
        public ScheduleType Type { get; set; }

        /// <summary>
        /// Day of the week (used for Weekly schedules).
        /// Example: "MON".
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Specific date (used for OneTime schedules).
        /// Example: "2025-09-18".
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Scheduled time of day in HH:mm format.
        /// Example: "22:00".
        /// </summary>
        public string Time { get; set; }
    }

    /// <summary>
    /// Enum for supported schedule types.
    /// </summary>
    public enum ScheduleType
    {
        Daily,
        Weekly,
        OneTime
    }
}
