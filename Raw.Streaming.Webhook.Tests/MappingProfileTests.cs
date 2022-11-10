using Google.Apis.YouTube.v3.Data;

namespace Raw.Streaming.Webhook.Tests
{
    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;
        private MapperConfiguration _config;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = new Mapper(_config);
        }

        [Test]
        public void MappingProfile_IsValid()
        {
            _config.AssertConfigurationIsValid();
        }

        [Test, AutoData]
        public void MappingProfile_TwitchChannelToGoLive_Succeeds(string title, string gameName, string broadcasterName)
        { 
            // Arrange
            var twitchChannel = new TwitchChannel()
            {
                Title = title,
                GameName = gameName,
                BroadcasterName = broadcasterName
            };

            // Act
            var result = _mapper.Map<GoLive>(twitchChannel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Property("Title").EqualTo(title));
            Assert.That(result, Has.Property("GameName").EqualTo(gameName));
            Assert.That(result, Has.Property("BroadcasterName").EqualTo(broadcasterName));
            Assert.That(result, Has.Property("Url").Contains(broadcasterName));
        }

        [Test, AutoData]
        public void MappingProfile_YoutubeFeedToVideo_Succeeds(string videoId, string authorName, string link)
        {
            // Arrange
            var youtubeFeed = new YoutubeFeed() 
            { 
                VideoId = videoId, 
                Author = new Author() { Name = authorName },
                Link = link
            };

            // Act
            var result = _mapper.Map<Common.Model.Video>(youtubeFeed);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Property("Id").EqualTo(videoId));
            Assert.That(result, Has.Property("AuthorName").EqualTo(authorName));
            Assert.That(result, Has.Property("Url").EqualTo(link));
        }

        [Test, AutoData]
        public void MappingProfile_TwitchScheduleSegmentToEventList_Succeeds(string segmentId, string title, string categoryName, DateTimeOffset startTime, DateTimeOffset endTime, bool isRecurring)
        {
            // Arrange
            var twitchScheduleSegment = new TwitchScheduleSegment()
            {
                Id = new TwitchScheduleId { SegmentId = segmentId },
                Category = new TwitchGame { Name = categoryName },
                StartTime = startTime,
                EndTime = endTime,
                Title = title,
                IsRecurring = isRecurring
            };

            // Act
            var result = _mapper.Map<Event>(twitchScheduleSegment);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Property("Title").EqualTo(title));
            Assert.That(result, Has.Property("Description").EqualTo(categoryName));
            Assert.That(result, Has.Property("Start").EqualTo(startTime));
            Assert.That(result, Has.Property("End").EqualTo(endTime));
            Assert.That(result, Has.Property("IsRecurring").EqualTo(isRecurring));
        }

        [Test, AutoData]
        public void MappingProfile_TwitchScheduleToEventList_Succeeds(string segmentId, string broadcasterName, string title, string categoryName, DateTimeOffset startTime, DateTimeOffset endTime, bool isRecurring)
        {
            // Arrange
            var twitchSchedule = new TwitchSchedule()
            {
                BroadcasterName = broadcasterName,
                Segments = new TwitchScheduleSegment[]
                {
                    new TwitchScheduleSegment()
                    {
                        Id = new TwitchScheduleId {SegmentId =  segmentId},
                        Category = new TwitchGame { Name = categoryName },
                        StartTime = startTime,
                        EndTime = endTime,
                        Title = title,
                        IsRecurring = isRecurring
                    }
                }
            };

            // Act
            var result = _mapper.Map<IEnumerable<Event>>(twitchSchedule);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.One.With.Property("Title").EqualTo(title));
            Assert.That(result, Has.One.With.Property("Description").EqualTo(categoryName));
            Assert.That(result, Has.One.With.Property("Start").EqualTo(startTime));
            Assert.That(result, Has.One.With.Property("End").EqualTo(endTime));
            Assert.That(result, Has.One.With.Property("Url").Contains(broadcasterName));
            Assert.That(result, Has.One.With.Property("Url").Contains(segmentId));
            Assert.That(result, Has.One.With.Property("IsRecurring").EqualTo(isRecurring));
        }

        [Test]
        public void MappingProfile_YoutubeVideoToEvent_Succeeds()
        {
            // Arrange
            var liveStreamingDetails = _fixture.Build<VideoLiveStreamingDetails>().Without(x => x.ScheduledEndTime).Create();
            var youtubeVideo = _fixture.Build<Google.Apis.YouTube.v3.Data.Video>().With(x => x.LiveStreamingDetails, liveStreamingDetails).Create();

            // Act
            var result = _mapper.Map<Event>(youtubeVideo);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Property("End").Null);
        }
    }
}
