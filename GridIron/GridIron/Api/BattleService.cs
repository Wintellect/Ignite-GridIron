using Refit;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace GridIron.Api
{
    class BattleService : IBattleService
    {
        IBattleService _implementation;

        public BattleService (string hostUrl)
        {
            //var handler = new HttpClientHandler ();
            var client = new HttpClient (/*handler*/) { BaseAddress = new Uri (hostUrl) };
            client.Timeout = TimeSpan.FromSeconds (5);
            _implementation = RestService.For<IBattleService> (client);
        }

        Task<Battle> IBattleService.GetStatus (StatusRequest request)
        {
            return _implementation.GetStatus (request);
        }

        Task IBattleService.Vote (VoteRequest request)
        {
            return _implementation.Vote (request);
        }
    }
}
