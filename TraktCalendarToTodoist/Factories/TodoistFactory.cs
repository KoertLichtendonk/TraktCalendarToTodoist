using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todoist.Net;
using Todoist.Net.Models;
using TraktCalendarToTodoist.Entities;
using TraktCalendarToTodoist.Helpers;

namespace TraktCalendarToTodoist.Factories
{
    public class TodoistFactory
    {
        private ITodoistClient _client;
        private RateLimiter _rateLimiter;
        private Config _config;

        public TodoistFactory(ITodoistClient client, Config config)
        {
            _client = client;
            _config = config;
            _rateLimiter = new RateLimiter(_client, 450, TimeSpan.FromMinutes(15));
        }

        public async Task<List<Item>> GetTasks(ITodoistClient client)
        {
            try
            {
                List<Item> response = _rateLimiter.ExecuteAsync(async client => await client.GetResourcesAsync(ResourceType.Items)).Result.Items.ToList();

                return response;
            }
            catch (AggregateException ex)
            {
                Task.Delay(_config.retryAfter);

                Console.WriteLine("Waiting for retryAfter period.");

                return GetTasks(client).Result;
            }
        }

        public async Task<ComplexId> CreateTask(ITodoistClient client, Item task)
        {
            try
            {
                ComplexId response = _rateLimiter.ExecuteAsync(async client => await client.Items.AddAsync(task)).Result;

                return response;
            }
            catch (AggregateException ex)
            {
                Task.Delay(_config.retryAfter);

                Console.WriteLine("Waiting for retryAfter period.");

                return CreateTask(client, task).Result;
            }
        }

        public async Task<ComplexId> CreateReminder(ITodoistClient client, Reminder reminder)
        {
            try
            {
                ComplexId response = _rateLimiter.ExecuteAsync(async client => await client.Reminders.AddAsync(reminder)).Result;

                return response;
            }
            catch (AggregateException ex)
            {
                Task.Delay(_config.retryAfter);

                Console.WriteLine("Waiting for retryAfter period.");

                return CreateReminder(client, reminder).Result;
            }
            }
        }
}
