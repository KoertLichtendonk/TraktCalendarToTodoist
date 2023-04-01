using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TraktCalendarToTodoist.Entities;

namespace TraktCalendarToTodoist.Helpers
{
    public class TraktFeed
    {
        public static List<TraktShow> DeserializeTraktCalendarRss(string url)
        {
            List<TraktShow> shows = new List<TraktShow>();

            using (XmlReader reader = XmlReader.Create(url))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    var show = new TraktShow
                    {
                        Title = item.Title.Text,
                        PublishDate = item.PublishDate
                    };
                    shows.Add(show);
                }
            }

            return shows;
        }
    }
}
