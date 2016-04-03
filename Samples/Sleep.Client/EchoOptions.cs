using System;
using CommandLine;

namespace Sleep.Client
{
    [Verb("echo", HelpText = "Send echo message")]
    public class EchoOptions
    {
        [Option("count", HelpText = "The number of messages to send", Default = 1)]
        public int Count { get; set; } = 1;

        [Option("when", HelpText = "The number minutes to delay processing")]
        public int When { get; set; }

        [Option("throw", HelpText = "Throw an error while processing")]
        public bool Throw { get; set; }

        [Option("message", HelpText = "The message to echo")]
        public string Message { get; set; }
    }
}