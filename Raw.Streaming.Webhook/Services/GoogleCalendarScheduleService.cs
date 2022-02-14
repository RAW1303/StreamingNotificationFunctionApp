using Google.Apis.Calendar.v3;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public class GoogleCalendarScheduleService : IScheduleService 
    {
        private readonly CalendarService _calendarService;
        public GoogleCalendarScheduleService(CalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<IReadOnlyList<StreamEvent>> GetScheduledStreamsAsync(DateTime from, DateTime to)
        {
            var eventsQuery = _calendarService.Events.List(AppSettings.ScheduleGoogleCalendarId);
            eventsQuery.TimeMin = from;
            eventsQuery.TimeMax = to;
            eventsQuery.TimeZone = "UTC";
            eventsQuery.SingleEvents = true;
            var events = await eventsQuery.ExecuteAsync();
            return events.Items.Select(e =>
            new StreamEvent()
            {
                Title = e.Summary,
                Description = e.Description,
                Game = e.Location,
                Start = e.Start.DateTime ?? from,
                End = e.End.DateTime ?? to
            }).ToList();
        }
    }
}
