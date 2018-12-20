using LogAnalyser;
using System;
using System.Diagnostics;

namespace ConsoleLogAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("Please provide the folder to search as an argument");

            var parser = new Parser(result => Debug.WriteLine(result));

           // parser.ParseLogs<SimpleSysperSearch, UrlAndPerIdParameters>(folder: args[0], new UrlAndPerIdParameters("editPersonAddressAndTelecomFrame.do?othIdAdrContext=12003"));

            parser.ParseLogs<SuccessAnalysis, SuccessAnalysisParameters>(folder: args[0], 
                   new SuccessAnalysisParameters ("editPersonAddressAndTelecomFrame.do?othIdAdrContext=12003", "saveNewPersonAddress_success.do"));
        }

    }
}
