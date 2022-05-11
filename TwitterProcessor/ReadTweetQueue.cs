using DataLayer;
using System.Collections.Concurrent;

namespace TwitterProcessor
{
    public class ReadTweetQueue : BackgroundService
    {
        private readonly ILogger<ReadTweetQueue> _logger;
        private ConcurrentQueue<string> _queue;
        private readonly TwitterRepository _twitterStats;
        private readonly int _maxQueues;


        public ReadTweetQueue(ILogger<ReadTweetQueue> logger, TwitterRepository twitterStats, ConcurrentQueue<string> queue, IConfiguration configuration)
        {
            _logger = logger;
            _twitterStats = twitterStats;
            _queue = queue;
            _maxQueues = configuration.GetValue<int>("MaxQueues");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // start _maxQueues GetTweets() methods to get the Tweet data
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < _maxQueues; i++)
                {
                    tasks.Add(new Task(async () => await GetTweets(stoppingToken)));
                }
                Parallel.ForEach(tasks, task =>
                {
                    task.Start();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting up Reader tasks");
            }
        }

        private async Task GetTweets(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string tweet = "";
                    // Try to get the data from the queue
                    if (_queue.TryDequeue(out tweet))
                    {
                        Console.WriteLine($"ID:{Task.CurrentId}, Cnt:{_queue.Count()} {tweet}");
                        // update the data store
                        _twitterStats.WriteTwitterData(tweet);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading queue");
                }
            }
        }



    }
}
