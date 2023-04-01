using QuickConfig;
using System.Reflection;
using Todoist.Net;
using Todoist.Net.Models;
using TraktCalendarToTodoist.Entities;
using TraktCalendarToTodoist.Factories;
using TraktCalendarToTodoist.Helpers;

ConfigManager _cm = new ConfigManager("TraktCalendarToTodoist");
Config _config = _cm.GetConfig<Config>("Config");

List<TraktShow> shows = TraktFeed.DeserializeTraktCalendarRss(_config.traktRSS);

ITodoistClient manager;
try
{
    manager = new TodoistClient(_config.todoistAPI);
}
catch (Exception ex)
{
    Console.WriteLine("Something went wrong with connecting to Todoist. Check if the credentials in the config are correct.\n\nException:\n{0}", ex.ToString());

    throw;
}

TodoistFactory todoistFactory = new TodoistFactory(manager);
List<Item> tasks = await todoistFactory.GetTasks(manager);

foreach(TraktShow show in shows)
{
    if(!tasks.Any(i => i.Content == show.Title))
    {
        Item calendarItem = new Item(show.Title)
        {
            DueDate = new DueDate(show.PublishDate.DateTime)
        };

        var test = await todoistFactory.CreateTask(manager, calendarItem);
    }
}