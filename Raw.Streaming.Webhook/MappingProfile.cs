using AutoMapper;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Model.Youtube;
using System.Collections.Generic;
using YoutubeVideo = Google.Apis.YouTube.v3.Data.Video;

namespace Raw.Streaming.Webhook;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TwitchChannel, GoLive>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"{AppSettings.TwitchBaseUrl}/{src.BroadcasterName}"));
        CreateMap<TwitchSchedule, Event>()
            .ForMember(dest => dest.Start, opt => opt.Ignore())
            .ForMember(dest => dest.End, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.IsRecurring, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"{AppSettings.TwitchBaseUrl}/{src.BroadcasterName}/schedule"));
        CreateMap<TwitchSchedule, IEnumerable<Event>>()
            .ConvertUsing<TwitchScheduleToEventListConverter>();
        CreateMap<TwitchScheduleSegment, Event>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.IsRecurring, opt => opt.MapFrom(src => src.IsRecurring))
            .ForMember(dest => dest.Url, opt => opt.MapFrom<TwitchScheduleUrlResolver>());
        CreateMap<YoutubeVideo, Event>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.LiveStreamingDetails.ScheduledStartTime))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.LiveStreamingDetails.ScheduledEndTime))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Snippet.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Snippet.Description))
            .ForMember(dest => dest.IsRecurring, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"{AppSettings.YoutubeVideoUrl}{src.Id}"));
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
        foreach (var segment in source.SegmentsExcludingVaction)
        {
            var model = context.Mapper.Map<Event>(source);
            context.Mapper.Map(segment, model);
            yield return model;
        }
    }
}

internal class TwitchScheduleUrlResolver : IValueResolver<TwitchScheduleSegment, Event, string>
{
    public string Resolve(TwitchScheduleSegment source, Event destination, string member, ResolutionContext context)
    {
        var queryString = source.IsRecurring ? $"segmentID={source.Id.SegmentId}" : $"seriesID={source.Id.SegmentId}";
        return $"{destination.Url}?{queryString}";
    }
}