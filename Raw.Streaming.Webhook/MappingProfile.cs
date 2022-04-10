﻿using AutoMapper;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Model.Youtube;

namespace Raw.Streaming.Webhook
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TwitchChannel, GoLive>();
            CreateMap<StreamEvent, Event>()
                .ForMember(dest => dest.Url, opt => opt.Ignore());
            CreateMap<TwitchClip, Clip>()
                .ForMember(dest => dest.GameName, opt => opt.Ignore());
            CreateMap<TwitchVideo, Video>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.AuthorName, opt => opt.Ignore());
            CreateMap<YoutubeFeed, Video>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.VideoId))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore());
        }
    }
}
