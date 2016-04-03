using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Sleep.Client
{
    [Verb("sleep", HelpText = "Send sleep message")]
    public class SleepOptions
    {
        [Option("count", HelpText = "The number of messages to send", Default = 1)]
        public int Count { get; set; }

        [Option("when", HelpText = "The number minutes to delay processing")]
        public int When { get; set; }

        [Option("throw", HelpText = "Throw an error while processing")]
        public bool Throw { get; set; }

        [Usage(ApplicationAlias = "")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Send Sleep message 10 times", new SleepOptions {Count = 10 });
                yield return new Example("Send Sleep with error", new SleepOptions { Throw = true });
            }
        }
    }
}