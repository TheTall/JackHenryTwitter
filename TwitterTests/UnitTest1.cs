using DataLayer;
using DataLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using TwitterReport.Controllers;

namespace TwitterTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestTwitterStats()
        {
            TwitterRepository twitterStats = new TwitterRepository();
            twitterStats.WriteTwitterData("{\"data\":{\"id\":\"1523760888825917440\",\"text\":\"This is a tweet\"}}");
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
            twitterStats.WriteTwitterData("{\"data\":{\"id\":\"1523760888825917440\",\"text\":\"This is a tweet\"}}");
            TweetStatsController tweetStatsController = new TweetStatsController(twitterStats);
            var res = tweetStatsController.GetTweetStats().Result;
            Assert.IsNotNull(res);
            var data = res.Value;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.TweetCount == 1);
        }

        //[TestMethod]
        //public void TestTwitterReader()
        //{
        //    HttpClient httpClient = new HttpClient();
        //    string bearerToken = "AAAAAAAAAAAAAAAAAAAAAFeocQEAAAAAMRidjdZzjMFNI%2FDD7DHpsvvhHPY%3DgDBMLfFbDiuUgMu3r2rXwbQctjw3mMpnlabPqhUIGQfQfW7CNn";
        //    TwitterReader twitterReader = new TwitterReader(httpClient, bearerToken);
        //    twitterReader.TwitterSetup(new System.Threading.CancellationToken()).Wait();

        //    string line = twitterReader.GetNextTweet().Result;
        //    Assert.IsNotNull(line);
        //    twitterReader.Dispose();
        //}

        //[TestMethod]
        //public void TestTwitterReaderBadToken()
        //{
        //    HttpClient httpClient = new HttpClient();
        //    string bearerToken = "xxxx";
        //    TwitterReader? twitterReader =null;
        //    try
        //    {

        //        twitterReader = new TwitterReader(httpClient, bearerToken);
        //        twitterReader.TwitterSetup(new System.Threading.CancellationToken()).Wait();
        //        string line = twitterReader.GetNextTweet().Result;
        //        Assert.IsNotNull(line);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        // should get error: Response status code does not indicate success: 401 (Unauthorized).
        //    }
        //    if (twitterReader != null)
        //        twitterReader.Dispose();
        //}

    }
}