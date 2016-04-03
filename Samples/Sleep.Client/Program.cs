using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using MongoDB.Messaging;
using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Service;
using Sleep.Client.Logging;
using Sleep.Messages;

namespace Sleep.Client
{
    public class Program
    {
        private static readonly Random _random = new Random();
        private static int _counter = 1;

        public static void Main(string[] args)
        {
            Logger.RegisterWriter(NLogWriter.Default);

            Initialize();
            DebugRun();
        }

        private static void Initialize()
        {
            MessageQueue.Default.Configure(c => c
                .Connection("MongoMessaging")
                .Queue(s => s
                    .Name(SleepMessage.QueueName)
                    .Retry(5)
                )
            );
        }

        private static void DebugRun()
        {
            bool processing = true;
            bool wait = false;

            while (processing)
            {
                string commandLine = wait ? Console.In.ReadLine() : string.Empty;
                if (string.IsNullOrEmpty(commandLine))
                    commandLine = "help";

                var args = NativeMethods.CommandLineToArgs(commandLine);

                Parser.Default.ParseArguments<SleepOptions, EchoOptions, QuitOptions>(args)
                  .WithParsed<SleepOptions>(opts => SendSleep(opts))
                  .WithParsed<EchoOptions>(opts => SendEcho(opts))
                  .WithParsed<QuitOptions>(opts => processing = false);


                wait = true;
            }
        }

        private static void SendEcho(EchoOptions opts)
        {
            int max = Math.Max(1, opts.Count);

            for (int i = 1; i <= max; i++)
            {
                var sleepMessage = new EchoMessage();
                sleepMessage.Text = $"Echo Message {i:000}: {opts.Message}";
                sleepMessage.Throw = opts.Throw;

                if (opts.When > 0)
                {
                    var message = MessageQueue.Default.Schedule(m => m
                        .Schedule(DateTime.Now.AddMinutes(opts.When))
                        .Queue(EchoMessage.QueueName)
                        .Data(sleepMessage)
                    ).Result;

                    Console.WriteLine("Schedule Message: '{0}', Id: {1}", message.Description, message.Id);
                }
                else
                {
                    var message = MessageQueue.Default.Publish(m => m
                        .Queue(EchoMessage.QueueName)
                        .Data(sleepMessage)
                    ).Result;

                    Console.WriteLine("Publish Message: '{0}', Id: {1}", message.Description, message.Id);
                }
            }

        }

        private static void SendSleep(SleepOptions opts)
        {
            int max = Math.Max(1, opts.Count);

            for (int i = 1; i <= max; i++)
            {
                int seconds = _random.Next(10, 20);

                var sleepMessage = new SleepMessage();
                sleepMessage.Time = TimeSpan.FromSeconds(seconds);
                sleepMessage.Text = $"Sleep Message {i:000}";
                sleepMessage.Throw = opts.Throw;

                if (opts.When > 0)
                {
                    var message = MessageQueue.Default.Schedule(m => m
                        .Schedule(DateTime.Now.AddMinutes(opts.When))
                        .Queue(SleepMessage.QueueName)
                        .Data(sleepMessage)
                    ).Result;

                    Console.WriteLine("Schedule Message: '{0}', Id: {1}", message.Description, message.Id);
                }
                else
                {
                    var message = MessageQueue.Default.Publish(m => m
                        .Queue(SleepMessage.QueueName)
                        .Data(sleepMessage)
                    ).Result;

                    Console.WriteLine("Publish Message: '{0}', Id: {1}", message.Description, message.Id);
                }
            }
        }
    }
}
