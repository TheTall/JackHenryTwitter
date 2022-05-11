using DataLayer;
using System.Collections.Concurrent;


namespace TwitterProcessor
{
    public class QueuedWorker : BackgroundService
    {
        private readonly ILogger<QueuedWorker> _logger;
        //private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TwitterRepository _twitterStats;

        private ConcurrentQueue<string> _queue;
        private TwitterReader _twitterReader;

        public QueuedWorker(ILogger<QueuedWorker> logger, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, TwitterRepository twitterStats, ConcurrentQueue<string> queue,
            TwitterReader twitterReader)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _twitterStats = twitterStats;
            _queue = queue;
            _twitterReader = twitterReader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            try
            {
                await _twitterReader.TwitterSetup(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    string line = await _twitterReader.GetNextTweet();
                    _queue.Enqueue(line);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading Tritter stream");
            }
        }


    }
}
