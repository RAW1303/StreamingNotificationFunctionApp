namespace Raw.Streaming.Common.Model
{
    public class Video : Entity
    {
        public string Id { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
