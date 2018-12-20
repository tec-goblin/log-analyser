using System;

namespace LogAnalyser
{
    public class ParsedLine
    {
        internal DateTime Start { get; }
        internal int  Duration { get; }
        internal string Verb { get; }
        internal Uri Uri { get; }
        internal short Status { get; } 
        internal string UserAgent { get; }

        public ParsedLine(DateTime start, int duration, string verb, Uri uri, short status, string userAgent)
        {
            Start = start;
            Duration = duration;
            Verb = verb;
            Uri = uri;
            Status = status;
            UserAgent = userAgent;
        }
    }
}