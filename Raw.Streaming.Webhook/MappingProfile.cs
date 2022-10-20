using AutoMapper;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Model.Youtube;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Webhook;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TwitchChannel, GoLive>();
        CreateMap<TwitchSchedule, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Start, opt => opt.Ignore())
            .ForMember(dest => dest.End, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.IsRecurring, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"https://twitch.tv/{src.BroadcasterName}"));
        CreateMap<TwitchSchedule, IEnumerable<Event>>()
            .ConvertUsing<TwitchScheduleToEventListConverter>();
        CreateMap<TwitchScheduleSegment, Event>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => $"TwitchEvent_{src.Id}"))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Game, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.IsRecurring, opt => opt.MapFrom(src => src.IsRecurring))
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
internal class TwitchScheduleToEventListConverter : ITypeConverter<TwitchSchedule, IEnumerable<Event>>
{
    public IEnumerable<Event> Convert(TwitchSchedule source, IEnumerable<Event> destination, ResolutionContext context)
    {
        foreach (var model in source.Segments.Select(e => context.Mapper.Map<Event>(e)))
        {
            context.Mapper.Map(source, model);
            yield return model;
        }
    }
}