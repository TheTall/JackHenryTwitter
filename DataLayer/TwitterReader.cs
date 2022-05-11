using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TwitterReader : IDisposable
    {
        private readonly HttpClient _httpClient;

        private string _url = "https://api.twitter.com/2/tweets/sample/stream";
        private string _bearerToken;

        private Stream _stream;
        private StreamReader _reader;

        public TwitterReader(HttpClient httpClient, string bearerToken)
        {
            _httpClient = httpClient;
            _bearerToken = bearerToken;
        }
        public async Task TwitterSetup(CancellationToken stoppingToken)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
            _stream = await _httpClient.GetStreamAsync(_url, stoppingToken);
            _reader = new StreamReader(_stream, System.Text.Encoding.ASCII, true);
            if (_reader == null)
            {
                throw new NullReferenceException("StreamReader returned null");
            }

        }
        public async Task<string> GetNextTweet()
        {
            return await _reader.ReadLineAsync();
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }
        }
    }
}
