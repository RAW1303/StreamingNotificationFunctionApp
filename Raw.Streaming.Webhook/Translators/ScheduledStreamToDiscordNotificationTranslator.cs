using System.Collections.Generic;
using System.Linq;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model;
using Raw.Streaming.Webhook.Model.Discord;

namespace Raw.Streaming.Webhook.Translators
{
    public static class ScheduledStreamToDiscordNotificationTranslator
    {
        private const string CALENDAR_URL_TEMPLATE = "https://calendar.google.com/calendar/u/0/embed?mode=WEEK&showPrint=0&showTabs=0&title=Roy%20Weller%20Stream%20Schedule&showCalendars=0&bgcolor=%2300BDFF&src=";

        public static Notification TranslateDailySchedule(IEnumerable<ScheduledStream> scheduledStreams)
        {
            return new Notification()
            {
                Embeds = scheduledStreams.Select(stream =>  
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = "Today"
                        },
                        Title = stream.Title,
                        Description = HtmlUtilities.ConverHtmlToText(stream.Description),
                        Color = 48639,
                        Fields = new EmbedField[] {
                            new EmbedField()
                            {
                                Name = "Game",
                                Value = stream.Game,
                                Inline = true
                            },
                            new EmbedField()
                            {
                                Name = "Time",
                                Value = $"{stream.Start:t} - {stream.End:t} UTC",
                                Inline = true
                            },
                        }
                    }).ToArray()
            };
        }

        public static Notification TranslateWeeklySchedule(IEnumerable<ScheduledStream> scheduledStreams)
        {
            if(!scheduledStreams.Any())
            {
                return new Notification()
                {
                    Embeds = new Embed[]
                    {
                        new Embed()
                        {
                            Title = "This Week",
                            Url = $"{CALENDAR_URL_TEMPLATE}{AppSettings.ScheduleGoogleCalendarId}",
                            Color = 48639,
                            Description = "No scheduled streams this week. Keep an eye on <#766821120522715141> for unscheduled streams"
                        }
                    }
                };
            }

            var streamsByDay = scheduledStreams.GroupBy(x => x.Start.DayOfWeek).ToDictionary(g => g.Key, g=> g.ToList());

            return new Notification()
            {
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Title = "This Week",
                        Url = $"{CALENDAR_URL_TEMPLATE}{AppSettings.ScheduleGoogleCalendarId}",
                        Color = 48639,
                        Fields = streamsByDay.OrderBy(x => ((int) x.Key + 6) % 7).Select(streams =>
                            new EmbedField()
                            {
                                Name = streams.Key.ToString(),
                                Value = string.Join("\n\n",streams.Value.Select(x => GetStreamSummaryString(x))),
                                Inline = false
                        }).ToArray()
                    }
                }
            };
        }

        private static string GetStreamSummaryString(ScheduledStream stream)
        {
            return $"{stream.Title}\n{stream.Game}\n{stream.Start:HH:mm} - {stream.End:HH:mm} UTC";
        }
    }
}
