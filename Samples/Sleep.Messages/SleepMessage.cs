using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sleep.Messages
{
    public class SleepMessage
    {
        public const string QueueName = "SleepQueue";

        public TimeSpan Time { get; set; }

        public string Text { get; set; }

        public bool Throw { get; set; }

        public override string ToString()
        {
            return $"Sleep Message Text: {Text}, Throw: {Throw}, Time: {Time}";
        }
    }
}
