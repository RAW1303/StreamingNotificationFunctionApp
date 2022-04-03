using AutoMapper;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model.Twitch;

namespace Raw.Streaming.Webhook
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TwitchClip, Clip>();
        }
    }
}
