using System;
using Web.Util;

namespace Web.ViewModel
{
    public class ResponseViewModel
    {
        public string status { get; set; }
        public object data { get; set; }
        public string message { get; set; } = string.Empty;
        public DateTime timestamp { get; set; } = DateTime.Now;

    }

    public class ResponseOutSideViewModel
    {
        public string status { get; set; }
        public object data { get; set; }
        public string token { get; set; }
        public string message { get; set; } = string.Empty;
        public string version { get; set; } = JWT.BuildVersion;
    }
}
