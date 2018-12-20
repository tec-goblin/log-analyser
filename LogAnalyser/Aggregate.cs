using System;
using System.Collections.Generic;
using System.Linq;

namespace LogAnalyser
{
    internal class Aggregate
    {
        internal DateTime Start { get; private set; } = DateTime.MaxValue;
        internal DateTime End { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Entries with a duration of 0 are considered Failures.
        /// </summary>
        internal int Failures { get; private set; } = 0;

        /// <summary>
        /// Only of successful matches
        /// </summary>
        private readonly List<TimeSpan> Durations = new List<TimeSpan>(100);

        double AverageDurationMs => Durations.Count == 0 ? 0.0 : (Durations.Sum(d => d.TotalMilliseconds) / Durations.Count); //might hit an arithmetic overflow here if the results are too many

        double MedianDurationMs => Durations.Count == 0 ? 0.0 : Durations.OrderBy(d => d).ElementAt(Durations.Count / 2).TotalMilliseconds;

        double FrequencyPerHour => (Durations.Count + Failures) / (End - Start).TotalHours;

        public void Update(LogMatch result)
        {
            if (result.Start < Start) Start = result.Start;
            if (End < result.Start) End = result.Start; //the duration of the result is not taken into account for simplicity

            if (result.Duration.HasValue)
                Durations.Add(result.Duration.Value);
            else
                Failures++;
        }

        public void MergeFrom(Aggregate right)
        {
            if (right.Start < Start) Start = right.Start;
            if (End < right.End) End = right.End;
            Durations.AddRange(right.Durations);
            Failures += right.Failures;
        }

        public override string ToString() => $"Found {FrequencyPerHour:F3} entries/hour: {Durations.Count} successes and {Failures} failures. For successes, average Duration is {AverageDurationMs:F0}ms and median is {MedianDurationMs:F0}ms.";
    }
}
