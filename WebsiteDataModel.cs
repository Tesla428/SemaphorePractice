using System;

namespace SemaphorePractice
{
    public class WebsiteDataModel
    {
        public string WebsiteUrl1 { get; set; } = "";
        public int Instance { get; set; }
        public string WebsiteUrl2 { get; set; } = "";
        public string WebsiteData1 { get; set; } = "";
        public string WebsiteData2 { get; set; } = "";
        public TimeSpan TimerInterval { get; set; }
    }
}
