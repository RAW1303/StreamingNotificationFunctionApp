using System;
using System.Collections.Generic;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    internal class TwitchSchedule
    {
        public string BroadcasterId { get; set; }
        public string BroadcasterName { get; set; }
        public string BroadcasterLogin { get; set; }
        public TwitchScheduleVacation Vacation { get; set; }
        public IList<TwitchScheduleSegment> Segments { get; set; }
    }
    internal class TwitchScheduleSegment
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public DateTime? CancelledUntil { get; set; }
        public TwitchGame Category { get; set; }
        public bool IsRecurring { get; set; }
    }

    internal class TwitchScheduleVacation
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
