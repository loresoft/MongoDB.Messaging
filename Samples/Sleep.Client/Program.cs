using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void Main(string[] args)
        {
            Logger.RegisterWriter(NLogWriter.Default);

            ShowVersion();
            Initialize();
            DebugRun(args);
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

        private static void ShowVersion()
        {
            Console.WriteLine("{0} {1}", ThisAssembly.AssemblyProduct, ThisAssembly.AssemblyFileVersion);
            Console.WriteLine(ThisAssembly.AssemblyCopyright);
            Console.WriteLine();
        }

        private static void DebugRun(string[] args)
        {
            DebugHelp();

            while (true)
            {
                string line = Console.In.ReadLine() ?? string.Empty;
                if (string.Equals(line, "Q", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                int count;

                if (int.TryParse(line, out count))
                {
                    Console.WriteLine("Send {0}x Sleep Messages...", count);
                    SendMany(count);
                }


                if (string.Equals(line, "S", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Schedule Sleep Message...");
                    ScheduleSleep();
                }
                else if (string.Equals(line, "E", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Send Sleep Message Error...");
                    SendSleep(1, true);
                }



                DebugHelp();
            }
        }



        private static void DebugHelp()
        {
            Console.WriteLine("Sleep Sample Commands.");
            Console.WriteLine("  1  Send x Sleep Messages");
            Console.WriteLine("  E  Send Sleep Message Error");
            Console.WriteLine("  S  Schedule Sleep Message");
            Console.WriteLine("  Q  Quit");
        }

        private static void SendMany(int max = 1)
        {
            for (int i = 1; i <= max; i++)
                SendSleep(i);
        }

        private static void SendSleep(int count = 1, bool isError = false)
        {
            int seconds = _random.Next(10, 20);

            var sleepMessage = new SleepMessage();
            sleepMessage.Time = TimeSpan.FromSeconds(seconds);
            sleepMessage.Text = string.Format("Sleep Message {0:000}", count);
            sleepMessage.Throw = isError;

            var message = MessageQueue.Default.Publish(m => m
                .Queue(SleepMessage.QueueName)
                .Data(sleepMessage)
            ).Result;

            Console.WriteLine("Publish Message: '{0}', Id: {1}", message.Description, message.Id);
        }

        private static void ScheduleSleep(int count = 1, bool isError = false)
        {
            int seconds = _random.Next(10, 20);

            var sleepMessage = new SleepMessage();
            sleepMessage.Time = TimeSpan.FromSeconds(seconds);
            sleepMessage.Text = string.Format("Schedule Message {0:000}", count);
            sleepMessage.Throw = isError;

            var message = MessageQueue.Default.Schedule(m => m
                .Schedule(DateTime.Now.AddMinutes(1))
                .Queue(SleepMessage.QueueName)
                .Data(sleepMessage)
            ).Result;

            Console.WriteLine("Publish Message: '{0}', Id: {1}", message.Description, message.Id);
        }
    }
}
