namespace Raw.Streaming.Webhook.Tests
{
    [TestFixture]
    public class MappingProfileTests
    {
        public IMapper _mapper;
        public MapperConfiguration _config;

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

        [Test]
        public void MappingProfile_YoutubeFeedToVideo_Succeeds()
        {
            // Arrange
            var videoId = "TestID";
            var authorName = "TestAuthor";
            var link = "Link";

            var youtubeFeed = new YoutubeFeed() 
            { 
                VideoId = videoId, 
                Author = new Author() { Name = authorName },
                Link = link
            };

            // Act
            var result = _mapper.Map<Video>(youtubeFeed);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Property("Id").EqualTo(videoId));
            Assert.That(result, Has.Property("AuthorName").EqualTo(authorName));
            Assert.That(result, Has.Property("Url").EqualTo(link));
        }
    }
}
