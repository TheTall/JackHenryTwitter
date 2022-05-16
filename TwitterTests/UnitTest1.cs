using DataLayer;
using DataLayer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TwitterReport.Controllers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TwitterProcessor;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace TwitterTests
{
    [TestClass]
    public class UnitTest1
    {

        private string tweetResult="{\"data\":{\"id\":\"1523760888825917440\",\"text\":\"This is a tweet\"}}";

        [TestMethod]
        public void TestTwitterStats()
        {
            TwitterRepository twitterStats = new TwitterRepository();
            twitterStats.WriteTwitterData(tweetResult);
            var res = twitterStats.GetStatistics().Result;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.TweetCount == 1);
        }

        [TestMethod]
        public void TestTwitterStatsBadData()
        {
            TwitterRepository twitterStats = new TwitterRepository();
            try
            {
                // missing last bracket
                twitterStats.WriteTwitterData("{\"data\":{\"id\":\"1523760888825917440\",\"text\":\"This is a tweet\"}");
            }
            catch (System.Exception ex)
            {
            }
            var res = twitterStats.GetStatistics().Result;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.TweetCount == 0);
        }

        [TestMethod]
        public void GetTwitterStatsController()
        {
            TwitterRepository twitterStats = new TwitterRepository();
            twitterStats.WriteTwitterData(tweetResult);
            TweetStatsController tweetStatsController = new TweetStatsController(twitterStats);
            var res = tweetStatsController.GetTweetStats().Result;
            Assert.IsNotNull(res);
            var data = res.Value;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.TweetCount == 1);
        }

        [TestMethod]
        public void TestTwitterReader()
        {
            var mockFactory=new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tweetResult),
                });



            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var myConfiguration = new Dictionary<string, string>
            {
                {"Twitter:BearerToken", "AAAAAAAAAAAAAAAAAAAAAFeocQEAAAAAMRidjdZzjMFNI%2FDD7DHpsvvhHPY%3DgDBMLfFbDiuUgMu3r2rXwbQctjw3mMpnlabPqhUIGQfQfW7CNn"},
                {"Twitter:StreamURL", "https://api.twitter.com/2/tweets/sample/stream"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();


            TwitterReader twitterReader = new TwitterReader(mockFactory.Object, configuration);
            twitterReader.TwitterSetup(new System.Threading.CancellationToken()).Wait();

            string line = twitterReader.GetNextTweet().Result;
            Assert.IsNotNull(line);
            twitterReader.Dispose();
        }

        [TestMethod()]
        public void TestReadTweetQueue()
        {
            var logMock = new Mock<ILogger<ReadTweetQueue>>();
            ILogger<ReadTweetQueue> logger = logMock.Object;

            var queue = new ConcurrentQueue<string>();

            var myConfiguration = new Dictionary<string, string>
            {
                {"MaxQueues", "1"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            TwitterRepository twitterRepository =new TwitterRepository();
            ReadTweetQueue readTweetQueue = new ReadTweetQueue(logger, twitterRepository, queue, configuration);
            CancellationToken cancellationToken = new CancellationToken();
            readTweetQueue.StartAsync(cancellationToken).ConfigureAwait(false);
            Thread.Sleep(1000);
            queue.Enqueue(tweetResult);
            Thread.Sleep(1000);
            readTweetQueue.StopAsync(cancellationToken);
        }
    }
}