using QuickConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraktCalendarToTodoist.Entities
{
    public class Config : IConfig
    {
        public string configPath { get; set; }
        public string traktRSS { get; set; }
        public string todoistAPI { get; set; }
        public string timezone { get; set; }

        public Config()
        {
            this.configPath = String.Empty;
            this.traktRSS = String.Empty;
            this.todoistAPI = String.Empty;
            this.timezone = "Europe/Amsterdam";
        }
    }
}
