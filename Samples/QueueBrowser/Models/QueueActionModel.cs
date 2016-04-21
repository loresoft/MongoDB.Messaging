using System;
using System.Collections.Generic;

namespace QueueBrowser.Models
{
    public class QueueActionModel
    {
        public QueueActionModel()
        {
            Ids = new List<string>();
        }

        public string Name { get; set; }

        public List<string> Ids { get; set; }
    }
}