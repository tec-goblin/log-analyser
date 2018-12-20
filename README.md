# log-analyser
A tool to extract statistics from apache-like http logs. It is not a library, but it's structured in a way that should be easy for you to adapt for your own needs.
## Who is it for
Practically me, or anyone else who, due to an organisational accident, cannot have their logs in a proper service like Kinbana or Azure Application Insights, and has to parse them with in-house tools.
## How to Use It
The *ConsoleLogAnalyser* project is just a very simple exe to test and run the tool.  
All the logic is in the *LogAnalyser* project, which is a **.NET Standard** library, so it could be used in most OSes and many flavours of .NET (in our case, it's designed to be called by a legacy WinForms application which presents the different search options and results).  
The project uses preview features of C#8 (strict nulls and range operators). You can already [use them now](https://blogs.msdn.microsoft.com/dotnet/2018/12/05/take-c-8-0-for-a-spin/).
## Making it your own
The *Parser* is navigating the structure of your logs. You might need to adapt *apacheLogRegex*, as most probably in your logs the fields might be in a different order or with a different separator (you can find apache log http logs regexes on the internet).  
You will most probably need to define new implementations of *ISearchStrategy*, to suit your needs. The included ones can be used as examples:  
* *SimpleSearch* is searching for strings in the url, and/or a specific parameter (in our case the id of the person). It has no real state and will give the same results regarding of the parsing order.
* *SuccessAnalysis* is used for A/B testing in our case. It finds the start of a task, and searches for the end of that task until a timeout has passed. It can track multiple attempts to a task in parallel, but it needs to read each node sequentially (if you don't use sticky sessions, you will need to algorithm which defines when the attempt to perform a task has failed).
  
If you find it useful, I'd love to know it! I am sure in your context you will have more interesting search patterns.
