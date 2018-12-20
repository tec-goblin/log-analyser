using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogAnalyser
{
    public class SimpleSysperSearch : ISearchStrategy<UrlAndPerIdParameters>
    {
        private Regex? perIdRegex;

        private UrlAndPerIdParameters? parameters;
        public UrlAndPerIdParameters Parameters
        {
            set
            {
                parameters = value;
                if (parameters.PerId != null)
                    perIdRegex = new Regex($"per_+id={parameters.PerId}", RegexOptions.IgnoreCase);
            }
        }

        public IEnumerable<LogMatch> NotifyLastLine() { yield break; }

        public IEnumerable<LogMatch> Apply(ParsedLine line)
        {
            if (parameters == null)
               throw new InvalidOperationException("Parameters must be initialised");

            if ((parameters.Url == null || line.Uri.OriginalString.ToLowerInvariant().Contains(parameters.Url.ToLowerInvariant()))
             && (perIdRegex == null || perIdRegex.IsMatch(line.Uri.Query)))
                yield return new LogMatch(line.Start, line.Uri.OriginalString, TimeSpan.FromMilliseconds(line.Duration));
         }
    }

    public class UrlAndPerIdParameters : SearchParameters
    {
        public int? PerId { get; }
        public UrlAndPerIdParameters(string? url, int? perId = null) : base(url)
        {
            if (url == null && !perId.HasValue)
                throw new ArgumentNullException("url", "Url must have a value if perId doesn't");

            PerId = perId;
        }
    }
}
