using System;

namespace Raw.Streaming.Webhook.Model
{
    public class ScheduledStream
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Game { get; set; }
        public string Description { get; set; }
    }
}
