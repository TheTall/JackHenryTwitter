using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class TweetStats
    {
        public long TweetCount { get; set; }
        public long TweetsPerMin { get; set; }
        public long TweetsPerSecond { get; set; }
        public long TotalMinutes { get; set; }
        public long TotalSeconds { get; set; }

    }
}
