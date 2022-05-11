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

        private string _url = "https://api.twitter.com/2/tweets/sample/stream";
        private string _bearerToken;

        private ConcurrentQueue<string> _queue;
        private TwitterReader _twitterReader;

        public QueuedWorker(ILogger<QueuedWorker> logger, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, TwitterRepository twitterStats, ConcurrentQueue<string> queue)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            IConfigurationSection section = configuration.GetSection("Twitter");
            _bearerToken = section.GetValue<string>("BearerToken");
            _twitterStats = twitterStats;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            // instantiate the TwitterReader
            using (_twitterReader = new TwitterReader(httpClient, _bearerToken))
            {
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
}
