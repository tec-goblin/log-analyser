using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyser
{
    public class Parser
    {
        private readonly Action<string> print;

        public Parser(Action<string> print)
        {
            this.print = print;
        }

        
        /// <param name="folder">The folder with the logs. 
        /// Any first-level subdirectories will be considered as different nodes, and analysis will be parallelised across them.</param>
        /// <param name="parameters">The parameters to be passed to the Search Strategy</param>
        public void ParseLogs<T, U>(string folder, U parameters) where U : SearchParameters
                                                                 where T : ISearchStrategy<U>, new()
            
        {
            var subDirectories = Directory.EnumerateDirectories(folder);
            Aggregate aggregate;
            if (subDirectories.Any())
            {
                var aggregates = (from d in subDirectories.AsParallel()
                                  select ParseFolder(new T { Parameters = parameters }, d)).ToArray();

                foreach (var agr in aggregates[1..])
                    aggregates[0].MergeFrom(agr);
                aggregate = aggregates[0];
            }
            else
                aggregate = ParseFolder(new T { Parameters = parameters }, folder);

            print(aggregate.ToString());
        }

        private Aggregate ParseFolder<U>(ISearchStrategy<U> searchStrategy, string folder) where U: SearchParameters
        {
            var folderAggregate = new Aggregate();

            foreach (var file in Directory.EnumerateFiles(folder, "*.log", SearchOption.AllDirectories))
            {
                foreach (var line in File.ReadAllLines(file)) //Async version exists only in .NET Core, it wouldn't be usable by WinForms
                {
                    var parsedLine = Parse(line);
                    if (parsedLine != null)
                        foreach (var result in searchStrategy.Apply(parsedLine))
                        { 
                            print(result.ToString());
                            folderAggregate.Update(result);
                        }
                }
            }

            var results = searchStrategy.NotifyLastLine();

            foreach (var result in results)
            {
                print(result.ToString());
                folderAggregate.Update(result);
            }

            return folderAggregate;
        }

        private static readonly Regex apacheLogRegex = new Regex(@"^(?<date>\S+\t\S+)\t\S+\t\S+\t\S+\t(?<status>\d+)\t(?<duration>\d+)\t\S+\t(?<verb>[A-Z]+)\t(?<path>\S+)\t(?<userAgent>[^\t]*)");

        private ParsedLine? Parse(string line)
        {
            var match = apacheLogRegex.Match(line);
            if (match.Success)
                return new ParsedLine(DateTime.Parse(match.Groups["date"].Value),//to check the parsing
                                      int.Parse(match.Groups["duration"].Value),
                                      match.Groups["verb"].Value,
                                      new Uri(match.Groups["path"].Value, UriKind.Relative),
                                      short.Parse(match.Groups["status"].Value),
                                      match.Groups["userAgent"].Value);
            else
                return null;
        }
    }
}
