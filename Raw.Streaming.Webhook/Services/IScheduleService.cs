using Raw.Streaming.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services;
internal interface IScheduleService
{
    Task<IEnumerable<Event>> GetScheduleAsync(DateTimeOffset from, DateTimeOffset? to = null);
}
