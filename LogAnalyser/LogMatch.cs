using System;

namespace LogAnalyser
{
    public readonly struct LogMatch //to test with classes as well
    {

        public TimeSpan? Duration { get; }
        public string Message { get; }
        public DateTime Start { get; }

        public LogMatch(DateTime start, string message, TimeSpan? duration = null)
        {
            Duration = duration;
            Start = start;
            Message = message;
        }

        public override string ToString() => $"{Start}: {Message}";
    }
}