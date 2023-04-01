using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todoist.Net;
using Todoist.Net.Models;
using TraktCalendarToTodoist.Helpers;

namespace TraktCalendarToTodoist.Factories
{
    public class TodoistFactory
    {
        private ITodoistClient _client;
        private RateLimiter _rateLimiter;

        public TodoistFactory(ITodoistClient client)
        {
            _client = client;
            _rateLimiter = new RateLimiter(_client, 450, TimeSpan.FromMinutes(15));
        }

        public async Task<List<Item>> GetTasks(ITodoistClient client)
        {
            List<Item> response = _rateLimiter.ExecuteAsync(async client => await client.GetResourcesAsync(ResourceType.Items)).Result.Items.ToList();

            return response;
        }

        public async Task<string> CreateTask(ITodoistClient client, Item task)
        {
            string response = _rateLimiter.ExecuteAsync(async client => await client.Items.AddAsync(task)).Result.PersistentId;

            return response;
        }
    }
}
