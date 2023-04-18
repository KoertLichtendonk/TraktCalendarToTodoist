using QuickConfig;
using System.Reflection;
using System.Text.RegularExpressions;
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

TodoistFactory todoistFactory = new TodoistFactory(manager, _config);
List<Item> tasks = await todoistFactory.GetTasks(manager);

foreach(TraktShow show in shows)
{
    Match match = Regex.Match(show.Title, @"\dx\d+");

    if (tasks.Any(i => i.Content == show.Title))
    {
        Console.WriteLine(String.Format("{0} bestaat al.", show.Title));
        continue;
    }    
        
    if (tasks.Any(i => i.Content.Contains(match.Value)))
    {
        Console.WriteLine(String.Format("{0} bestaat al als aflevering {1}.", show.Title, match.Value));
        continue;
    }

    Item calendarItem = new Item(show.Title)
    {
        DueDate = new DueDate(show.PublishDate.DateTime, false, _config.timezone)
    };

    calendarItem.Labels.Add("Trakt");

    ComplexId task = await todoistFactory.CreateTask(manager, calendarItem);

    if(!task.IsEmpty)
    {
        await todoistFactory.CreateReminder(manager, new Reminder(task) { Type = ReminderType.Relative, MinuteOffset = 15 });
        await todoistFactory.CreateReminder(manager, new Reminder(task) { Type = ReminderType.Relative, MinuteOffset = 30 });
        await todoistFactory.CreateReminder(manager, new Reminder(task) { Type = ReminderType.Relative, MinuteOffset = 60 });
    }
}