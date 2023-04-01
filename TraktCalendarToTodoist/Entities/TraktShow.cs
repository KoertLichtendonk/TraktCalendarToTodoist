using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraktCalendarToTodoist.Entities
{
    public class TraktShow
    {
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
    }
}
