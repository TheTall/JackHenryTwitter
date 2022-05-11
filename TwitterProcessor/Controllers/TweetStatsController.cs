using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TwitterReport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetStatsController : ControllerBase
    {
        private readonly TwitterRepository _twitterStats;
        public TweetStatsController(TwitterRepository twitterStats)
        {
            _twitterStats = twitterStats;
        }
        [HttpGet]
        public async Task<ActionResult<TweetStats>> GetTweetStats()
        {
            TweetStats tweetStats = await _twitterStats.GetStatistics();

            return tweetStats;
        }

    }
}
