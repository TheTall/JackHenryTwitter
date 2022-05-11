using DataLayer.Models;
using Newtonsoft.Json;

namespace DataLayer
{
    public class TwitterRepository
    {
        private long tweetCount = 0;
        private DateTime? startTime = null;

        public TwitterRepository()
        {
            tweetCount = 0;
        }

        public void WriteTwitterData(string tweetData)
        {
            // set startdate on first write of twitter data
            if (startTime == null)
                startTime = DateTime.Now;

            Tweet tweet = JsonConvert.DeserializeObject<Tweet>(tweetData);
            // This is where you would store the twitter data

            // For now, just increment the counter for now
            Interlocked.Increment(ref tweetCount);
            // add a fake delay that would be present if updating a database
            Thread.Sleep(100);
        }

        public async Task<TweetStats> GetStatistics()
        {

            // would be reading a database async

            // get stats
            long ttlMin = (long)DateTime.Now.Subtract(startTime.Value).TotalMinutes;
            long ttlSec = (long)DateTime.Now.Subtract(startTime.Value).TotalSeconds;
            long tweetsPerMin = ttlMin == 0 ? 0 : tweetCount / ttlMin;
            long tweetsPerSec = ttlSec == 0 ? 0 : tweetCount / ttlSec;
            TweetStats tweetStats = new TweetStats()
            {
                TweetCount = tweetCount,
                TweetsPerMin = tweetsPerMin,
                TweetsPerSecond = tweetsPerSec,
                TotalMinutes = ttlMin,
                TotalSeconds = ttlSec

            };
            return tweetStats;
        }

    }
}