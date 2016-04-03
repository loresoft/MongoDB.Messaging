using System;

namespace Sleep.Messages
{
    public class EchoMessage
    {
        public const string QueueName = "EchoQueue";

        public string Text { get; set; }

        public bool Throw { get; set; }

        public override string ToString()
        {
            return $"Echo Message Text: {Text}";
        }
    }
}