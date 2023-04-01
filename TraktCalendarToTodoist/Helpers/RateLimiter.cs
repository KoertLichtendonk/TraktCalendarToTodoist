using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todoist.Net;
using Todoist.Net.Exceptions;

namespace TraktCalendarToTodoist.Helpers
{
    public class RateLimiter
    {
        private readonly ITodoistClient _client;
        private readonly SemaphoreSlim _rateLimiter;
        private readonly TimeSpan _rateLimitInterval;

        public RateLimiter(ITodoistClient client, int maxRequestsPerInterval, TimeSpan rateLimitInterval)
        {
            _client = client;
            _rateLimiter = new SemaphoreSlim(maxRequestsPerInterval, maxRequestsPerInterval);
            _rateLimitInterval = rateLimitInterval;
        }

        public async Task<T> ExecuteAsync<T>(Func<ITodoistClient, Task<T>> action)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var result = await action(_client);
                return result;
            }
            finally
            {
                _ = Task.Delay(_rateLimitInterval).ContinueWith(t => _rateLimiter.Release());
            }
        }
    }
}
