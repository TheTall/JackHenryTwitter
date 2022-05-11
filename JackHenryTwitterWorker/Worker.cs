using DataLayer;

namespace TwitterWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TwitterStats _twitterStats;

        private string _url= "https://api.twitter.com/2/tweets/sample/stream";
        //private string _APIKey;
        //private string _APISecret;
        private string _bearerToken;

        public Worker(ILogger<Worker> logger,  IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, TwitterStats twitterStats)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            IConfigurationSection section= configuration.GetSection("Twitter");
            //_APIKey = section.GetValue<string>("APIKey");
            //_APISecret = section.GetValue<string>("APISecret");
            _bearerToken = section.GetValue<string>("BearerToken");
            _twitterStats = twitterStats;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpClient httpClient=_httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
                Stream? stream = await httpClient.GetStreamAsync(_url, stoppingToken);
                _twitterStats.Start();
                while (!stoppingToken.IsCancellationRequested)
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.ASCII, true);
                    Console.WriteLine(reader.ReadLine());
                    _twitterStats.IncrementTweetCount();
                }
                stream.Close();
            }
        }

    }
}