using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyser
{
    public class SuccessAnalysis : ISearchStrategy<SuccessAnalysisParameters>
    {
        private SuccessAnalysisParameters? parameters;
        public SuccessAnalysisParameters Parameters { set { parameters = value; } }

        private readonly IDictionary<string, DateTime> attemptsByUser = new Dictionary<string, DateTime>();

        private readonly Regex perIdRegex = new Regex(@"perId=(?<perId>\d+)", RegexOptions.IgnoreCase);

    public IEnumerable<LogMatch> Apply(ParsedLine line)
        {
            if (parameters == null)
                throw new InvalidOperationException("Parameters must be initialised");
            
            var path = line.Uri.OriginalString.ToLowerInvariant();
            if (path.Contains(parameters.Url.ToLowerInvariant())) //i think the warning is a compiler idiosyncracy, to report
            {
                var perIdSearch = perIdRegex.Match(path);
                if (perIdSearch.Success)
                {
                    var user = perIdSearch.Groups["perId"].Value;
                    if (!attemptsByUser.ContainsKey(user))
                        attemptsByUser[user] = line.Start;
                }
            }
            else if (path.Contains(parameters.SuccessPattern.ToLowerInvariant()))
            {
                var perIdSearch = perIdRegex.Match(path);
                if (perIdSearch.Success)
                {
                    var user = perIdSearch.Groups["perId"].Value; //avoiding this duplication is not elegant in a code using yield return
                    if (attemptsByUser.ContainsKey(user))
                    {
                        var taskDuration = line.Start - attemptsByUser[user] + TimeSpan.FromMilliseconds(line.Duration);
                        var match = new LogMatch(line.Start, $"Successful completion of the task in {taskDuration.TotalSeconds}s", taskDuration);
                        attemptsByUser.Remove(user);
                        yield return match;
                    }
                }
            }

            var tooOld = line.Start - parameters.SuccessTimeOut;
            var keysToRemove = from k in attemptsByUser.Keys
                               where attemptsByUser[k] < tooOld
                               select k;
            foreach (var k in keysToRemove)
            {
                yield return new LogMatch(attemptsByUser[k], "Failed attempt of the task");
                attemptsByUser.Remove(k);
            }
        }
        public IEnumerable<LogMatch> NotifyLastLine() => from start in attemptsByUser.Values
                                                         select new LogMatch(start, "Did not complete by the end of the analysis");
    }

    public class SuccessAnalysisParameters : SearchParameters
    {
        public string SuccessPattern { get; }
        public TimeSpan SuccessTimeOut { get; }

        public SuccessAnalysisParameters(string url, string successPattern, TimeSpan? successTimeout = null) : base(url)
        {
            SuccessPattern = successPattern;
            SuccessTimeOut = successTimeout ?? TimeSpan.FromMinutes(20);
        }
    }
}
