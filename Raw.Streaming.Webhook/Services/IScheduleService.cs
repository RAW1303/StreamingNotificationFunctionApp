using Raw.Streaming.Webhook.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface IScheduleService
    {
        public Task<IReadOnlyList<StreamEvent>> GetScheduledStreamsAsync(DateTime from, DateTime to);
    }
}
