using System.Collections.Generic;

namespace LogAnalyser
{
    /// <summary>
    /// An algorithm operating to each parsed line of the input. 
    /// Some implementations are stateful and expect data from each node to be ordered in chronological order
    /// </summary>
    public interface ISearchStrategy<in T> where T: SearchParameters
    {
        T Parameters { set; }
        IEnumerable<LogMatch> Apply(ParsedLine line);
        IEnumerable<LogMatch> NotifyLastLine();
    }

    public class SearchParameters
    {
        public string? Url { get; }
        public SearchParameters(string? url)
        {
            Url = url;
        }
    }
}