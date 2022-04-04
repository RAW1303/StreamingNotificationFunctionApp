using AutoMapper;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Model.Youtube;

namespace Raw.Streaming.Webhook
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TwitchChannel, GoLive>();
            CreateMap<StreamEvent, Event>();
            CreateMap<TwitchClip, Clip>();
            CreateMap<TwitchVideo, Video>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UserName));
            CreateMap<YoutubeFeed, Video>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.VideoId))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Link));
        }
    }
}
