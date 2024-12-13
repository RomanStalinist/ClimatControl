using System;
using System.Collections.Generic;

namespace DemoExam.Models
{
    public class Statistics
    {
        public int CompletedRequestsCount { get; set; }
        public TimeSpan AverageCompletionTime { get; set; }
        public Dictionary<string, int> FaultTypeStatistics { get; set; } = new Dictionary<string, int>();
    }
}
