using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordMessageTranslator
    {
        public static Message TranslateDailySchedule(IEnumerable<Event> events)
        {
            return new Message()
            {
                Embeds = events.Select(e =>
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = "Today"
                        },
                        Title = e.Title,
                        Description = e.Url,
                        Color = 48639,
                        Fields = new EmbedField[] {
                            new EmbedField()
                            {
                                Name = "Game",
                                Value = e.Game,
                                Inline = true
                            },
                            new EmbedField()
                            {
                                Name = "Time",
                                Value = $"{e.Start:t} - {e.End:t} UTC",
                                Inline = true
                            },
                        }
                    }).ToArray()
            };
        }

        internal static Message TranslateWeeklySchedule(IEnumerable<Event> scheduledStreams)
        {
            if(!scheduledStreams.Any())
            {
                return new Message()
                {
                    Embeds = new Embed[]
                    {
                        new Embed()
                        {
                            Title = "This Week",
                            Color = 48639,
                            Description = "No scheduled streams this week. Keep an eye on <#766821120522715141> for unscheduled streams"
                        }
                    }
                };
            }

            var streamsByDay = scheduledStreams.GroupBy(x => x.Start.DayOfWeek).ToDictionary(g => g.Key, g=> g.ToList());

            return new Message()
            {
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Title = "This Week",
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

        private static string GetStreamSummaryString(Event stream)
        {
            return $"{stream.Title}\n{stream.Game}\n{stream.Start:HH:mm} - {stream.End:HH:mm} UTC";
        }
    }
}
